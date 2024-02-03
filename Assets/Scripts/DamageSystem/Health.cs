using DG.Tweening;
using FMODUnity;
using Game.GeneralManagers;
using Game.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;

namespace Game.DamageSystem
{
	public class Health : MonoBehaviour
	{
		[field: SerializeField, ReadOnly]
		public float Current { get; private set; }

		[field: SerializeField, Tooltip("Can be set via code to another value.")]
		public float Max { get; private set; } = 100f;

		[SerializeField]
		private Vector2 CriticalIndicatorOffset = new(0.2f, 0.5f);

		[SerializeField, Tooltip("How much HP (in percent) should be left for this character to be in critical health.")]
		private float CriticalPercent = 0.3f;

		[Space, SerializeField, BoxGroup("Tools")]
		private bool StartCritical;

		[SerializeField]
		private EventReference HurtSound;

		public bool DisplayCriticalIndicator { get; private set; } = true;
		public bool DisableDamage { get; set; } = false;

		public bool IsCritical => Current / Max <= CriticalPercent;
		public bool IsDead => Current == 0f;
		public float Percent => Current / Max;

		public event Action<float> OnMaxSet;

		public event Action<float> OnDamaged;
		public event Action<DamageType> OnDeath;

		public event Action<float> OnHealed;
		public event Action OnReachMaxHealth;

		public event Action OnCritical;
		public event Action OnHealFromCritical;

		private GameObject _criticalIndicator;

		private Tween _playCriticalSFXTween;

		private bool isCriticalIndicatorVisible = false;

		private void Awake()
		{
			Current = Max;

			OnCritical += OnEnterCritical;
			OnHealFromCritical += OnLeaveCritical;
			OnDeath += _ =>
			{
				SetDisplayCriticalIndiactor(false);
			};

			if (StartCritical)
			{
				MakeCritical();
			}
		}

		private void OnDestroy()
		{
			// This Ensures that the critical indicator is destroyed when the Health component is destroyed.
			HideCriticalIndicator();
		}

		public void SetMax(float max)
		{
			Max = max;
			OnMaxSet?.Invoke(max);
		}

		public void AddHealth(float amount)
		{
			if (IsDead) return;

			bool isMax = Current == Max;
			bool wasCritical = IsCritical;

			// Remove HP.
			Current = Mathf.Min(Max, Current + amount);

			OnHealed?.Invoke(amount);

			// If was less than max before but now max.
			if (!isMax && Current == Max)
			{
				OnReachMaxHealth?.Invoke();
			}

			if (wasCritical && !IsCritical)
			{
				OnHealFromCritical?.Invoke();
			}
		}

		public void TakeDamage(DamageInfo info)
		{
			if (IsDead || DisableDamage) return;

			// The actual amount removed (capped at 0).
			float delta = Mathf.Min(Current, info.Damage);

			// Remove HP.
			Current = Mathf.Max(0f, Current - info.Damage);

			OnDamaged?.Invoke(delta);

			if (PossessionManager.Instance.PossessedCharacter.gameObject == gameObject)
			{
				AudioManager.PlayOneShot(PossessionManager.Instance.PlayerHurtSound);
			}
			else
			{
				AudioManager.PlayOneShot(HurtSound);
			}

			if (Current < Max * CriticalPercent)
			{
				OnCritical?.Invoke();
			}

			if (Current == 0f)
			{
				OnDeath?.Invoke(info.Type);
			}
		}
		public void TakeDamage(float damage)
		{
			TakeDamage(new DamageInfo(damage));
		}
		public void TakeDamage(float damage, Vector2 direction)
		{
			TakeDamage(new DamageInfo(damage, direction));
		}
		public void TakeDamage(float damage, Vector2 direction, DamageType source)
		{
			TakeDamage(new DamageInfo(damage, direction, source));
		}
		public void TakeDamage(float damage, DamageType source)
		{
			TakeDamage(new DamageInfo(damage, source));
		}

		[Button, BoxGroup("Tools")]
		public void MakeCritical()
		{
			Current = (Max * CriticalPercent) - 0.1f;
			TakeDamage(1f);
		}

		[Button, BoxGroup("Tools")]
		public void FullHeal()
		{
			AddHealth(Max - Current + 1f);
		}

		public void Kill(DamageType source = DamageType.Misc)
		{
			TakeDamage(Current, source);
		}

		public void SetDisplayCriticalIndiactor(bool display)
		{
			if (DisplayCriticalIndicator != display)
			{
				DisplayCriticalIndicator = display;

				if (display && IsCritical)
				{
					ShowCriticalIndicator();
				}

				if (!display && IsCritical)
				{
					HideCriticalIndicator();
				}
			}
		}

		public void OnPossessed()
		{
			_playCriticalSFXTween?.Kill();
		}

		private void OnEnterCritical()
		{
			if (DisplayCriticalIndicator)
			{
				ShowCriticalIndicator();

				_playCriticalSFXTween.Kill();
				_playCriticalSFXTween = DOVirtual.DelayedCall(0.38f, () =>
				{
					if (_criticalIndicator != null)
					{
						RuntimeManager.PlayOneShotAttached("event:/SFX/EnterCritical", _criticalIndicator);
					}
				}, ignoreTimeScale: false);
			}
		}

		private void OnLeaveCritical()
		{
			if (_criticalIndicator)
			{
				HideCriticalIndicator();
			}
		}

		private void ShowCriticalIndicator()
		{
			if (!isCriticalIndicatorVisible)
			{
				var bounds = GetComponent<Collider2D>().bounds;
				Vector3 offset = CriticalIndicatorOffset;

				const string key = "CriticalHealth.prefab";
				Vector2 pos = bounds.center + (Vector3.up * bounds.extents.y) + offset;

				_criticalIndicator = Addressables.InstantiateAsync(key, pos, Quaternion.identity, transform)
						.WaitForCompletion();

				isCriticalIndicatorVisible = true;
			}
		}

		private void HideCriticalIndicator()
		{
			if (_criticalIndicator != null)
			{
				Destroy(_criticalIndicator);
				isCriticalIndicatorVisible = false;
			}
		}
	}
}
