using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class RandomTrigger : BTTask
	{
		private readonly float _chance;
		private readonly BTProperty<bool> _trigger;

		public RandomTrigger(float chance, BTProperty<bool> trigger)
		{
			_chance = chance;
			_trigger = trigger;
		}

		protected override BTStatus OnUpdate()
		{
			if (Random.value <= _chance)
			{
				_trigger.Value = true;
			}

			return BTStatus.Success;
		}
	}
}
