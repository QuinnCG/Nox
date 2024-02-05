﻿using Game.DamageSystem;
using Game.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProjectileSystem
{
	public class SimpleTrailProjectile : SimpleProjectile
	{
		[SerializeField, Required, Tooltip("Must be a child."), BoxGroup("On Hit")]
		private Transform Trail;

		[SerializeField, BoxGroup("On Hit")]
		private float LifespanAfterHit = 2f;

		private bool _hit;

		protected override bool OnHitDamageable(Health health)
		{
			if (_hit) return false;

			TrySpawnHitPrefab();
			TryPlayHitSound();

			var dmgInfo = new DamageInfo()
			{
				//Type = Owner.TryGetComponent(out BossBrain _)
				//? DamageType.Boss : DamageType.Enemy,
				Damage = Damage,
				Direction = Direction,
				Source = Owner
			};

			OnDamage?.Invoke(dmgInfo);
			bool success = health.TakeDamage(dmgInfo);

			if (success)
			{
				_hit = true;
				DetachChild();
				Destroy(gameObject);
			}

			return success;
		}

		protected override void OnHitObstacle(Collider2D collider)
		{
			TrySpawnHitPrefab();
			TryPlayHitSound();

			DetachChild();
			Destroy(gameObject);
		}

		protected override void OnLifespanEnd()
		{
			DetachChild();
		}

		private void DetachChild()
		{
			if (Trail != null && Trail.transform.parent == transform)
			{
				Trail.transform.parent = null;
				Trail.transform.position = transform.position;

				Destroy(Trail.gameObject, LifespanAfterHit);
			}
		}
	}
}
