using DG.Tweening;
using Game.DamageSystem;
using Game.MovementSystem;
using Game.Player;
using Game.ProjectileSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Game.AI
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public abstract class EnemyBrain : MonoBehaviour
	{
		[SerializeField, BoxGroup("Debug")]
		private bool DebugMode;

		public PlayerManager PlayerManager { get; private set; }

		public float HP => Health.Current;
		public float MaxHP => Health.Max;
		public bool IsDead => Health.IsDead;

		/// <summary>
		/// The movement component on this enemy.
		/// </summary>
		public Movement Movement { get; private set; }
		/// <summary>
		/// The health component on this enemy.
		/// </summary>
		public Health Health { get; private set; }
		/// <summary>
		/// The hitbox for this enemy.
		/// </summary>
		public Collider2D Collider { get; private set; }
		/// <summary>
		/// The bounds of the hitbox for this enemy.
		/// </summary>
		public Bounds Bounds => Collider.bounds;

		public Vector2 PlayerPosition
		{
			get
			{
				var possessed = PossessionManager.Instance.PossessedCharacter;
				if (possessed != null)
					return possessed.transform.position;
				else
					return transform.position;
			}
		}
		public float PlayerHealthPercent
		{
			get
			{
				var health = PossessionManager.Instance.PossessedHealth;
				if (health != null)
					return health.Current / health.Max;
				else
					return 0f;
			}
		}
		public Vector2 DirectionToPlayer => (PlayerPosition - (Vector2)transform.position).normalized;
		public float DistanceToPlayer => Vector2.Distance(transform.position, PlayerPosition);

		public State ActiveState => _stateMachine.Active;

		/// <summary>
		/// True if any of the EnemyBrain jump methods are used to make the character jump.
		/// </summary>
		protected bool IsJumping
		{
			get
			{
				if (_jumpTween == null) return false;
				return _jumpTween.IsActive();
			}
		}

		private readonly StateMachine _stateMachine = new();
		private Tween _jumpTween;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Health = GetComponent<Health>();
			Collider = GetComponent<Collider2D>();
		}

		protected virtual void Start()
		{
			PlayerManager = PlayerManager.Instance;
		}

		protected virtual void Update()
		{
			if (PossessionManager.Instance.PossessedCharacter != null)
			{
				_stateMachine.Update();
			}

#if UNITY_EDITOR
			if (DebugMode)
			{
				string state = ActiveState != null ? _stateMachine.Active.Name : "None";

				Debug.Log(
					"Active State: ".Color(StringColor.White)
					+ state.Color(StringColor.Yellow).Bold()
					+ ".".Color(StringColor.White));
			}
#endif
		}

		/* STATE MACHINE */
		protected State CreateState(Action callback, string name = "No Name")
		{
			return _stateMachine.CreateState(callback, name);
		}
		protected State CreateState(Func<IEnumerator> coroutineCallback, string name = "No Name")
		{
			return _stateMachine.CreateState(coroutineCallback, name);
		}

		protected void TransitionTo(State state)
		{
			_stateMachine.TransitionTo(state);
		}

		/* UTILITY */
		protected void Move(Vector2 direction)
		{
			Movement.Move(direction);
		}

		/// <summary>
		/// Moves towards a target point and returns true when its within "stoppingDistance".
		/// </summary>
		/// <returns>True if within "stoppingDistance" of target, false otherwise.</returns>
		protected bool MoveTo(Vector2 target, float stoppingDistance = 0.2f)
		{
			var diff = target - (Vector2)transform.position;
			Move(diff.normalized);

			return Vector2.Distance(transform.position, target) < stoppingDistance;
		}

		protected void Dash(Vector2 direction, float speed, float duration, bool contrallable = false)
		{
			Movement.Move(direction);
			Movement.Dash(speed, duration, contrallable);
		}

		protected void Shoot(GameObject prefab, Vector2 origin, Vector2 target, ShootSpawnInfo info = default)
		{
			Projectile.Spawn(prefab, origin, target, gameObject, info);
		}

		protected Tween Jump(Vector2 target, float height, float duration)
		{
			_jumpTween?.Kill();
			_jumpTween = transform.DOJump(target, height, 1, duration).SetEase(Ease.Linear);

			return _jumpTween;
		}

		protected Tween SuperJump(Vector2 target, float height, float duration, GameObject shadowPrefab)
		{
			Collider.enabled = false;
			GameObject shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);

			Vector2 jumpStart = transform.position;
			Vector2 jumpEnd = target;

			var tween = Jump(jumpEnd, height, duration);
			tween.onUpdate += () =>
			{
				float progress = tween.Elapsed() / tween.Duration();
				shadow.transform.position = Vector2.Lerp(jumpStart, jumpEnd, progress);
			};
			tween.onComplete += () =>
			{
				Collider.enabled = true;
				Destroy(shadow);
			};

			return tween;
		}

		protected void Delay(float delay, TweenCallback callback)
		{
			DOVirtual.DelayedCall(delay, callback, false);
		}

		/// <summary>
		/// Returns true if no "Obstacle" is between "origin" and "target", false otherwise.
		/// </summary>
		protected bool LineOfSight(Vector2 origin, Vector2 target)
		{
			const string layer = "Obstacle";
			return !Physics2D.Linecast(origin, target, LayerMask.GetMask(layer));
		}

		protected void TeleportTo(Vector2 target)
		{
			transform.position = target;
		}

		protected Vector2 GetPointClosestTo(Vector2 to, params Transform[] points)
		{
			float cloestDst = float.PositiveInfinity;
			Transform closestTransform = null;

			foreach (var transform in points)
			{
				float dst = Vector2.Distance(to, transform.position);
				if (dst < cloestDst)
				{
					cloestDst = dst;
					closestTransform = transform;
				}
			}

			return closestTransform.position;
		}

		protected Vector2 GetPointFarthestFrom(Vector2 from, params Transform[] points)
		{
			float farthestDst = 0f;
			Transform farthestTransform = null;

			foreach (var transform in points)
			{
				float dst = Vector2.Distance(from, transform.position);
				if (dst > farthestDst)
				{
					farthestDst = dst;
					farthestTransform = transform;
				}
			}

			return farthestTransform.position;
		}

		protected Vector2 GetRandomFromTransforms(params Transform[] transforms)
		{
			return transforms[UnityEngine.Random.Range(0, transforms.Length - 1)].position;
		}

		protected YieldUntil SpawnVFXOneShot(GameObject prefab, Vector2 position, bool destroyWhenFinished = true)
		{
			GameObject instance = Instantiate(prefab, position, Quaternion.identity);
			if (destroyWhenFinished)
			{
				VisualEffect vfx = instance.GetComponent<VisualEffect>();

				Task.Run(() =>
				{
					bool isValid = instance != null;
					while (vfx.aliveParticleCount > 0 && isValid) Debug.Log(vfx.aliveParticleCount);

					if (isValid)
					{
						Debug.Log("Destroyed!");
						Destroy(instance);
					}
				});

				return new YieldUntil(() => vfx.aliveParticleCount == 0);
			}

			return new YieldUntil(() => true);
		}
	}
}
