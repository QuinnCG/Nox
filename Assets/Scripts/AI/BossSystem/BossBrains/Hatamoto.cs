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
		private float DistanceBeforeFlee = 4f;

		[SerializeField, BoxGroup("Phase 1")]
		private float FleeDistance = 5f;

		[SerializeField, Required, BoxGroup("References")]
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
			var fleeFrom = new FleeFrom(PlayerPos, FleeDistance);
			firstPhase.Add(fleeFrom);
			var nearPlayer = new IsNearTarget(PlayerPos, DistanceBeforeFlee);
			fleeFrom.AddCondition(nearPlayer);

			// As soon as the player is not near, the task self aborts.
			// Either tasks should not abort themselves (or can opt out) or things must update every frame.

			var shootSeq = new Sequence();
			firstPhase.Add(shootSeq);
			var shoot = new ShootAtTarget(ShurikenPrefab, transform, PlayerPos, new ShootSpawnInfo()
			{
				Count = 3,
				Method = ShootMethod.EvenSpread,
				SpreadAngle = 45f
			});
			shootSeq.Add(shoot, new Wait(2f));

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
