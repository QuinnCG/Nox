namespace Game.AI.BehaviorTree.Tasks
{
	public class Wait : BTTask
	{
		private readonly Timer _timer;

		public Wait(float duration)
		{
			_timer = new Timer(duration);
		}

		protected override void OnStart()
		{
			_timer.Reset();
		}

		protected override BTStatus OnUpdate()
		{
			return _timer.IsComplete ? BTStatus.Success : BTStatus.Running;
		}
	}
}
