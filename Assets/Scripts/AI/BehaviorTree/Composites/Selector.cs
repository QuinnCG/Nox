using UnityEngine;

namespace Game.AI.BehaviorTree.Composites
{
	public class Selector : Sequence
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
			for (int i = 0; i == _index; i++)
			{
				BTNode child = Children[i];

				if (child.Evaluate())
				{
					if (i != _index)
					{
						Children[_index].Interrupt();
					}

					_index = i;
					BTStatus status = child.Update();

					if (status == BTStatus.Success)
					{
						return BTStatus.Success;
					}
					else if (status == BTStatus.Failure)
					{
						_index++;
						if (_index > _last)
						{
							return BTStatus.Failure;
						}
					}
				}
				else
				{
					_index++;
					if (_index > _last)
					{
						return BTStatus.Failure;
					}
				}
			}

			return BTStatus.Running;
		}
	}
}
