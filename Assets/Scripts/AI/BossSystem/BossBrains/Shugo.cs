using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEditor.Rendering;
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

		private Vector2 _jumpStart, _jumpEnd;
		private Transform _shadow;

		protected override void Start()
		{
			base.Start();

			_idle = CreateState(OnIdle, "Idle");
			_sakeChug = CreateState(OnSakeChug, "Sake Chug");
			_jump = CreateState(OnJump, "Jump");
			_callReinforcements = CreateState(OnCallReinforcements, "Call Reinforcements");

			_abilityCooldown = new Timer(0.5f);

			TransitionTo(_idle);
		}

		private void OnIdle()
		{
			if (_abilityCooldown.IsDone)
			{
				float randomValue = Random.value;
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

		private IEnumerator OnJump()
		{
			yield return new YieldEnumerator(OnJumpSequence(PlayerPosition, 5f, 2f));
			TransitionTo(_idle);
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

		private IEnumerator OnJumpSequence(Vector2 target, float height, float duration)
		{
			Collider.enabled = false;
			SpawnShadow();

			_jumpStart = transform.position;
			_jumpEnd = PlayerPosition;

			var tween = Jump(_jumpEnd, 15f, 2f);

			while (!tween.IsComplete())
			{
				float progress = tween.Elapsed() / tween.Duration();
				_shadow.position = Vector2.Lerp(_jumpStart, _jumpEnd, progress);
				
				yield return new YieldNextFrame();
			}
			
			Collider.enabled = true;
			Destroy(_shadow.gameObject);
		}

		private void SpawnShadow()
		{
			_shadow = Instantiate(ShadowPrefab, transform.position, Quaternion.identity).transform;
			_shadow.GetComponent<ShadowController>().enabled = false;
		}
	}
}
