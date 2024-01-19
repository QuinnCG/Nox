namespace Game.AI.States
{
	public class SubSystemState : State
	{
		private readonly StateMachine _stateMachine;

		public SubSystemState(StateMachine subSystem)
		{
			_stateMachine = subSystem;
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
