using Game.Characters;
using System.Collections;
using UnityEngine;

namespace Game.AI.Brains
{
	public class FlintLockBanditBrain : EnemyBrain
	{
		[SerializeField]
		private float PlayerDstToFlee = 4f;

		private State _idle, _engage, _barrage, _flee;
		private Timer _engageTimer, _engageEndTimer, _engageShootIntervalTimer;

		private FlintLockBandit _bandit;

		protected override void Start()
		{
			base.Start();

			_bandit = Character as FlintLockBandit;

			_idle = CreateState(OnIdle, "Idle");
			_engage = CreateState(OnEngage, "Engage");
			_barrage = CreateState(OnBarrage, "Barrage");
			_flee = CreateState(OnFlee, "Flee");

			ResetEngageTimer();
			_engageEndTimer = new Timer();
			_engageShootIntervalTimer = new Timer(1.5f);

			Idle();
		}

		/* TRANSITIONS */
		private void Idle()
		{
            if (Random.value <= 0.25f)
            {
				Barrage();
				return;
            }

            TransitionTo(_idle);
		}
		
		private void Flee()
		{
			TransitionTo(_flee);
		}

		private void Engage()
		{
			_engageEndTimer.Duration = Random.Range(2f, 5f);
			TransitionTo(_engage);
			ResetEngageTimer();
		}

		private void Barrage()
		{
			TransitionTo(_barrage);
		}

		/* STATES */
		private void OnIdle()
		{
			Animator.Play(_bandit.IdleAnim);
			Move(Vector2.zero);
			Movement.FaceDirection(DirectionToPlayer.x);

			if (IsPlayerTooClose())
			{
				Flee();
			}
			else if (_engageTimer.IsDone)
			{
				Engage();
			}
		}

		private void OnEngage()
		{
			if (DistanceToPlayer > 1f)
			{
				Animator.Play(_bandit.MoveAnim);
				Vector2 dir = DirectionToPlayer;
				Move(dir);
			}
			else
			{
				Animator.Play(_bandit.IdleAnim);
				Move(Vector2.zero);
			}

			if (_engageShootIntervalTimer.IsDone)
			{
				ShootAtPlayer();
				_engageShootIntervalTimer.Reset(Random.Range(0.5f, 1.5f));
			}

			if (_engageEndTimer.IsDone)
			{
				Idle();
			}
		}

		private IEnumerator OnBarrage()
		{
			Animator.Play(_bandit.IdleAnim);
			yield return new YieldSeconds(2f);

			var duration = new Timer(Random.Range(2f, 4f));
			while (!duration.IsDone)
			{
				Debug.Log("!");
				ShootAtPlayer();
				yield return new YieldSeconds(0.2f);
			}
		}

		private void OnFlee()
		{
			Animator.Play(_bandit.MoveAnim);
			Move(-DirectionToPlayer);

			if (DistanceToPlayer > 10f)
			{
				Barrage();
			}
		}

		/* UTILITIES */
		private bool IsPlayerTooClose()
		{
			return DistanceToPlayer <= PlayerDstToFlee;
		}

		private void ResetEngageTimer()
		{
			_engageTimer = new Timer(Random.Range(2f, 12f));
		}

		private void ShootAtPlayer()
		{
			Vector2 rand = Random.insideUnitSphere;
			rand *= 1f;
			rand += PlayerPosition;

			//Character.Attack(rand);
		}
	}
}
