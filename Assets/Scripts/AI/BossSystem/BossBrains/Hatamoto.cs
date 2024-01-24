using UnityEngine;
using Game.AI.BehaviorTree.Composites;
using Game.AI.BehaviorTree.Conditionals;
using Sirenix.OdinInspector;
using Game.AI.BehaviorTree.Tasks;
using Game.AI.BehaviorTree;
using Game.AI.BehaviorTree.Decorators;

namespace Game.AI.BossSystem.BossBrains
{
	public class Hatamoto : BossBrain
	{
		[SerializeField, MinValue(0f), MaxValue(1f)]
		private float SecondPhaseHP = 0.5f;

		[SerializeField, BoxGroup("Phase 1")]
		private float IdealFleeDistance = 3f;

		[Expose]
		public BTProperty<float> DistanceToPlayer = new();

		protected override void Awake()
		{
			base.Awake();
			Health.OnDamaged += OnDamaged;
		}

		protected override void Start()
		{
			base.Start();

			// Phases.
			var firstPhase = new Sequence();
			var secondPhase = new Sequence();

			var secondPhaseCondition = new IsEqual<int>(Phase, 2);
			secondPhase.AddCondition(secondPhaseCondition);

			AddNode(firstPhase, secondPhase);

			// Phase 1.
			var nearPlayer = new IsNearTarget(PlayerPos, IdealFleeDistance);
			var fleeFrom = new FleeFrom(PlayerPos);
			var getDst = new GetDistanceFromTarget(PlayerPos, DistanceToPlayer);
			fleeFrom.AddCondition(nearPlayer);
			fleeFrom.AddDecorator(getDst);
			firstPhase.Add(fleeFrom);

			// Phase 2.
			// TODO:
		}

		// TODO: Distance to player keeps growing.

		private void OnDamaged(float damage)
		{
			if (Health.Current <= Health.Max * SecondPhaseHP)
			{
				Phase = 2;
			}
		}
	}
}
