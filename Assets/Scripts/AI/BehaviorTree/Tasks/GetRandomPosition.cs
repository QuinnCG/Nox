using System;
using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class GetRandomPosition : BTTask
	{
		private enum Type
		{
			Points,
			LocalRadius,
			GlobalRadius
		}

		private readonly Action<Vector2> _callback;

		private readonly Type _type;
		private readonly Transform[] _points;
		private readonly Vector2 _origin;
		private readonly float _radius;

		public GetRandomPosition(Action<Vector2> result, params Transform[] points)
		{
			_callback = result;
			_type = Type.Points;
			_points = points;
		}
		public GetRandomPosition(Action<Vector2> result, float radius)
		{
			_callback = result;

			_type = Type.LocalRadius;
			_radius = radius;
		}
		public GetRandomPosition(Action<Vector2> result, Vector2 origin, float radius)
		{
			_callback = result;

			_type = Type.GlobalRadius;
			_origin = origin;
			_radius = radius;
		}

		protected override BTStatus OnUpdate()
		{
			switch (_type)
			{
				case Type.Points:
					_callback(_points[UnityEngine.Random.Range(0, _points.Length - 1)].position);
					break;

				case Type.LocalRadius:
					_callback((_radius * 2f * UnityEngine.Random.insideUnitCircle) + (Vector2)Agent.transform.position);
					break;

				case Type.GlobalRadius:
					_callback((_radius * 2f * UnityEngine.Random.insideUnitCircle) + _origin);
					break;
			}

			return BTStatus.Success;
		}
	}
}
