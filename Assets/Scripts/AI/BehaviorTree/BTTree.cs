using Game.AI.BehaviorTree.Composites;
using System;
using UnityEngine;

namespace Game.AI.BehaviorTree
{
	public class BTTree
	{
		public event Action<BTNode> OnNodeUpdate;
		public event Action<BTTask> OnTaskStart;

		public EnemyBrain Agent { get; private set; }

		public BTTask ActiveTask { get; private set; }
		public BTNode ActiveNode { get; private set; }

		private readonly Selector _root = new();

		public BTTree(EnemyBrain agent)
		{
			Agent = agent;
		}

		public void Start()
		{
			_root.SetTree(this);
		}

		public void Update()
		{
			_root.Update();
		}

		public void Add(params BTNode[] children)
		{
			_root.Add(children);
		}

		public BTNode GetRoot()
		{
			return _root;
		}

		public void SetActiveNode(BTNode node)
		{
			ActiveNode = node;
			OnNodeUpdate?.Invoke(ActiveNode);
		}

		public void SetActiveTask(BTTask task)
		{
			ActiveTask = task;
			OnTaskStart?.Invoke(ActiveTask);
		}
	}
}
