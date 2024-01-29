using System;

namespace Game.AI
{
	public class State
	{
		public string Name { get; private set; }
		public Action Callback { get; private set; }

		public State(Action callback, string name)
		{
			Name = name;
			Callback = callback;
		}
	}
}
