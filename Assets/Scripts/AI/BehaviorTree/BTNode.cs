using System;
using System.Collections.Generic;

namespace Game.AI.BehaviorTree
{
	public abstract class BTNode
	{
		public BTComposite Parent { get; private set; }
		public BTTree Tree => Parent != null ? Parent.Tree : _tree;

		protected EnemyBrain Agent => Tree.Agent;

		private readonly List<BTConditional> _conditionals = new();

		private bool _started;
		protected BTTree _tree;

		public BTStatus Update()
		{
			if (!_started)
			{
				_started = true;
				OnStart();
			}

			BTStatus status = OnUpdate();
			Tree.SetActiveNode(this);

			if (status is BTStatus.Success or BTStatus.Failure)
			{
				_started = false;
				OnFinish();
			}

			return status;
		}

		public void Interrupt()
		{
			OnFinish(true);
		}

		public bool Evaluate()
		{
			foreach (var conditional in _conditionals)
			{
				if (!conditional.Evaluate())
				{
					return false;
				}
			}

			return true;
		}

		public void Condition(params BTConditional[] conditionals)
		{
			_conditionals.AddRange(conditionals);
		}

		public void SetParent(BTComposite parent)
		{
			Parent = parent;
		}

		public void SetTree(BTTree tree)
		{
			_tree = tree;
		}

		public virtual BTNode[] GetChildren() => Array.Empty<BTNode>();

		protected virtual void OnStart() { }
		protected virtual BTStatus OnUpdate() => BTStatus.Success;
		protected virtual void OnFinish(bool isInterruption = false) { }
	}
}
