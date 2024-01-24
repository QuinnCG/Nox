using UnityEngine;

namespace Game.AI.BehaviorTree.Conditionals
{
	public class IsNearTarget : BTConditional
	{
		private readonly BTProperty<Vector2> _target;
		private readonly float _distance;
		private readonly bool _invert;

		/// <param name="target">The target is test distance from.</param>
		/// <param name="invert">If true, will instead test if agent is not near the target.</param>
		public IsNearTarget(BTProperty<Vector2> target, float distance, bool invert = false)
		{
			_target = target;
			_distance = distance;
			_invert = invert;
		}

		protected override bool OnEvaluate()
		{
			float dst = Vector2.Distance(Agent.Position.Value, _target.Value);
			if (_invert)
			{
				return dst > _distance;
			}

			return dst <= _distance;
		}
	}
}
