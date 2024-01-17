using Game.DamageSystem;
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
		public State ActiveState { get; private set; }

		private State _startState;
		private readonly Dictionary<State, List<(Func<bool> condition, State next)>> _states = new();

		public PlayerManager PlayerManager { get; private set; }

		public float HP => Health.Current;
		public float MaxHP => Health.Max;
		public bool IsDead => Health.IsDead;

		public Movement Movement { get; private set; }
		public Health Health { get; private set; }
		public Collider2D Collider { get; private set; }
		public Bounds Bounds => Collider.bounds;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Health = GetComponent<Health>();
			Collider = GetComponent<Collider2D>();
		}

		protected virtual void Start()
		{
			PlayerManager = PlayerManager.Instance;
			TransitionTo(_startState);
		}

		protected virtual void Update()
		{
			if (ActiveState != null)
			{
				ActiveState.OnUpdate(this);

				var connections = _states[ActiveState];
				foreach (var (condition, next) in connections)
				{
					if (condition())
					{
						TransitionTo(next);
						break;
					}
				}
			}
		}

		protected void Connect(State origin, State next, Func<bool> condition)
		{
			if (_states.TryGetValue(origin, out var connections))
			{
				connections.Add((condition, next));
				return;
			}

			_states.Add(origin, new() { (condition, next) });
		}

		protected void SetStartingState(State state)
		{
			_startState = state;
			_states.Add(state, new());
		}

		private void TransitionTo(State state)
		{
#if UNITY_EDITOR
			if (state == null)
				Debug.LogError("Enemy brain has been given a null state to transition to!");
			#endif

			ActiveState?.OnFinish(this);
			ActiveState = state;
			ActiveState.OnStart(this);
		}
	}
}
