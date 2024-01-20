using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class Log : BTTask
	{
		private readonly string _message;
		private readonly bool _continuous;

		public Log(string message, bool continuous = false)
		{
			_message = message;
			_continuous = continuous;
		}

		protected override BTStatus OnUpdate()
		{
			Debug.Log(_message);
			
			if (_continuous)
				return BTStatus.Running;
			else
				return BTStatus.Success;
		}
	}
}
