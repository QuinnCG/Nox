using Game.AI.BossSystem;
using Game.DamageSystem;
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

		protected override void OnHitDamageable(Health health)
		{
			TrySpawnHitPrefab();
			TryPlayHitSound();

			var dmgInfo = new DamageInfo()
			{
				Type = Owner.TryGetComponent(out BossBrain _)
				? DamageType.Boss : DamageType.Enemy,
				Damage = Damage,
				Direction = Direction
			};

			OnDamage?.Invoke(dmgInfo);
			bool success = health.TakeDamage(dmgInfo);

			if (success)
			{
				DetachChild();
				Destroy(gameObject);
			}
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
