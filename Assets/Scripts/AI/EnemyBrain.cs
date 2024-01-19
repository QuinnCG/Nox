using Game.DamageSystem;
using Game.MovementSystem;
using Game.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public abstract class EnemyBrain : MonoBehaviour
	{
		public PlayerManager PlayerManager { get; private set; }

		[SerializeField]
		private bool ShowDebug;

		public float HP => Health.Current;
		public float MaxHP => Health.Max;
		public bool IsDead => Health.IsDead;

		public Movement Movement { get; private set; }
		public Health Health { get; private set; }
		public Collider2D Collider { get; private set; }
		public Bounds Bounds => Collider.bounds;

		private StateMachine _stateMachine;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Health = GetComponent<Health>();
			Collider = GetComponent<Collider2D>();

			_stateMachine = new StateMachine(this);
		}

		protected virtual void Start()
		{
			PlayerManager = PlayerManager.Instance;
			_stateMachine.Start();
		}

		protected virtual void Update()
		{
			_stateMachine.ShowDebug = ShowDebug;
			_stateMachine.Update();
		}

		/// <summary>
		/// Connect states as if they are part of the state machine. If they aren't they will be added.
		/// </summary>
		/// <param name="origin">The state to transition from.</param>
		/// <param name="next">The state to transition to.</param>
		/// <param name="condition">The method to be polled every frame that decides whether or not to transition.</param>
		protected void Connect(State origin, State next, Func<bool> condition)
		{
			_stateMachine.Connect(origin, next, condition);
		}

		/// <summary>
		/// Sets the first state the state-machine transitions to.
		/// </summary>
		protected void SetStartingState(State state)
		{
			_stateMachine.SetStartingState(state);
		}
	}
}
