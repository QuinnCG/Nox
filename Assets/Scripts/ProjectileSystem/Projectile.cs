using Game.DamageSystem;
using Game.Player;
using UnityEngine;

namespace Game.ProjectileSystem
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public abstract class Projectile : MonoBehaviour
	{
		public Vector2 CrosshairPos => CrosshairManager.Instance.CurrentPosition;

		private Rigidbody2D _rb;
		private Vector2 _vel;

		public static void Spawn(GameObject prefab)
		{

		}
		public static void Spawn(string key)
		{

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

		public void Move(Vector2 velocity)
		{
			_vel = velocity;
		}

		protected virtual void OnSpawn(Vector2 direction) { }

		protected virtual void OnCollide(Health health) { }
	}
}
