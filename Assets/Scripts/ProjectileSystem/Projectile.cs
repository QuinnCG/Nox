using Game.DamageSystem;
using Game.Player;
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

		private Rigidbody2D _rb;
		private Vector2 _vel;

		public static Projectile Spawn(GameObject prefab, Vector2 origin, Vector2 direction)
		{
			var instance = Instantiate(prefab, origin, Quaternion.identity);
			var proj = instance.GetComponent<Projectile>();
			proj.OnSpawn(direction);

			return proj;
		}
		public static AsyncOperationHandle Spawn(string key, Vector2 origin, Vector2 direction)
		{
			var handle = Addressables.InstantiateAsync(key, origin, Quaternion.identity);
			handle.Completed += opHandle =>
			{
				var instance = opHandle.Result;
				var proj = instance.GetComponent<Projectile>();
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
			if (collider.TryGetComponent(out Health health))
			{
				var possessed = PlayerManager.Instance.PossessedCharacter;
				if (health.gameObject != possessed.gameObject)
				{
					OnCollide(health);
				}
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

		protected virtual void OnSpawn(Vector2 direction) { }

		protected virtual void OnCollide(Health health) { }
	}
}