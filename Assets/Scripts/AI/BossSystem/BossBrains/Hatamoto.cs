using UnityEngine;
using Game.AI.BehaviorTree.Composites;
using Game.AI.BehaviorTree.Conditionals;
using Sirenix.OdinInspector;
using Game.AI.BehaviorTree.Tasks;

namespace Game.AI.BossSystem.BossBrains
{
	public class Hatamoto : BossBrain
	{
		[SerializeField, MinValue(0f), MaxValue(1f)]
		private float SecondPhaseHP = 0.5f;

		[SerializeField, BoxGroup("Phase 1")]
		private float IdealFleeDistance = 3f;

		protected override void Awake()
		{
			base.Awake();

			Health.OnDamaged += OnDamaged;
		}

		protected override void Start()
		{
			base.Awake();

			// Phases.
			var firstPhase = new Sequence();
			var secondPhase = new Sequence();

			var secondPhaseCondition = new CustomCondition(() => Phase == 2);
			secondPhase.AddCondition(secondPhaseCondition);

			AddNode(firstPhase, secondPhase);

			// Phase 1.
			var nearPlayer = new IsNearTarget(() => PlayerPos, IdealFleeDistance);
			var fleeFrom = new FleeFrom(() => PlayerPos);
			fleeFrom.AddCondition(nearPlayer);
			firstPhase.Add(fleeFrom);

			// Phase 2.
			// TODO:
		}

		private void OnDamaged(float damage)
		{
			if (Health.Current <= Health.Max * SecondPhaseHP)
			{
				Phase = 2;
			}
		}
	}
}
