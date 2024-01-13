using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
	[RequireComponent(typeof(Movement))]
	public abstract class Character : MonoBehaviour
	{
		[SerializeField, BoxGroup("Dash")]
		private float DashSpeed = 18f;
		[SerializeField, BoxGroup("Dash")]
		private float DashDuration = 0.2f;
		[SerializeField, BoxGroup("Dash")]
		private float DashCooldown = 0.2f;

		private float _nextDashTime;

		protected Movement Movement { get; private set; }

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
		}

		public bool IsPossessed { get; private set; }

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
	}
}
