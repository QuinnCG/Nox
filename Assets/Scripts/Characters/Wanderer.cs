using Game.Player;
using Game.ProjectileSystem;
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

		[Space, SerializeField, BoxGroup("Attack"), Required]
		private GameObject ProjectilePrefab;

		[SerializeField, BoxGroup("Attack"), Required]
		private Transform ProjectileSpawn;

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
			Vector2 origin = ProjectileSpawn.position;
			Vector2 pos = CrosshairManager.Instance.CurrentPosition;

			Projectile.Spawn(ProjectilePrefab, origin, pos, gameObject);
		}

		public override void Dash()
		{
			base.Dash();
			Animator.Play(DashAnim);
		}
	}
}
