using Game.DamageSystem;
using Game.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ProjectileSystem
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public abstract class Projectile : MonoBehaviour
	{
		public static List<Projectile> Spawned { get; } = new();

		public Vector2 CrosshairPos => CrosshairManager.Instance.CurrentPosition;

		public System.Action<DamageInfo> OnDamage { get; set; }
		public System.Action OnHit { get; set; }

		protected Character Owner { get; private set; }
		protected Vector2 Velocity => _rb.velocity;
		protected Vector2 Direction => _rb.velocity.normalized;

		private Rigidbody2D _rb;
		private Vector2 _vel;

		public static Projectile Spawn(GameObject prefab, Vector2 origin, Vector2 target, Character owner)
		{
			var instance = Instantiate(prefab, origin, Quaternion.identity);
			var proj = instance.GetComponent<Projectile>();
			proj.Owner = owner;
			proj.OnSpawn(target);

			Spawned.Add(proj);
			proj.OnHit += () => Spawned.Remove(proj);

			return proj;
		}
		public static void Spawn(GameObject prefab, Vector2 origin, Vector2 target, Character owner, ShootSpawnInfo spawnInfo = default)
		{
			Projectile Shoot(Vector2 tar) => Spawn(prefab, origin, tar, owner);
			Vector2 RotateTarget(float angle, float maxAngle)
			{
				Vector2 diff = target - origin;
				Vector2 dir = diff.normalized;
				float dst = diff.magnitude;

				Vector2 newDir = Quaternion.AngleAxis(angle - (maxAngle / 2f), Vector3.forward) * dir;
				return origin + (newDir * dst);
			}

			Vector2 direction = target - origin;
			direction.Normalize();

			if (spawnInfo.Method == ShootMethod.Straight)
			{
				for (int i = 0; i < spawnInfo.Count; i++)
				{
					Shoot(target);
				}
			}
			else if (spawnInfo.Method == ShootMethod.EvenSpread)
			{
				float angle = spawnInfo.SpreadAngle / spawnInfo.Count;

				if (spawnInfo.Count % 2 == 0)
					angle += angle / 2f;

				for (int i = 0; i < spawnInfo.Count; i++)
				{
					Shoot(RotateTarget(angle * i, spawnInfo.SpreadAngle));
				}
			}
			else if (spawnInfo.Method == ShootMethod.RandomSpread)
			{
				for (int i = 0; i < spawnInfo.Count; i++)
				{
					float angle = Random.Range(0f, spawnInfo.SpreadAngle);
					Shoot(RotateTarget(angle, spawnInfo.SpreadAngle));
				}
			}
			else if (spawnInfo.Method == ShootMethod.EvenCircle)
			{
				float angle = 360f / spawnInfo.Count;

				for (int i = 0; i < spawnInfo.Count; i++)
				{
					Shoot(RotateTarget(angle * i, 360f));
				}
			}
			else if (spawnInfo.Method == ShootMethod.RandomCircle)
			{
				for (int i = 0; i < spawnInfo.Count; i++)
				{
					float angle = Random.Range(0f, 360f);
					Shoot(RotateTarget(angle, 360f));
				}
			}
		}

		protected virtual void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		protected virtual void LateUpdate()
		{
			_rb.velocity = _vel;
			_vel = Vector2.zero;
		}

		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			if (CanCollide(collider) && collider.TryGetComponent(out Health health))
			{
				if (OnHitDamageable(health))
				{
					OnHit?.Invoke();
				}
			}
			else if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				OnHitObstacle(collider);
				OnHit?.Invoke();
			}
		}

		public void Move(Vector2 velocity, bool rotateToFace = true, float rotationOffset = -45f)
		{
			_vel = velocity;

			if (rotateToFace)
			{
				Vector2 dir = velocity.normalized;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationOffset;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
		}

		protected virtual bool CanCollide(Collider2D collider)
		{
			if (collider.gameObject == Owner)
				return false;

			return true;
		}

		protected virtual void OnSpawn(Vector2 target)
		{
			// If the player shot this projectile, notify PossessionManager.
			if (PossessionManager.Instance.PossessedCharacter.gameObject == Owner)
			{
				PlayerManager.Instance.OnSpawnProjectile(this);
			}
		}

		protected virtual bool OnHitDamageable(Health health) => true;

		protected virtual void OnHitObstacle(Collider2D collider)
		{
			Destroy(gameObject);
		}
	}
}
