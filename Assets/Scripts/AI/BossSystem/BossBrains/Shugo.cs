using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

namespace Game.AI.BossSystem.BossBrains
{
	public class Shugo : BossBrain
	{
		[SerializeField, Required, BoxGroup("Phase 1")]
		private float JumpHeight = 5f;

		[SerializeField, Required, BoxGroup("Phase 1")]
		private float JumpDuration = 2f;

		[SerializeField, Required, BoxGroup("Phase 1")]
		private float SuperJumpHeight = 5f;

		[SerializeField, Required, BoxGroup("Phase 1")]
		private float SuperJumpDuration = 2f;

		[SerializeField, Required, BoxGroup("Phase 2")]
		private float JumpDurationFactor = 0.7f;

		[SerializeField, Required, BoxGroup("Phase 2")]
		private float SuperJumpDurationFactor = 0.7f;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject FirePrefab;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject MinionPrefab;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject ShadowPrefab;

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] ReinforcementPoints;

		private State _wander, _superJump, _fireSpew, _summon;
		private Timer _abilityCooldown;

		private Vector2 _jumpStart, _jumpEnd;
		private Transform _shadow;

		protected override void Start()
		{
			base.Start();

			_wander = CreateState(OnWander, "Wander");
			_superJump = CreateState(OnSuperJump, "Super Jump");
			_fireSpew = CreateState(OnFireSpew, "Fire Spew");
			_summon = CreateState(OnSummon, "Summon");

			Idle();
		}

		private void Idle()
		{
			TransitionTo(_wander);
		}

		private void ExecuteRandomSpecial()
		{
			// Random decide on special
		}

		private void OnWander()
		{
			/*
			 * Idle
			 * 
			 * 50/50:
			 * Jump on player
			 * Jump to random pos
			 * 
			 * If special timer is done:
			 * ExecuteRandomSpecial
			 */
		}

		private void OnSuperJump()
		{
			// Jump high up and land near player
			// On land: spawn circle of fireballs
			// Idle
		}

		private void OnFireSpew()
		{
			// Jump to corner of room
			// Shoot shotgun spread of fireballs
			// Idle
		}

		private void OnSummon()
		{
			// Roar!
			// Simultaneously, spawn minions in poof of black smoke
		}
	}
}
