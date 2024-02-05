using DG.Tweening;
using FMODUnity;
using Game.Common;
using Game.DamageSystem;
using Game.GeneralManagers;
using Game.ProjectileSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
		private int FireSpewWaveCount = 2;

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
		private Trigger EnterTrigger;

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] FireSpewPoints;

		[SerializeField, Required, BoxGroup("References")]
		private BoxCollider2D JumpRegion;

		[SerializeField, Required, BoxGroup("References")]
		private Transform[] SummonPoints;

		[SerializeField, Required, BoxGroup("References")]
		private Transform CenterPoint;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject BanditSpawnSmoke;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip IdleAnim;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip FireSpewStart, FireSpew;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip JumpStart, JumpLoop, JumpEnd;

		[SerializeField, BoxGroup("Animations"), Required]
		private AnimationClip Roar, Death;

		[SerializeField, BoxGroup("SFX"), Required]
		private EventReference JumpSound, LandSound;

		[SerializeField, BoxGroup("SFX"), Required]
		private EventReference FireSpewStartSound, FireSpewSound;

		[SerializeField, BoxGroup("SFX"), Required]
		private EventReference FireStompSound;

		[SerializeField, BoxGroup("SFX"), Required]
		private EventReference RoarSound, DeathSound;

		[SerializeField, BoxGroup("SFX"), Required]
		private EventReference MinionSpawnSound;

		private bool IsSecondPhase => Phase > 1;
		private float RealJumpDuration => IsSecondPhase ? JumpDuration : (JumpDuration * JumpDurationFactor);

		private State _stationary, _wander, _superJump, _fireSpew, _summon, _dead;
		private Timer _idleTimer, _specialTimer, _summonTimer;

		private bool _isPlayerIn;
		private readonly List<Character> _aliveMinions = new();

		private Vector2 _deathPos;

		protected override void Start()
		{
			base.Start();

			Health.OnDamaged += OnDamage;
			Health.OnDeath += _ => Dead();

			// States.
			_stationary = CreateState(OnStartState, "Stationary");
			_wander = CreateState(OnWander, "Wander");
			_superJump = CreateState(OnSuperJump, "Super Jump");
			_fireSpew = CreateState(OnFireSpew, "Fire Spew");
			_summon = CreateState(OnSummon, "Summon");
			_dead = CreateState(OnDeath, "Dead");

			// Timers.
			_idleTimer = new Timer();
			_specialTimer = new Timer();
			_summonTimer = new Timer(5f);

			TransitionTo(_stationary);
		}

		private void LateUpdate()
		{
			if (IsDead)
			{
				transform.position = _deathPos;
				transform.DOKill();
			}
		}

		/* EVENTS */
		public override void OnPlayerEnter()
		{
			_isPlayerIn = true;
		}

		private void OnDamage(float health)
		{
			// Switch to second phase.
			if (Health.Percent <= PhaseTwoHealthPercentThreshold && Phase != 2)
			{
				Phase = 2;
				TransitionTo(_summon);
			}
		}

		private void Dead()
		{
			_deathPos = transform.position;
			TransitionTo(_dead);
		}

		/* SUPER STATES */
		private void Idle()
		{
			if (IsDead) return;

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
			if (IsDead) return;

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
		}

		/* STATES */
		private IEnumerator OnStartState()
		{
			yield return new YieldUntil(() => _isPlayerIn);

			// Jump to center. Skip start-up.
			ShugoJump(CenterPoint.position, 20f, 2f, skipStart: true).Yield();
			Idle();
		}

		// Idle/passive state.
		private void OnWander()
		{
			if (!IsJumping)
			{
				Animator.Play(IdleAnim);
			}

			if (_specialTimer.IsDone && !IsJumping)
			{
				ExecuteRandomSpecial();
			}
			else if (_summonTimer.IsDone && !IsJumping && _aliveMinions.Count < 5)
			{
				TransitionTo(_summon);
			}
			else if (_idleTimer.IsDone && !IsJumping)
			{
				// Jump onto player.
				if (Random.value < 0.5f)
				{
					Tween jump = ShugoJump(PlayerPosition, JumpHeight, RealJumpDuration);
					jump.onComplete += () =>
					{
						if (ActiveState == _wander)
						{
							Idle();
						}
					};
				}
				// Jump to random point.
				else
				{
					Vector2 center = JumpRegion.bounds.center;
					Vector2 half = JumpRegion.size / 2f;
					Vector2 lower = center - half;
					Vector2 upper = center + half;

					var target = new Vector2()
					{
						x = Random.Range(lower.x, upper.x),
						y = Random.Range(lower.y, upper.y)
					};

					Tween jump = ShugoJump(target, JumpHeight, RealJumpDuration);
					jump.onComplete += () =>
					{
						if (ActiveState == _wander)
						{
							Idle();
						}
					};
				}
			}
		}
		private IEnumerator OnSuperJump()
		{
			// Jump to player.
			float duration = SuperJumpDuration * (IsSecondPhase ? SuperJumpDurationFactor : 1f);
			Tween jump = ShugoJump(PlayerPosition, SuperJumpHeight, duration);
			yield return jump.Yield();

			AudioManager.PlayOneShot(FireStompSound);

			// Spawn ring of fire.
			if (IsSecondPhase)
			{
				for (int i = 0; i < SuperJumpFireballWaveCount; i++)
				{
					Shoot(FireballPrefab, FireballSpawnPoint.position, Vector2.up, new ShootSpawnInfo()
					{
						Count = SuperJumpFireballCount2,
						Method = ShootMethod.RandomCircle
					});

					yield return new YieldSeconds(SuperJumpFireballWaveInterval);
				}
			}
			else
			{
				Shoot(FireballPrefab, transform.position, Vector2.up, new ShootSpawnInfo()
				{
					Count = SuperJumpFireballCount,
					Method = ShootMethod.EvenCircle
				});
			}

			// Transition to idle.
			Idle();
			ResetSpecialtimer();
		}

		private IEnumerator OnFireSpew()
		{
			// Jump to corner.
			Vector2 target = GetPointClosestTo(PlayerPosition, FireSpewPoints, true);
			Tween jump = ShugoJump(target, JumpHeight, JumpDuration);
			yield return jump.Yield();

			DOVirtual.DelayedCall(0.85f, () => AudioManager.PlayOneShot(FireSpewStartSound));
			yield return Animator.Play(FireSpewStart).Yield();

			for (int i = 0; i < FireSpewWaveCount; i++)
			{
				Movement.FaceDirection(DirectionToPlayer.x);

				// Spew fire.
				Shoot(FireballPrefab, FireballSpawnPoint.position, PlayerPosition, new ShootSpawnInfo()
				{
					Count = IsSecondPhase ? FireSpewCount2 : FireSpewCount,
					SpreadAngle = IsSecondPhase ? FireSpewAngle2 : FireSpewAngle,
					Method = ShootMethod.RandomSpread
				});

				AudioManager.PlayOneShot(FireSpewSound);
				yield return Animator.Play(FireSpew).Yield();
			}

			ResetSpecialtimer();

			// Transition to idle.
			Idle();
		}

		private IEnumerator OnSummon()
		{
			if (_aliveMinions.Count <= 5)
			{
				int summonCount = 5;
				summonCount = Mathf.Min(summonCount, SummonPoints.Length);

				for (int i = 0; i < summonCount; i++)
				{
					if (_aliveMinions.Count >= 5)
						break;

					Transform point = SummonPoints[Random.Range(0, SummonPoints.Length - 1)];
					var instance = Instantiate(MinionPrefab, point.position, Quaternion.identity, transform.root);

					var character = instance.GetComponent<Character>();
					_aliveMinions.Add(character);
					instance.GetComponent<Health>().OnDeath += _ => _aliveMinions.Remove(character);
					character.OnPossessed += () => _aliveMinions.Remove(character);

					var smoke = Instantiate(BanditSpawnSmoke, character.transform.position, Quaternion.identity);
					Destroy(smoke, 3f);

					AudioManager.PlayOneShot(MinionSpawnSound, character.transform.position);
				}

				yield return new YieldSeconds(1.4f);

				Animator.Play(Roar);
				AudioManager.PlayOneShot(RoarSound, transform.position);
			}

			_summonTimer.Duration = Random.Range(7f, 15f);
			Idle();
		}

		private IEnumerator OnDeath()
		{
			transform.DOKill();

			// Kill minions.
			foreach (var minion in _aliveMinions)
			{
				minion.GetComponent<Health>().Kill();
			}
			_aliveMinions.Clear();

			// Freeze on last frame.
			Animator.Play(Death);
			AudioManager.PlayOneShot(DeathSound, transform.position);

			yield return Death.Yield();

			Animator.Stop();
			Animator.enabled = false;

			// Never loop this state.
			yield return new YieldUntil(() => false);
		}

		/* UTILITIES */
		private Tween ShugoJump(Vector2 target, float height, float duration, bool skipStart = false, bool skipEnd = false)
		{
			if (IsDead) return null;

			var sequence = DOTween.Sequence();
			Health.OnDeath += _ =>
			{
				if (sequence != null && sequence.IsActive())
				{
					DOTween.Kill(sequence);
				}
			};

			if (!skipStart)
			{
				// Start.
				sequence.Append(DOVirtual.DelayedCall(0f, () => Animator.Play(JumpStart)));
				sequence.AppendInterval(JumpStart.length - 0.01f);

				sequence.Append(DOVirtual.DelayedCall(0f, () => AudioManager.PlayOneShot(JumpSound)));
			}

			// Loop.
			sequence.Append(DOVirtual.DelayedCall(0f, () => GetComponentInChildren<SpriteRenderer>().sortingOrder = 100));

			sequence.Append(DOVirtual.DelayedCall(0f, () => Animator.Play(JumpLoop)));
			sequence.Append(SuperJump(target, height, duration, ShadowPrefab));

			// End.
			sequence.Append(DOVirtual.DelayedCall(0f, () => GetComponentInChildren<SpriteRenderer>().sortingOrder = 0));

			sequence.Append(DOVirtual.DelayedCall(0f, () => AudioManager.PlayOneShot(LandSound)));
			//if (!skipEnd)
			//{
			//	sequence.Append(DOVirtual.DelayedCall(0f, () => AudioManager.PlayOneShot(LandSound)));
			//	sequence.Append(DOVirtual.DelayedCall(0f, () => Animator.Play(JumpEnd)));
			//	sequence.AppendInterval(JumpEnd.length - 0.01f);
			//}

			float dir = target.x - transform.position.x;
			sequence.onUpdate += () => { if (IsJumping) { Movement.FaceDirection(dir); } };

			return sequence;
		}

		private void ResetSpecialtimer()
		{
			float interval = Random.Range(SpecialAttackInterval.x, SpecialAttackInterval.y);
			interval *= IsSecondPhase ? SpecialAttackIntervalFactor : 1f;
			_specialTimer.Duration = interval;

			_specialTimer.Reset();
		}
	}
}
