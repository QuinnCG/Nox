﻿using FMODUnity;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProjectileSystem
{
	public class SimpleProjectile : Projectile
	{
		[field: SerializeField, Unit(Units.MetersPerSecond), BoxGroup("Core")]
		public float Speed { get; set; } = 14f;

		[SerializeField, BoxGroup("Core")]
		private bool RotateToFace = true;

		[SerializeField, Unit(Units.Degree), ShowIf(nameof(RotateToFace)), BoxGroup("Core")]
		private float RotationalOffset = -45f;

		[SerializeField, BoxGroup("Core")]
		private float Damage = 25f;

		[SerializeField, BoxGroup("On Hit")]
		private EventReference HitSound;

		[SerializeField, BoxGroup("On Hit")]
		private GameObject SpawnOnHit;

		private Vector2 _direction;

		private void Update()
		{
			Move(_direction * Speed, RotateToFace, RotationalOffset);
		}

		protected override void OnSpawn(Vector2 direction)
		{
			_direction = direction;
		}

		protected override void OnCollide(Health health)
		{
			if (SpawnOnHit)
			{
				Instantiate(SpawnOnHit);
			}

			health.TakeDamage(Damage, DamageSource.Enemy);
			Destroy(gameObject);
		}
	}
}
