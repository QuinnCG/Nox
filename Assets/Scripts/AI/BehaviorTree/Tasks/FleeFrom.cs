using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class FleeFrom : BTTask
	{
		private readonly BTProperty<Vector2> _target;

		/// <param name="target">The current point in space to flee from.</param>
		/// <param name="desiredDistance">The distance from the target this agent is trying to reach.</param>
		public FleeFrom(BTProperty<Vector2> target)
		{
			_target = target;
		}

		protected override BTStatus OnUpdate()
		{
			var pointToAvoid = _target.Value;
			var dir = Agent.Position.Value - pointToAvoid;
			dir.Normalize();

			Agent.Movement.Move(dir);
			return BTStatus.Running;
		}
	}
}
