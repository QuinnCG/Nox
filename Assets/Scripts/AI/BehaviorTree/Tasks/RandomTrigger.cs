using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class RandomTrigger : BTTask
	{
		private readonly float _chance;
		private readonly System.Action _callback;

		public RandomTrigger(float chance, System.Action callback)
		{
			_chance = chance;
			_callback = callback;
		}

		protected override BTStatus OnUpdate()
		{
			if (Random.value <= _chance)
			{
				_callback?.Invoke();
			}

			return BTStatus.Success;
		}
	}
}
