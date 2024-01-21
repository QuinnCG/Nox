using Game.AI.BehaviorTree.Composites;
using Game.AI.BehaviorTree.Conditionals;

namespace Game.AI.BossSystem.BossBrains
{
	public class Hatamoto : BossBrain
	{
		protected override void Start()
		{
			base.Awake();

			// Phases.
			var firstPhase = new Sequence();
			var secondPhase = new Sequence();

			var secondPhaseCondition = new CustomCondition(() => Phase == 2);
			secondPhase.Condition(secondPhaseCondition);

			AddNode(firstPhase, secondPhase);

			// Phase 1.
			// TODO:

			// Phase 2.
			// TODO:
		}
	}
}
