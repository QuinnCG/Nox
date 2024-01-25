using UnityEngine;

namespace Game.AI.BehaviorTree.Decorators
{
	public class GetDistanceBetween : BTDecorator
	{
		private readonly BTProperty<Vector2> _a;
		private readonly BTProperty<Vector2> _b;
		private readonly BTProperty<float> _result;

		public GetDistanceBetween(BTProperty<Vector2> a, BTProperty<Vector2> b, BTProperty<float> result)
		{
			_a = a;
			_b = b;
			_result = result;
		}

		protected override void OnUpdate()
		{
			_result.Value = Vector2.Distance(_a.Value, _b.Value);
		}
	}
}
