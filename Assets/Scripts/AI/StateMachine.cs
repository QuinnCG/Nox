using System;

namespace Game.AI
{
	public class StateMachine
	{
		public State Active { get; private set; }

		public void Update()
		{
			Active.Callback();
		}

		public void TransitionTo(State state)
		{
			if (state != Active)
			{
				Active = state;
			}
		}

		public State CreateState(Action callback, string name = "No Name")
		{
			return new State(callback, name);
		}
	}
}
