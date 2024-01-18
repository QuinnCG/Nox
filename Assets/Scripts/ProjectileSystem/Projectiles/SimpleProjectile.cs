using Game.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProjectileSystem
{
	public class SimpleProjectile : Projectile
	{
		[SerializeField, Unit(Units.MetersPerSecond)]
		private float Speed = 14f;

		[SerializeField]
		private float Damage = 25f;

		[Space, SerializeField]
		private GameObject SpawnOnDeath;

		private Vector2 _direction;

		private void Update()
		{
			Move(_direction * Speed);
		}

		protected override void OnSpawn(Vector2 direction)
		{
			_direction = direction;
		}

		protected override void OnCollide(Health health)
		{
			if (SpawnOnDeath)
			{
				Instantiate(SpawnOnDeath);
			}

			health.TakeDamage(Damage, DamageSource.Minion);
			Destroy(gameObject);
		}
	}
}
