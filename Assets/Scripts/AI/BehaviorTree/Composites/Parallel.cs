namespace Game.AI.BehaviorTree.Composites
{
	public class Parallel : BTComposite
	{
		protected override BTStatus OnUpdate()
		{
			bool running = false;

			foreach (var child in Children)
			{
				BTStatus status = child.Update();
				if (status == BTStatus.Failure)
				{
					return BTStatus.Failure;
				}
				else if (status == BTStatus.Running)
				{
					running = true;
				}
			}

			return running ? BTStatus.Running : BTStatus.Success;
		}
	}
}
