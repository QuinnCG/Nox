namespace Game.AI.BehaviorTree.Conditionals
{
	public class IsNull : BTConditional
	{
		private readonly object _value;
		private readonly bool _invert;

		public IsNull(object value, bool invert = false)
		{
			_value = value;
			_invert = invert;
		}

		protected override bool OnEvaluate()
		{
			return _invert ? (_value == null) : (_value != null);
		}
	}
}
