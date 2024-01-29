using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class FleeFrom : BTTask
	{
		private readonly BTProperty<Vector2> _target;
		private readonly float _distance;

		/// <param name="target">The current point in space to flee from.</param>
		/// <param name="desiredDistance">The distance from the target this agent is trying to reach.</param>
		public FleeFrom(BTProperty<Vector2> target, float distance)
		{
			_target = target;
			_distance = distance;
		}

		protected override BTStatus OnUpdate()
		{
			var pointToAvoid = _target.Value;
			var diff = Agent.Position.Value - pointToAvoid;
			var dir = diff.normalized;

			Agent.Movement.Move(dir);

			if (diff.magnitude >= _distance)
				return BTStatus.Success;
			else
				return BTStatus.Running;
		}
	}
}
