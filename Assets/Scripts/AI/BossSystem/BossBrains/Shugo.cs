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

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] ReinforcementPoints;

		[SerializeField, BoxGroup("Jump Settings")]
		private float JumpHeight = 5f;

		[SerializeField, BoxGroup("Jump Settings")]
		private float JumpDuration = 1f;

		private State _idle, _sakeChug, _jump, _callReinforcements;
		private Timer _abilityCooldown;
		private bool isJumping;

		private Collider2D bossCollider;

		protected override void Start()
		{
			base.Start();

			_idle = CreateState(OnIdle, "Idle");
			_sakeChug = CreateState(OnSakeChug, "Sake Chug");
			_jump = CreateState(OnJump, "Jump");
			_callReinforcements = CreateState(OnCallReinforcements, "Call Reinforcements");

			_abilityCooldown = new Timer(5f);

			bossCollider = GetComponent<Collider2D>();

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

		private void OnCallReinforcements()
		{
			SummonReinforcements();
			TransitionTo(_idle);
		}

		private void SummonReinforcements()
		{
			foreach (Transform spawnPoint in ReinforcementPoints)
			{
				Instantiate(MinionPrefab, spawnPoint.position, Quaternion.identity);
			}
		}

		private void ChugSakeAndBlowFire()
		{
			Instantiate(FirePrefab, transform.position, Quaternion.identity);
		}

		private void JumpAttack()
		{
			bossCollider.enabled = false; // Disable the collider during the jump
			GameObject shadow = Instantiate(ShadowPrefab, transform.position, Quaternion.identity);
			ShadowController shadowController = shadow.GetComponent<ShadowController>();

			if (shadowController != null)
			{
				shadowController.SetTarget(PlayerPosition);
			}

			StartCoroutine(PerformJumpAnimation(shadow));
		}

		private System.Collections.IEnumerator PerformJumpAnimation(GameObject shadow)
		{
			Vector3 startPosition = transform.position;
			Vector3 jumpTarget = shadow.transform.position; // Jump to the shadow's position

			float elapsedTime = 0f;

			while (elapsedTime < JumpDuration)
			{
				transform.position = Vector3.Lerp(startPosition, jumpTarget, elapsedTime / JumpDuration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			transform.position = jumpTarget;

			yield return new WaitForSeconds(1f);

			JumpCompleted();

			elapsedTime = 0f;

			while (elapsedTime < JumpDuration)
			{
				transform.position = Vector3.Lerp(jumpTarget, startPosition, elapsedTime / JumpDuration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			transform.position = startPosition;

			bossCollider.enabled = true; // Re-enable the collider after the jump
			Destroy(shadow);
		}

		private void JumpCompleted()
		{
			isJumping = false;
			TransitionTo(_idle);
		}
	}
}




