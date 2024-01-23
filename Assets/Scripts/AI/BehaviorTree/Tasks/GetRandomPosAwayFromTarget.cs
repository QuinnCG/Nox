using System;
using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class GetRandomPosAwayFromTarget : BTTask
	{
		private const float ObstacleBuffer = 0.5f;

		private readonly Action<Vector2> _setTarget;
		private readonly Func<Vector2> _getOrigin;

		private readonly float _idealDistance;
		private readonly float _maxAngle;

		public GetRandomPosAwayFromTarget(Action<Vector2> setTarget, Func<Vector2> getOrigin, float idealDst, float maxAngle = 90f)
		{
			_setTarget = setTarget;
			_getOrigin = getOrigin;

			_idealDistance = idealDst;
			_maxAngle = maxAngle;
		}

		protected override BTStatus OnUpdate()
		{
			Vector2 start = _getOrigin();
			Vector2 end = Agent.transform.position;

			Vector2 dir = GetDirection(end - start);
			Vector2 pos = GetActualDistance(dir);

			_setTarget?.Invoke(pos);
			return BTStatus.Success;
		}

		// Calculates a random direction between '_maxAngle' and from 'startDir'.
		private Vector2 GetDirection(Vector2 startDir)
		{
			startDir.Normalize();
			float angle = Mathf.Atan2(startDir.y, startDir.x);

			float halfMax = _maxAngle / 2f;
			float delta = UnityEngine.Random.Range(-halfMax, halfMax);

			angle += delta * Mathf.Deg2Rad;
			var newDir = new Vector2()
			{
				x = Mathf.Cos(angle),
				y = Mathf.Sin(angle),
			};

			return newDir;
		}

		private Vector2 GetActualDistance(Vector2 dir)
		{
			Vector2 start = Agent.transform.position;
			Vector2 end = start + (dir * (_idealDistance - ObstacleBuffer));
			int mask = LayerMask.GetMask("Obstacle");

			var hit = Physics2D.Linecast(start, end, mask);

			// Has line-of-sight.
			if (hit.collider == null)
			{
				return end;
			}

			// Does not have line-of-sight.
			return hit.point;

		}
	}
}
