using FMODUnity;
using Game.AI.BossSystem;
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

		[SerializeField, BoxGroup("Core"), Tooltip("Values left than 0 will be treated as infinite.")]
		private float Lifespan = 5f;

		[SerializeField, BoxGroup("On Hit")]
		private EventReference ThrowSound;

		[SerializeField, BoxGroup("On Hit")]
		private EventReference HitSound;

		[SerializeField, BoxGroup("On Hit")]
		private GameObject SpawnOnHit;

		private Vector2 _direction;

		private void Update()
		{
			Move(_direction * Speed, RotateToFace, RotationalOffset);
		}

		protected override void OnSpawn(Vector2 target)
		{
			base.OnSpawn(target);

			_direction = target - (Vector2)transform.position;
			_direction.Normalize();

			if (!ThrowSound.IsNull)
			{
				RuntimeManager.PlayOneShotAttached(ThrowSound, gameObject);
			}

			if (Lifespan > 0f)
			{
				Destroy(gameObject, Lifespan);
			}
		}

		protected override void OnHitDamageable(Health health)
		{
			if (SpawnOnHit)
			{
				Instantiate(SpawnOnHit);
			}

			if (!HitSound.IsNull)
			{
				RuntimeManager.PlayOneShot(HitSound, transform.position);
			}

			var dmgInfo = new DamageInfo()
			{
				Type = Owner.TryGetComponent(out BossBrain _)
				? DamageType.Boss : DamageType.Enemy,
				Damage = Damage,
				Direction = Direction
			};

			OnDamage?.Invoke(dmgInfo);
			health.TakeDamage(dmgInfo);

			Destroy(gameObject);
		}

		protected override bool CanCollide(Collider2D collider)
		{
			return base.CanCollide(collider) || collider.gameObject.layer == LayerMask.NameToLayer("Obstacle");
		}
	}
}
