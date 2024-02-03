using DG.Tweening;
using Game.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Characters
{
	public class FlintLockBandit : Character
	{
		[SerializeField]
		private float GunOffset = 0.4f;

		[SerializeField, Required, BoxGroup("References")]
		private Transform GunPivot;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject GunPrefab;

		[SerializeField, Required, BoxGroup("Animations")]
		private AnimationClip IdleAnim, MoveAnim, RollAnim, DeathAnim;

		private Transform _gun;

		protected override void Update()
		{
			base.Update();

			if (!Movement.IsDashing)
			{
				Animator.Play(Movement.IsMoving ? MoveAnim : IdleAnim);
			}

			if (_gun != null)
			{
				UpdateGunPosition();
			}
		}

		protected override void OnPossess()
		{
			var instance = Instantiate(GunPrefab, transform.position, Quaternion.identity).transform;
			_gun = instance.transform;
		}

		protected override void OnUnpossess()
		{
			if (_gun.gameObject != null)
			{
				Destroy(_gun.gameObject);
				_gun = null;
			}
		}

		protected override void OnDash()
		{
			Animator.Play(RollAnim);
			base.OnDash();
		}

		protected override void OnAttack(Vector2 target)
		{
			
		}

		protected override void OnDeath()
		{
			GetComponent<Collider2D>().enabled = false;
			Animator.Play(DeathAnim);
			DOVirtual.DelayedCall(DeathAnim.length - 0.01f, () => Destroy(gameObject));
		}

		private void UpdateGunPosition()
		{
			Vector2 cursor = CrosshairManager.Instance.CurrentPosition;
			Vector2 center = GunPivot.position;

			Vector2 dir = cursor - center;
			dir.Normalize();

			Vector2 final = center + (dir * GunOffset);
			_gun.position = final;
		}
	}
}
