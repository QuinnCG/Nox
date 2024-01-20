using Game.DamageSystem;
using Game.Player;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.ProjectileSystem
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public abstract class Projectile : MonoBehaviour
	{
		public Vector2 CrosshairPos => CrosshairManager.Instance.CurrentPosition;

		public Action<DamageInfo> OnDamage { get; set; }

		protected GameObject Owner { get; private set; }
		protected Vector2 Velocity => _rb.velocity;
		protected Vector2 Direction => _rb.velocity.normalized;

		private Rigidbody2D _rb;
		private Vector2 _vel;

		public static Projectile Spawn(GameObject prefab, Vector2 origin, Vector2 direction, GameObject owner)
		{
			var instance = Instantiate(prefab, origin, Quaternion.identity);
			var proj = instance.GetComponent<Projectile>();
			proj.Owner = owner;
			proj.OnSpawn(direction);

			return proj;
		}
		public static AsyncOperationHandle Spawn(string key, Vector2 origin, Vector2 direction, GameObject owner)
		{
			var handle = Addressables.InstantiateAsync(key, origin, Quaternion.identity);
			handle.Completed += opHandle =>
			{
				var instance = opHandle.Result;
				var proj = instance.GetComponent<Projectile>();
				proj.Owner = owner;
				proj.OnSpawn(direction);
			};

			return handle;
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
				OnHitDamageable(health);
			}
			else if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				OnHitObstacle(collider);
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

		protected virtual void OnSpawn(Vector2 direction)
		{
			if (PossessionManager.Instance.PossessedCharacter.gameObject == Owner)
			{
				PlayerManager.Instance.OnSpawnProjectile(this);
			}
		}

		protected virtual void OnHitDamageable(Health health) { }

		protected virtual void OnHitObstacle(Collider2D collider)
		{
			Destroy(gameObject);
		}
	}
}
