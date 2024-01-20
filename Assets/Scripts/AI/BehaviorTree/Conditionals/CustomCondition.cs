using System;

namespace Game.AI.BehaviorTree.Conditionals
{
	public class CustomCondition : BTConditional
	{
		private readonly Func<bool> _condition;

		public CustomCondition(Func<bool> condition)
		{
			_condition = condition;
		}

		protected override bool OnEvaluate()
		{
			return _condition();
		}
	}
}
