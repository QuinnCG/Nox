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
			if (Children.Count == 0)
				return BTStatus.Success;

			BTNode child = Children[_index];

			if (!child.Evaluate())
			{
				return BTStatus.Failure;
			}

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
