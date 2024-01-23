using System;
using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class FleeFrom : BTTask
	{
		private readonly Func<Vector2> _target;

		/// <param name="target">The current point in space to flee from.</param>
		/// <param name="desiredDistance">The distance from the target this agent is trying to reach.</param>
		public FleeFrom(Func<Vector2> target)
		{
			_target = target;
		}

		protected override BTStatus OnUpdate()
		{
			var pointToAvoid = _target();
			var dir = Agent.Position - pointToAvoid;
			dir.Normalize();

			Agent.Movement.Move(dir);
			return BTStatus.Running;
		}
	}
}
