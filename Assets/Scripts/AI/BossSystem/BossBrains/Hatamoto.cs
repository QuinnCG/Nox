using UnityEngine;
using Game.AI.BehaviorTree.Composites;
using Game.AI.BehaviorTree.Conditionals;
using Sirenix.OdinInspector;
using Game.AI.BehaviorTree.Tasks;
using Game.AI.BehaviorTree;
using Game.AI.BehaviorTree.Decorators;
using Game.ProjectileSystem;

namespace Game.AI.BossSystem.BossBrains
{
	public class Hatamoto : BossBrain
	{
		[SerializeField, MinValue(0f), MaxValue(1f), Tooltip("0f = 0% and 1f = 100%")]
		private float SecondPhaseHP = 0.5f;

		[SerializeField, BoxGroup("Phase 1")]
		private float IdealFleeDistance = 5f;

		[SerializeField, Required]
		private GameObject ShurikenPrefab;

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
			AddNode(firstPhase, secondPhase);

			var isSecondPhase = new IsEqual<int>(Phase, 2);
			secondPhase.AddCondition(isSecondPhase);

			// Phase 1.
			var fleeFrom = new FleeFrom(PlayerPos);
			firstPhase.Add(fleeFrom);
			var nearPlayer = new IsNearTarget(PlayerPos, IdealFleeDistance);
			fleeFrom.AddCondition(nearPlayer);

			var shootSeq = new Sequence();
			firstPhase.Add(shootSeq);
			var shoot = new ShootAtTarget(ShurikenPrefab, transform, PlayerPos, new ShootSpawnInfo()
			{
				Count = 3,
				Method = ShootMethod.EvenSpread,
				SpreadAngle = 45f
			});
			var wait = new Wait(2f);
			shootSeq.Add(shoot, wait);

			// Phase 2.
			// TODO:
		}

		private void OnDamaged(float damage)
		{
			if (Health.Current <= Health.Max * SecondPhaseHP)
			{
				Phase.Value = 2;
			}
		}
	}
}
