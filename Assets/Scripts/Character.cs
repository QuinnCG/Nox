using Sirenix.OdinInspector;
using UnityEngine;

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

		private float _nextDashTime;

		protected Movement Movement { get; private set; }

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
		}

		public bool IsPossessed { get; private set; }

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
