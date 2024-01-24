using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI.BehaviorTree
{
	public abstract class BTNode
	{
		public BTComposite Parent { get; private set; }
		public BTTree Tree {get; private set; }

		protected EnemyBrain Agent { get; private set; }

		private readonly List<BTConditional> _conditionals = new();
		private readonly List<BTDecorator> _decorators = new();

		private bool _started;
		protected BTTree _tree;

		public BTStatus Update()
		{
			if (!_started)
			{
				foreach (var decorator in _decorators)
				{
					decorator.Start();
				}

				_started = true;
				OnStart();

				if (this is BTTask task)
				{
					Tree?.SetActiveTask(task);
				}
			}

			foreach (var decorator in _decorators)
			{
				decorator.Update();
			}

			BTStatus status = OnUpdate();
			Tree?.SetActiveNode(this);

			if (status is BTStatus.Success or BTStatus.Failure)
			{
				foreach (var decorator in _decorators)
				{
					decorator.Finish();
				}

				_started = false;
				OnFinish();
			}

			return status;
		}

		public void Interrupt()
		{
			OnFinish(true);

			foreach (var decorator in _decorators)
			{
				decorator.Finish();
			}
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

		public void AddCondition(params BTConditional[] conditionals)
		{
			_conditionals.AddRange(conditionals);
		}

		public void AddDecorator(params BTDecorator[] decorators)
		{
			_decorators.AddRange(decorators);
		}

		public void SetParent(BTComposite parent)
		{
			Parent = parent;
		}

		public virtual void SetTree(BTTree tree)
		{
			_tree = tree;
			Agent = tree.Agent;

			foreach (var decorator in _decorators)
			{
				decorator.Tree = tree;
			}

			foreach (var conditional in _conditionals)
			{
				conditional.Tree = tree;
			}
		}

		public virtual BTNode[] GetChildren() => Array.Empty<BTNode>();

		protected virtual void OnStart() { }
		protected virtual BTStatus OnUpdate() => BTStatus.Success;
		protected virtual void OnFinish(bool isInterruption = false) { }
	}
}
