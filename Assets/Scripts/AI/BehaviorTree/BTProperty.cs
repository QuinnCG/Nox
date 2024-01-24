using System;

namespace Game.AI.BehaviorTree
{
	public class BTProperty<T> : IBTProperty where T: new()
	{
		public event Action<BlackBoardValueChangeInfo<T>> OnValueChange;

		public T Value
		{
			get => _value;
			set
			{
				OnValueChange?.Invoke(new()
				{
					OldValue = _value,
					NewValue = value
				});

				_value = value;
			}
		}

		object IBTProperty.Value
		{
			get => Value;
			set => Value = (T)value;
		}

		private T _value;

		public BTProperty()
		{
			_value = new();
		}
		public BTProperty(T defaultValue)
		{
			_value = defaultValue;
		}

		public static implicit operator BTProperty<T>(T value) => new(value);
	}
}
