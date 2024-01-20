using System.Collections.Generic;

namespace Game.AI.BehaviorTree
{
	public class BTComposite : BTNode
	{
		protected List<BTNode> Children { get; } = new();

		public void Add(params BTNode[] children)
		{
			foreach (var child in children)
			{
				child.SetParent(this);
				Children.Add(child);
			}
		}
	}
}
