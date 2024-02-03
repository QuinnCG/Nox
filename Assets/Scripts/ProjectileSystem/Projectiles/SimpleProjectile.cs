using FMODUnity;
using Game.AI.BossSystem;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Game.ProjectileSystem
{
	public class SimpleProjectile : Projectile
	{
		[field: SerializeField, Unit(Units.MetersPerSecond), BoxGroup("Core")]
		public float Speed { get; set; } = 14f;

		[SerializeField, BoxGroup("Core"), Unit(Units.DegreesPerSecond)]
		private float RotationRate = 0f;

		[SerializeField, BoxGroup("Core"), EnableIf(nameof(RotationRate), 0f), Tooltip("RotationRate must equal 0f for RotateToFace to be allowable.")]
		private bool RotateToFace = true;

		[SerializeField, Unit(Units.Degree), ShowIf(nameof(RotateToFace)), BoxGroup("Core")]
		private float RotationalOffset = -45f;

		[SerializeField, BoxGroup("Core")]
		protected float Damage = 25f;

		[SerializeField, BoxGroup("Core"), Tooltip("Values less than 0 will be treated as infinite.")]
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
			bool rotateToFace = RotationRate == 0f && RotateToFace;
			Move(_direction * Speed, rotateToFace, RotationalOffset);

			float angle = transform.rotation.eulerAngles.z;
			transform.rotation = Quaternion.AngleAxis(angle + (RotationRate * Time.deltaTime), Vector3.forward);
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
				StartCoroutine(LifespanSequence());
			}
		}

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
				Destroy(gameObject);
			}
		}

		protected override void OnHitObstacle(Collider2D collider)
		{
			TrySpawnHitPrefab();
			TryPlayHitSound();

			Destroy(gameObject);
		}

		protected override bool CanCollide(Collider2D collider)
		{
			return base.CanCollide(collider) || collider.gameObject.layer == LayerMask.NameToLayer("Obstacle");
		}

		protected void TrySpawnHitPrefab()
		{
			if (SpawnOnHit)
			{
				Instantiate(SpawnOnHit, transform.position, Quaternion.identity);
			}
		}

		protected void TryPlayHitSound()
		{
			if (!HitSound.IsNull)
			{
				RuntimeManager.PlayOneShot(HitSound, transform.position);
			}
		}

		protected virtual void OnLifespanEnd() { }

		private IEnumerator LifespanSequence()
		{
			yield return new WaitForSeconds(Lifespan);

			OnLifespanEnd();
			Destroy(gameObject);
		}
	}
}
