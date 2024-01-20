using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
	public class StateMachine
	{
		public State ActiveState { get; private set; }
		public bool ShowDebug { get; set; }

		private readonly Dictionary<State, List<(Func<bool> condition, State next)>> _states = new();
		private readonly Dictionary<State, StateMachine> _subSystems = new();

		private State _startState;
		private readonly EnemyBrain _agent;

		public StateMachine(EnemyBrain agent)
		{
			_agent = agent;
		}

		public void Start()
		{
			TransitionTo(_startState);
		}

		public void Update()
		{
			if (ActiveState != null)
			{
				ActiveState.OnUpdate(_agent);
				if (_subSystems.TryGetValue(ActiveState, out var stateMachine))
				{
					stateMachine.Update();
				}

#if UNITY_EDITOR
				if (ShowDebug)
				{
					Debug.Log(
						$"State Machine ({_agent.gameObject.name}): ".Bold().Color(StringColor.White)
						+ "active: " + ActiveState.Name.Bold().Color(StringColor.White));
				}
#endif

				if (_states.TryGetValue(ActiveState, out var conditions))
				{
					foreach (var (condition, next) in conditions)
					{
						if (condition())
						{
							TransitionTo(next);
							break;
						}
					}
				}
			}
		}

		public void Finish()
		{
			ActiveState.OnFinish(_agent);
		}

		public void SetStartingState(State state)
		{
			_startState = state;
			_states.Add(state, new());
		}

		public void Connect(State origin, State next, Func<bool> condition)
		{
			if (_states.TryGetValue(origin, out var connections))
			{
				connections.Add((condition, next));
				return;
			}

			_states.Add(origin, new() { (condition, next) });
		}

		public void TransitionTo(State state)
		{
			StateMachine stateMachine;

			if (ActiveState != null)
			{
				ActiveState.OnFinish(_agent);
				if (_subSystems.TryGetValue(ActiveState, out stateMachine))
				{
					stateMachine.Finish();
				}
			}

			ActiveState = state;
			ActiveState.OnStart(_agent);
			if (_subSystems.TryGetValue(ActiveState, out stateMachine))
			{
				stateMachine.Start();
			}
		}

		public void Attach(State parent, StateMachine child)
		{
			_subSystems.Add(parent, child);
		}
	}

	// TODO: Behavior Tree?
}
