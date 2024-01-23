namespace Game.AI.BehaviorTree
{
	public abstract class BTConditional
	{
		public BTTree Tree { get; set; }

		protected EnemyBrain Agent => Tree.Agent;

		public bool Evaluate() => OnEvaluate();

		protected virtual void OnStart() { }
		protected virtual bool OnEvaluate() => true;
	}
}
