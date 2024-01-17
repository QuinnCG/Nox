using UnityEngine;

namespace Game.AI.States
{
	public class WanderState : State
	{
		private readonly float _radius;
		private readonly float _waitDuration;
		private readonly float _waitDeviation;
		private readonly float _stoppingDistance;

		private Vector2 _origin;
		private Vector2? _target;
		private float _nextMoveTime;

		public WanderState(float radius, float waitDuration, float waitDeviation = 0.5f, float stoppingDistance = 0.2f)
		{
			_radius = radius;
			_waitDuration = waitDuration;
			_waitDeviation = waitDeviation;
			_stoppingDistance = stoppingDistance;
		}

		public override void OnStart(EnemyBrain enemy)
		{
			_origin = enemy.transform.position;
			_target = null;
		}

		public override void OnUpdate(EnemyBrain enemy)
		{
			if (_target.HasValue)
			{
				if (Vector2.Distance(enemy.transform.position, _target.Value) < _stoppingDistance)
				{
					_target = null;

					float halfDeviation = _waitDeviation / 2f;
					float duration = _waitDuration + Random.Range(-halfDeviation, halfDeviation);
					duration = Mathf.Max(0f, duration);

					_nextMoveTime = Time.time + duration;
				}
				else if (Time.time > _nextMoveTime)
				{
					// No need to normalize here because Movement.Move() does that for you.
					Vector2 dir = _target.Value - (Vector2)enemy.transform.position;
					enemy.Movement.Move(dir);
				}
			}
			else
			{
				_target = _origin + (Random.insideUnitCircle * _radius);
			}
		}
	}
}
