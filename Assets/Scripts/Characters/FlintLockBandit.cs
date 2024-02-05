using DG.Tweening;
using FMODUnity;
using Game.AI;
using Game.DamageSystem;
using Game.GeneralManagers;
using Game.Player;
using Game.ProjectileSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Characters
{
	public class FlintLockBandit : Character
	{
		[SerializeField]
		private float GunOffset = 0.4f;

		[SerializeField]
		private float FireRate = 0.2f;

		[SerializeField, Required, BoxGroup("References")]
		private Transform GunPivot;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject GunPrefab;

		[field: SerializeField, Required, BoxGroup("References")]
		public GameObject ProjectilePrefab { get; private set; }

		[field: SerializeField, Required, BoxGroup("Animations")]
		public AnimationClip IdleAnim { get; private set; }

		[field: SerializeField, Required, BoxGroup("Animations")]
		public AnimationClip MoveAnim { get; private set; }

		[field: SerializeField, Required, BoxGroup("Animations")]
		public AnimationClip RollAnim { get; private set; }

		[field: SerializeField, Required, BoxGroup("Animations")]
		public AnimationClip DeathAnim { get; private set; }

		[SerializeField, BoxGroup("SFX")]
		private EventReference ShootSound;

		private Transform _gun;
		private Health _health;
		private Timer _shootTimer;

		private void Start()
		{
			_health = GetComponent<Health>();

			var instance = Instantiate(GunPrefab, GunPivot.position, Quaternion.identity).transform;
			_gun = instance.transform;

			_shootTimer = new Timer();
		}

		protected override void Update()
		{
			base.Update();
			_gun.gameObject.SetActive(!_health.IsDead && !Movement.IsDashing);

			if (IsPossessed)
			{
				if (!Movement.IsDashing)
				{
					Animator.Play(Movement.IsMoving ? MoveAnim : IdleAnim);
				}
			}

			if (_gun != null)
			{
				UpdateGunPosition();
			}
		}

		private void OnDestroy()
		{
			if (_gun != null)
			{
				Destroy(_gun.gameObject);
			}
		}

		protected override void OnDash()
		{
			Animator.Play(RollAnim);
			base.OnDash();
		}

		protected override void OnAttack(Vector2 target)
		{
			if (_shootTimer.IsDone)
			{
				_shootTimer.Reset(FireRate);

				Projectile.Spawn(ProjectilePrefab, _gun.position, target, this);
				if (!ShootSound.IsNull)
				{
					AudioManager.PlayOneShot(ShootSound, _gun.position);
				}
			}
		}

		protected override void OnDeath()
		{
			GetComponent<Collider2D>().enabled = false;
			GetComponent<EnemyBrain>().enabled = false;
			Animator.Play(DeathAnim);
			DOVirtual.DelayedCall(DeathAnim.length - 0.01f, () => Destroy(gameObject));
		}

		private void UpdateGunPosition()
		{
			Vector2 target;

			if (IsPossessed)
			{
				target = CrosshairManager.Instance.CurrentPosition;
			}
			else
			{
				target = PossessionManager.Instance.Position;
			}

			Vector2 center = GunPivot.position;

			Vector2 dir = target - center;
			dir.Normalize();

			// Position.
			Vector2 finalPos = center + (dir * GunOffset);
			_gun.position = finalPos;

			// Rotation.
			float angle = Mathf.Atan2(dir.y, dir.x);
			_gun.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);

			// Scale.
			_gun.localScale = new Vector3(1f, dir.x < 0f ? -1f : 1f, 1f);
		}
	}
}
