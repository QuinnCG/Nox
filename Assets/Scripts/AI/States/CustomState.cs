using System;

namespace Game.AI.States
{
	public class CustomState : State
	{
		public override string Name => _name != null ? _name : base.Name;

		private readonly Action<EnemyBrain> OnStartEvent;
		private readonly Action<EnemyBrain> OnUpdateEvent;
		private readonly Action<EnemyBrain> OnFinishEvent;

		private readonly string _name;

		public CustomState(Action<EnemyBrain> start = null, Action<EnemyBrain> update = null, Action<EnemyBrain> finish = null, string name = null)
		{
			OnStartEvent = start;
			OnUpdateEvent = update;
			OnFinishEvent = finish;

			_name = name;
		}

		public override void OnStart(EnemyBrain agent)
		{
			OnStartEvent?.Invoke(agent);
		}

		public override void OnUpdate(EnemyBrain agent)
		{
			OnUpdateEvent?.Invoke(agent);
		}

		public override void OnFinish(EnemyBrain agent)
		{
			OnFinishEvent?.Invoke(agent);
		}
	}
}
