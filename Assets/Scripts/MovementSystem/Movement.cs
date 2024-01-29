using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.MovementSystem
{
	/// <summary>
	/// This is the movement system used by characters in the game.
	/// </summary>
	public class Movement : Locomotion
	{
		[field: SerializeField, Unit(Units.MetersPerSecond)]
		public float MoveSpeed { get; set; } = 7f;

		[Space, SerializeField]
		private EventReference DashSound;

		public bool IsDashing { get; private set; }
		public bool IsMoving { get; private set; }

		private float _knockbackSpeed;
		private Vector2 _knockbackDirection;
		private Tween _knockbackTween;

		private float _dashSpeed;
		private Vector2 _dashDirection;
		private float _dashEndTime;

		private Vector2 _lastMoveDir = Vector2.down;
		private bool _isDashControllable;

		private void Update()
		{
			if (_knockbackSpeed > 0f)
			{
				AddVelocity(_knockbackDirection * _knockbackSpeed);
			}

			if (IsDashing)
			{
				if (Time.time > _dashEndTime)
				{
					IsDashing = false;
					return;
				}

				if (_isDashControllable)
				{
					AddVelocity(_dashDirection * _dashSpeed);
				}
				else
				{
					SetVelocity(_dashDirection * _dashSpeed);
				}
			}
		}

		public void Move(Vector2 direction)
		{
			if (IsDashing) return;

			IsMoving = direction != Vector2.zero;
			Vector2 inputDir = direction.normalized;

			if (direction != Vector2.zero)
			{
				_lastMoveDir = inputDir;
			}

			AddVelocity(MoveSpeed * inputDir);

			// Flip sprite.
			if (direction.x > 0f)
			{
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else if (direction.x < 0f)
			{
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}
		}

		public void Dash(float speed, float duration, bool controllable = false)
		{
			if (!IsDashing)
			{
				IsDashing = true;
				_isDashControllable = controllable;

				_dashEndTime = Time.time + duration;
				_dashSpeed = speed;
				_dashDirection = _lastMoveDir;

				if (!DashSound.IsNull)
				{
					RuntimeManager.PlayOneShot(DashSound, transform.position);
				}
			}
		}

		public void ApplyKnockback(Vector2 velocity, float duration)
		{
			_knockbackSpeed = velocity.magnitude;
			_knockbackDirection = velocity.normalized;

			_knockbackTween?.Kill();
			_knockbackTween = DOTween.To(() => _knockbackSpeed, x => _knockbackSpeed = x, 0f, duration)
				.SetEase(Ease.OutCubic)
				.OnComplete(() => _knockbackSpeed = 0f);
		}
	}
}
