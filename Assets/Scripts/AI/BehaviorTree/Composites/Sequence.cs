using Unity.VisualScripting;
using UnityEngine;

namespace Game.AI.BehaviorTree.Composites
{
	public class Sequence : BTComposite
	{
		private int _index;
		private int _last;

		protected override void OnStart()
		{
			_index = 0;
			_last = Children.Count - 1;
		}

		protected override BTStatus OnUpdate()
		{
			BTNode child = Children[_index];
			BTStatus status = child.Update();

			if (status == BTStatus.Success)
			{
				_index++;
				if (_index > _last)
				{
					return BTStatus.Success;
				}
			}
			else if (status == BTStatus.Failure)
			{
				return BTStatus.Failure;
			}

			return BTStatus.Running;
		}
	}
}
