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

		[SerializeField, Required, BoxGroup("Jump Settings")]
		private float jumpHeight = 5f;

		[SerializeField, Required, BoxGroup("Jump Settings")]
		private float jumpDuration = 1f;

		[SerializeField, Required, BoxGroup("Jump Settings")]
		private Transform[] jumpTargets;

		[SerializeField, Required, BoxGroup("Reinforcement Settings")]
		private Transform[] reinforcementSpawnPoints;

		private State _idle, _sakeChug, _lowJumpAndSakeChug, _callReinforcements, _jump;
		private Timer _abilityCooldown;
		private bool isJumping;

		protected override void Start()
		{
			base.Start();

			_idle = CreateState(OnIdle, "Idle");
			_sakeChug = CreateState(OnSakeChug, "Sake Chug");
			_lowJumpAndSakeChug = CreateState(OnLowJumpAndSakeChug, "Low Jump and Sake Chug");
			_callReinforcements = CreateState(OnCallReinforcements, "Call Reinforcements");
			_jump = CreateState(OnJump, "Jump");

			_abilityCooldown = new Timer(5f);

			TransitionTo(_idle);
		}

		private void OnIdle()
		{
			if (_abilityCooldown.IsDone)
			{
				float randomValue = UnityEngine.Random.value;
				if (randomValue < 0.25f)
				{
					TransitionTo(_sakeChug);
				}
				else if (randomValue < 0.5f)
				{
					TransitionTo(_lowJumpAndSakeChug);
				}
				else if (randomValue < 0.75f)
				{
					TransitionTo(_callReinforcements);
				}
				else
				{
					TransitionTo(_jump);
				}

				_abilityCooldown.Reset();
			}
		}

		private void OnSakeChug()
		{
			ChugSakeAndBlowFire();
			TransitionTo(_idle);
		}

		private void OnLowJumpAndSakeChug()
		{
			// Choose a jump target
			Transform jumpTarget = ChooseRandomTarget(jumpTargets);

			// Choose a reinforcement spawn point
			Transform spawnPoint = ChooseRandomTarget(reinforcementSpawnPoints);

			Jump(jumpTarget.position, jumpHeight, jumpDuration).OnComplete(() =>
			{
				// Spawn reinforcements at the chosen spawn point
				Instantiate(MinionPrefab, spawnPoint.position, Quaternion.identity);

				// Perform Sake Chug
				ChugSakeAndBlowFire();

				TransitionTo(_idle);
			});
		}

		private void OnCallReinforcements()
		{
			SummonReinforcements();
			TransitionTo(_idle);
		}

		private void OnJump()
		{
			// Logic for jumping state
		}

		private void SummonReinforcements()
		{
			foreach (Transform spawnPoint in reinforcementSpawnPoints)
			{
				Instantiate(MinionPrefab, spawnPoint.position, Quaternion.identity);
			}
		}

		private void ChugSakeAndBlowFire()
		{
			Instantiate(FirePrefab, transform.position, Quaternion.identity);
		}

		private Sequence Jump(Vector2 target, float height, float duration)
		{
			isJumping = true;
			return JumpAttack(target, height, duration).OnComplete(() =>
			{
				isJumping = false;
			});
		}

		private Sequence JumpAttack(Vector2 target, float height, float duration)
		{
			Collider.enabled = false; // Disable the collider during the jump
			GameObject shadow = Instantiate(ShadowPrefab, transform.position, Quaternion.identity);

			// Pass the player's transform as the target for the shadow
			ShadowController shadowController = shadow.GetComponent<ShadowController>();
			if (shadowController != null)
			{
				shadowController.SetTarget(target);
			}

			return DOTween.Sequence()
					.Append(transform.DOJump(target, height, 1, duration).SetEase(Ease.Linear))
					.OnComplete(() =>
					{
						JumpCompleted(shadow);
					});
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
			Destroy(shadow);
			Collider.enabled = true; // Re-enable the collider
		}

		private Transform ChooseRandomTarget(Transform[] targets)
		{
			if (targets.Length == 0)
			{
				Debug.LogError("No targets available.");
				return null;
			}

			int randomIndex = UnityEngine.Random.Range(0, targets.Length);
			return targets[randomIndex];
		}
	}
}
