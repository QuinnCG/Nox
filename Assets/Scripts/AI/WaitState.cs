namespace Game.AI
{
	public class WaitState : State
	{
		public bool IsFinished { get; private set; }

		private readonly float _duration;

		public WaitState(float duration)
		{
			_duration = duration;
		}

		public override void OnStart(EnemyBrain agent)
		{
			IsFinished = false;

			var timer = new Timer(_duration);
			timer.OnFinish += () => IsFinished = true;
		}
	}
}
