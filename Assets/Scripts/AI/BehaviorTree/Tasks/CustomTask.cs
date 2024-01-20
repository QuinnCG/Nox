using System;

namespace Game.AI.BehaviorTree.Tasks
{
	public class CustomTask : BTTask
	{
		public Action StartCallback;
		public Func<BTStatus> UpdateCallback;
		public Action<bool> FinishCallback;

		public CustomTask(Func<BTStatus> updateCallback)
		{
			UpdateCallback = updateCallback;
		}

		protected override void OnStart()
		{
			StartCallback?.Invoke();
		}

		protected override BTStatus OnUpdate()
		{
			return UpdateCallback.Invoke();
		}

		protected override void OnFinish(bool isInterruption = false)
		{
			FinishCallback?.Invoke(isInterruption);
		}
	}
}
