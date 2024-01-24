using UnityEngine;

namespace Game.AI.BehaviorTree.Decorators
{
	public class GetDistanceFromTarget : BTDecorator
	{
		private readonly BTProperty<Vector2> _target;
		private readonly BTProperty<float> _distance;

		public GetDistanceFromTarget(BTProperty<Vector2> target, BTProperty<float> distance)
		{
			_target = target;
			_distance = distance;
		}

		protected override void OnUpdate()
		{
			_distance.Value = Vector2.Distance(Agent.transform.position, _target.Value);
		}
	}
}
