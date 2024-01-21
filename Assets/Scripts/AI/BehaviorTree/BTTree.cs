using Game.AI.BehaviorTree.Composites;
using UnityEngine;

namespace Game.AI.BehaviorTree
{
	public class BTTree
	{
		public bool EnableDebug { get; set; }

		public EnemyBrain Agent { get; private set; }

		private readonly Selector _root = new();
		private BTNode _activeNode;

		public BTTree(EnemyBrain agent)
		{
			_root.SetTree(this);
			Agent = agent;
		}

		public void Update()
		{
			_root.Update();

            if (EnableDebug)
			{
				Debug.Log(
					"Behavior Tree: "
					+ "[Active] ".Color(StringColor.Yellow)
					+ _activeNode?.GetType().Name.Bold().Color(StringColor.White));
			}
		}

		public void Add(params BTNode[] children)
		{
			_root.Add(children);
		}

		public void SetActiveNode(BTNode node)
		{
			_activeNode = node;
		}
	}
}
