using Game.AnimationSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Characters
{
	public class Wanderer : Character
	{
		[Space, SerializeField, BoxGroup("Animation"), Required]
		private AnimationClip IdleAnim;

		[SerializeField, BoxGroup("Animation"), Required]
		private AnimationClip MoveAnim;

		[SerializeField, BoxGroup("Animation"), Required]
		private AnimationClip DashAnim;

		protected override void Update()
		{
			base.Update();

			if (!IsPossessed)
			{
				Movement.Move(Vector2.zero);
			}

			if (!Movement.IsDashing)
			{
				Animator.Play(Movement.IsMoving ? MoveAnim : IdleAnim);
			}
		}

		public override void Attack(Vector2 target)
		{
			// TODO: Add attack.
		}

		public override void Dash()
		{
			base.Dash();
			Animator.Play(DashAnim);
		}
	}
}
