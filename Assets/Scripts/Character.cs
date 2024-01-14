﻿using Sirenix.OdinInspector;
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

		public bool IsPossessed { get; private set; }

		protected Movement Movement { get; private set; }

		private Animator _animator;
		private float _nextDashTime;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			_animator = GetComponentInChildren<Animator>();
		}

		protected virtual void Update()
		{
			_animator.SetBool("IsMoving", Movement.IsMoving);

			// Any subclass that overrides this method (provided they make a call to base)
			// will not have thier update method called if they're possessed.
			if (IsPossessed)
			{
				return;
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
