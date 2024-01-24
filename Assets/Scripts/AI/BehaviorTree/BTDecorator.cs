namespace Game.AI.BehaviorTree
{
	public abstract class BTDecorator
	{
		public BTTree Tree { get; set; }

		protected EnemyBrain Agent => Tree.Agent;

		public void Start()
		{
			OnStart();
		}

		public void Update()
		{
			OnUpdate();
		}

		public void Finish()
		{
			OnFinish();
		}

		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnFinish() { }
	}
}
