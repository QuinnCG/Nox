﻿using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class MoveTo : BTTask
	{
		private readonly BTProperty<Vector2> _target;
		private readonly float _stoppingDistance;

		public MoveTo(BTProperty<Vector2> target, float stoppingDistance = 0.2f)
		{
			_target = target;
			_stoppingDistance = stoppingDistance;
		}

		protected override BTStatus OnUpdate()
		{
			Vector2 origin = Agent.transform.position;
			Vector2 target = _target.Value;

			float dst = Vector2.Distance(origin, target);
			if (dst <= _stoppingDistance)
			{
				return BTStatus.Success;
			}

			Vector2 dir = target - origin;
			Agent.Movement.Move(dir);

			return BTStatus.Running;
		}
	}
}
