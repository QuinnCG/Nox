namespace Game.AI.BehaviorTree
{
	public abstract class BTConditional
	{
		public bool Evaluate() => OnEvaluate();

		protected virtual void OnStart() { }
		protected virtual bool OnEvaluate() => true;
	}
}
