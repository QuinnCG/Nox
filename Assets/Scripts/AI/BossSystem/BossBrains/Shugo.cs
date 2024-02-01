using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AI.BossSystem.BossBrains
{
	public class Shugo : BossBrain
	{
		[SerializeField, Required, BoxGroup("References")]
		private GameObject FirePrefab;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject MinionPrefab;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject ShadowPrefab;

		private State _idle, _sakeChug, _jump, _callReinforcements;
		private Timer _abilityCooldown;
		private bool isJumping;

		protected override void Start()
		{
			base.Start();

			_idle = CreateState(OnIdle, "Idle");
			_sakeChug = CreateState(OnSakeChug, "Sake Chug");
			_jump = CreateState(OnJump, "Jump");
			_callReinforcements = CreateState(OnCallReinforcements, "Call Reinforcements");

			_abilityCooldown = new Timer(5f);

			TransitionTo(_idle);
		}

		private void OnIdle()
		{
			if (_abilityCooldown.IsDone)
			{
				float randomValue = UnityEngine.Random.value;
				if (randomValue < 0.33f)
				{
					TransitionTo(_sakeChug);
				}
				else if (randomValue < 0.66f)
				{
					TransitionTo(_jump);
				}
				else
				{
					TransitionTo(_callReinforcements);
				}

				_abilityCooldown.Reset();
			}
		}

		private void OnSakeChug()
		{
			ChugSakeAndBlowFire();
			TransitionTo(_idle);
		}

		private void OnJump()
		{
			if (!isJumping)
			{
				isJumping = true;
				JumpAttack();
			}
		}

		private void JumpAttack()
		{
			// Instantiate the shadow at Shugo's current position
			GameObject shadow = Instantiate(ShadowPrefab, transform.position, Quaternion.identity);

			// Uses the shadow GameObject to track the player's movement
			ShadowController shadowController = shadow.GetComponent<ShadowController>();
			if (shadowController != null)
			{
				shadowController.SetTarget(PlayerPosition);
			}

			// Use animations or other mechanisms for the jump attack
			// After the jump animation is complete, call JumpCompleted()
			// Example: StartCoroutine(PerformJumpAnimation());
		}

		private void JumpCompleted()
		{
			// Perform logic when the jump attack is completed
			isJumping = false;
			Destroy(GameObject.FindGameObjectWithTag("Shadow")); // Assuming the tag is set appropriately
			TransitionTo(_idle);
		}

		private void OnCallReinforcements()
		{
			CallReinforcements();
			TransitionTo(_idle);
		}

		private void ChugSakeAndBlowFire()
		{
			Instantiate(FirePrefab, transform.position, Quaternion.identity);
		}

		private void CallReinforcements()
		{
			Instantiate(MinionPrefab, transform.position, Quaternion.identity);
		}
	}
}


