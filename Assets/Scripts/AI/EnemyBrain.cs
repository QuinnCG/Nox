using DG.Tweening;
using Game.DamageSystem;
using Game.MovementSystem;
using Game.Player;
using Game.ProjectileSystem;
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEngine;

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
				Debug.Log(
					"Active State: ".Color(StringColor.White)
					+ _stateMachine.Active.Name.Color(StringColor.Yellow).Bold()
					+ ".".Color(StringColor.White));
			}
#endif
		}

		/* STATE MACHINE */
		protected State CreateState(Action callback, string name = "No Name")
		{
			return _stateMachine.CreateState(callback, name);
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

		protected TweenCallback Jump(Vector2 target, float height, float duration)
		{
			_jumpTween?.Kill();
			_jumpTween = transform.DOJump(target, height, 1, duration).SetEase(Ease.Linear);

			return _jumpTween.onComplete;
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
	}
}
