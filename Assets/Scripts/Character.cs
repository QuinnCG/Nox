using Game.AnimationSystem;
using Game.MovementSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Game
{
	/// <summary>
	/// This class is the main control for characters.
	/// The player controller will tell this class what to do.
	/// AI controllers will tell this class what to do.
	/// </summary>
	[RequireComponent(typeof(Movement))]
	public class Character : MonoBehaviour
	{
		[SerializeField, BoxGroup("Dash")]
		private float DashSpeed = 18f;

		[SerializeField, BoxGroup("Dash")]
		private float DashDuration = 0.2f;

		[SerializeField, BoxGroup("Dash")]
		private float DashCooldown = 0.2f;

		[Space, SerializeField]
		private VisualEffect DashTrail;

		public bool IsPossessed { get; private set; }

		protected Movement Movement { get; private set; }
		protected PlayableAnimator Animator { get; private set; }

		private float _nextDashTime;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Animator = GetComponentInChildren<PlayableAnimator>();
		}

		protected virtual void Update()
		{
			if (DashTrail)
			{
				if (Movement.IsDashing)
					DashTrail.SetVector3("Direction", Movement.Velocity.normalized * -1f);

				DashTrail.SetBool("Spawn", Movement.IsDashing);
			}
		}

		public void Possess()
		{
			if (!IsPossessed)
			{
				IsPossessed = true;
			}
		}

		public void UnPossess()
		{
			if (IsPossessed)
			{
				IsPossessed = false;
			}
		}

		public virtual void Attack(Vector2 target) { }

		public virtual void Dash()
		{
			if (Time.time > _nextDashTime)
			{
				Movement.Dash(DashSpeed, DashDuration);
				_nextDashTime = Time.time + DashDuration + DashCooldown;
			}
		}

		protected void CastDamageBox(Vector2 origin, Vector2 size, float damage, Vector2 direction, float knockback)
		{
			// TODO: Create hitbox.
		}
	}
}
