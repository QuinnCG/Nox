using DG.Tweening;
using Game.ProjectileSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game.AI.BossSystem.BossBrains
{
	public class Shugo : BossBrain
	{
		[SerializeField, BoxGroup("Phase 1")]
		private float JumpHeight = 5f, JumpDuration = 2f;

		[SerializeField, BoxGroup("Phase 1")]
		private float SuperJumpHeight = 5f, SuperJumpDuration = 2f;

		[SerializeField, BoxGroup("Phase 1")]
		private float SuperJumpWeight = 1f, FireSpewWeight = 1f;

		[SerializeField, BoxGroup("Phase 1"), MinMaxSlider(0f, 10f, ShowFields = true)]
		private Vector2 SpecialAttackInterval = new(3f, 7f);

		[SerializeField, BoxGroup("Phase 1"), MinMaxSlider(0f, 10f, ShowFields = true)]
		private Vector2 IdleDuration = new(0.5f, 2f);

		[SerializeField, BoxGroup("Phase 1")]
		private int SuperJumpFireballCount = 36;

		[SerializeField, BoxGroup("Phase 1")]
		private int FireSpewCount = 12;

		[SerializeField, BoxGroup("Phase 1")]
		private float FireSpewAngle = 90f;

		[SerializeField, BoxGroup("Phase 2")]
		private float PhaseTwoHealthPercentThreshold = 0.5f;

		[SerializeField, BoxGroup("Phase 2")]
		private float JumpDurationFactor = 0.7f, SuperJumpDurationFactor = 0.7f;

		[SerializeField, BoxGroup("Phase 2")]
		private float SpecialAttackIntervalFactor = 0.7f;

		[SerializeField, BoxGroup("Phase 2")]
		private float IdleDurationFactor = 0.5f;

		[SerializeField, BoxGroup("Phase 2")]
		private int SuperJumpFireballCount2 = 48;

		[SerializeField, BoxGroup("Phase 2")]
		private int SuperJumpFireballWaveCount = 2;

		[SerializeField, BoxGroup("Phase 2")]
		private float SuperJumpFireballWaveInterval = 0.35f;

		[SerializeField, BoxGroup("Phase 2")]
		private int FireSpewCount2 = 24;

		[SerializeField, BoxGroup("Phase 2")]
		private float FireSpewAngle2 = 120f;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject FireballPrefab;

		[SerializeField, Required, BoxGroup("References")]
		private Transform FireballSpawnPoint;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject MinionPrefab;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject ShadowPrefab;

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] FireSpewPoints;

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] JumpPoints;

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] SummonPoints;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip IdleAnim;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip FireSpewStart, FireSpew;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip JumpStart, JumpLoop, JumpEnd;

		private bool IsSecondPhase => Phase > 1;
		private float RealJumpDuration => IsSecondPhase ? JumpDuration : (JumpDuration * JumpDurationFactor);

		private State _delayedStart, _wander, _superJump, _fireSpew, _summon;
		private Timer _idleTimer, _specialTimer;

		protected override void Start()
		{
			base.Start();

			Health.OnDamaged += OnDamage;

			// States.
			_delayedStart = CreateState(OnDelayedStart, "Delayed Start");
			_wander = CreateState(OnWander, "Wander");
			_superJump = CreateState(OnSuperJump, "Super Jump");
			_fireSpew = CreateState(OnFireSpew, "Fire Spew");
			_summon = CreateState(OnSummon, "Summon");

			// Timers.
			_idleTimer = new Timer();
			_specialTimer = new Timer();

			//TransitionTo(_delayedStart);
			Idle();
		}

		private void OnDamage(float health)
		{
			// Switch to second phase.
			if (Health.Percent <= PhaseTwoHealthPercentThreshold)
			{
				Phase = 2;
			}
		}

		/* SUPER STATES */
		private void Idle()
		{
			// Reset idle timer.
			float duration = Random.Range(IdleDuration.x, IdleDuration.y);
			duration *= IsSecondPhase ? IdleDurationFactor : 1f;
			_idleTimer.Duration = duration;

			// Transition to most passive state.
			TransitionTo(_wander);
		}

		// Execute weighted-random special attack.
		private void ExecuteRandomSpecial()
		{
			// States and their weights (used for chance).
			(float weight, State state)[] specials = new (float, State)[]
			{
				(SuperJumpWeight, _superJump),
				(FireSpewWeight, _fireSpew),
			};

			// Needed for weighted chance calculation.
			float sum = specials.Sum(x => x.weight);

			// Choose at random while accounting for weights.
			foreach (var (weight, state) in specials)
			{
				float chance = weight / sum;
                if (Random.value <= chance)
                {
					TransitionTo(state);
					return;
                }
            }

			// Due to floating-point imprecision, there's a chance no state will be selected above.
			var fallback = specials[Random.Range(0, specials.Length - 1)];
			TransitionTo(fallback.state);

			// Reset timer.
			float interval = Random.Range(SpecialAttackInterval.x, SpecialAttackInterval.y);
			interval *= IsSecondPhase ? SpecialAttackIntervalFactor : 1f;
			_specialTimer.Duration = interval;

			_specialTimer.Reset();
		}

		/* STATES */
		private void OnDelayedStart()
		{
			// Wait for player to enter room.
		}

		private void OnWander()
		{
			Animator.Play(IdleAnim);

			if (_specialTimer.IsDone && !IsJumping)
			{
				ExecuteRandomSpecial();
			}
			else if (_idleTimer.IsDone && !IsJumping)
			{
				// Jump onto player.
				if (Random.value < 0.5f)
				{
					Tween jump = ShugoJump(PlayerPosition, JumpHeight, RealJumpDuration);
					jump.onComplete += () => Idle();
				}
				// Jump to random point.
				else
				{
					Vector2 target = GetPointClosestTo(PlayerPosition, FireSpewPoints);

					Tween jump = ShugoJump(target, JumpHeight, RealJumpDuration);
					jump.onComplete += () => Idle();
				}
			}
		}

		private IEnumerator OnSuperJump()
		{
			// Jump to player.
			float duration = SuperJumpDuration * (IsSecondPhase ? SuperJumpDurationFactor : 1f);
			Tween jump = ShugoJump(PlayerPosition, SuperJumpHeight, duration);
			yield return jump.Yield();

			// Spawn ring of fire.
			if (IsSecondPhase)
			{
				for (int i = 0; i < SuperJumpFireballWaveCount; i++)
				{
					Shoot(FireballPrefab, FireballSpawnPoint.position, Vector2.up, new ShootSpawnInfo()
					{
						Count = SuperJumpFireballCount2,
						Method = ShootMethod.EvenCircle
					});

					yield return new YieldSeconds(SuperJumpFireballWaveInterval);
				}
			}
			else
			{
				Shoot(FireballPrefab, FireballSpawnPoint.position, Vector2.up, new ShootSpawnInfo()
				{
					Count = SuperJumpFireballCount,
					Method = ShootMethod.EvenCircle
				});
			}

			// Transition to idle.
			Idle();
		}

		private IEnumerator OnFireSpew()
		{
			// Jump to corner.
			Vector2 target = GetPointClosestTo(PlayerPosition, FireSpewPoints);
			Tween jump = ShugoJump(target, JumpHeight, JumpDuration);
			yield return jump.Yield();

			// Spew fire.
			Shoot(FireballPrefab, FireballSpawnPoint.position, PlayerPosition, new ShootSpawnInfo()
			{
				Count = IsSecondPhase ? FireSpewCount2 : FireSpewCount,
				SpreadAngle = IsSecondPhase ? FireSpewAngle2 : FireSpewAngle,
				Method = ShootMethod.RandomSpread
			});

			// Transition to idle.
			Idle();
		}

		private void OnSummon()
		{
			// Roar!
			// Simultaneously, spawn minions in poof of black smoke
		}

		/* UTILITIES */
		private Tween ShugoJump(Vector2 target, float height, float duration)
		{
			var sequence = DOTween.Sequence();
			sequence.Append(DOVirtual.DelayedCall(0f, () => Animator.Play(JumpStart)));
			sequence.AppendInterval(JumpStart.length - 0.01f);
			sequence.Append(DOVirtual.DelayedCall(0f, () => Animator.Play(JumpLoop)));
			sequence.Append(SuperJump(target, height, duration, ShadowPrefab));
			sequence.Append(DOVirtual.DelayedCall(0f, () => Animator.Play(JumpEnd)));

			return sequence;
		}
	}
}
