namespace Game.AI.States
{
	public class SubSystemState : State
	{
		public override string Name => _name ?? base.Name;

		private readonly StateMachine _stateMachine;
		private readonly string _name;

		public SubSystemState(StateMachine subSystem, string name = null)
		{
			_stateMachine = subSystem;
			_name = name;
		}

		public override void OnStart(EnemyBrain agent)
		{
			_stateMachine.Start();
		}

		public override void OnUpdate(EnemyBrain agent)
		{
			_stateMachine.Update();
		}

		public override void OnFinish(EnemyBrain agent)
		{
			_stateMachine.Finish();
		}
	}
}
