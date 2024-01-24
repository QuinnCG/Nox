namespace Game.AI.BehaviorTree.Conditionals
{
	public class CustomCondition : BTConditional
	{
		private readonly BTProperty<bool> _condition;

		public CustomCondition(BTProperty<bool> condition)
		{
			_condition = condition;
		}

		protected override bool OnEvaluate()
		{
			return _condition.Value;
		}
	}
}
