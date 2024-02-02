using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
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
		private Transform[] ReinforcementPoints; // Add this field to store reinforcement spawn points

		[SerializeField, Required, BoxGroup("Jump Settings")]
		private float jumpHeight = 5f;

		[SerializeField, Required, BoxGroup("Jump Settings")]
		private float jumpDuration = 1f;

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
			Collider.enabled = false; // Disable the collider during the jump
			GameObject shadow = Instantiate(ShadowPrefab, transform.position, Quaternion.identity);

			// Pass the player's transform as the target for the shadow
			ShadowController shadowController = shadow.GetComponent<ShadowController>();
			if (shadowController != null)
			{
				if (PlayerPosition != null)
				{
					shadowController.SetTarget(PlayerPosition);
				}
				else
				{
					// Handle the case where PlayerTransform is not available
					Debug.LogError("PlayerTransform not available.");
				}
			}

			StartCoroutine(PerformJumpAnimation(shadow, PlayerPosition != null ? PlayerPosition : (Vector2)transform.position));
		}

		private IEnumerator PerformJumpAnimation(GameObject shadow, Vector3 targetPosition)
		{
			float elapsedTime = 0f;

			while (elapsedTime < jumpDuration)
			{
				float jumpProgress = Mathf.Clamp01(elapsedTime / jumpDuration);
				float jumpHeightOffset = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;

				transform.position = Vector3.Lerp(transform.position, targetPosition, jumpProgress);
				shadow.transform.position = new Vector3(targetPosition.x, targetPosition.y + jumpHeightOffset, targetPosition.z);

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			JumpCompleted(shadow);
		}

		private void JumpCompleted(GameObject shadow)
		{
			// Perform logic when the jump attack is completed
			isJumping = false;
			Destroy(shadow);
			Collider.enabled = true; // Re-enable the collider
			TransitionTo(_idle);
		}
	}
}







