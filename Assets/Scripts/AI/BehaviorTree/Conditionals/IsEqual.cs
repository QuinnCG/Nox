namespace Game.AI.BehaviorTree.Conditionals
{
	public class IsEqual<T> : BTConditional where T : new()
	{
		private readonly BTProperty<T> _property;
		private readonly T _value;

		public IsEqual(BTProperty<T> property, T value)
		{
			_property = property;
			_value = value;
		}

		protected override bool OnEvaluate()
		{
			return _property.Value.Equals(_value);
		}
	}
}
