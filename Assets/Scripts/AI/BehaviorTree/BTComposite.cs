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

		public sealed override BTNode[] GetChildren()
		{
			return Children.ToArray();
		}

		public override void SetTree(BTTree tree)
		{
			base.SetTree(tree);

			foreach (var child in Children)
			{
				child.SetTree(tree);
			}
		}
	}
}
