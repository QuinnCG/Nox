using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

		public bool DisplayCriticalIndicator { get; private set; } = true;

		public bool IsCritical => Current / Max <= CriticalPercent;

		public event Action<float> OnMaxSet;

		public event Action<float> OnDamaged;
		public event Action<DamageSource> OnDeath;

		public event Action<float> OnHealed;
		public event Action OnReachMaxHealth;

		public event Action OnCritical;
		public event Action OnHealFromCritical;

		private GameObject _criticalIndicator;

		private Material _healthyMat;
		private Material _criticalMat;
		private SpriteRenderer _characterRenderer;

		private void Awake()
		{
			Current = Max;

			// The health component doesn't require a damage component,
			// but if one is present on the same game object,
			// then it will interface with it.
			if (TryGetComponent(out Damage damage))
			{
				// Note: look up "lambda functions".
				damage.OnDamage += info => RemoveHealth(info.Damage, info.Source);
			}

			_characterRenderer = GetComponentInChildren<SpriteRenderer>();

			_healthyMat = Addressables.LoadAssetAsync<Material>("HealthyOutline.mat").WaitForCompletion();
			_criticalMat = Addressables.LoadAssetAsync<Material>("CriticalOutline.mat").WaitForCompletion();

			OnCritical += OnEnterCritical;
			OnHealFromCritical += OnLeaveCritical;

			if (StartCritical)
			{
				MakeCritical();
			}
		}

		[Button, BoxGroup("Tools")]
		public void MakeCritical()
		{
			Current = (Max * CriticalPercent) - 0.1f;
			RemoveHealth(1f);
		}

		[Button, BoxGroup("Tools")]
		public void FullHeal()
		{
			AddHealth(Max - Current + 1f);
		}

		public void SetMax(float max)
		{
			Max = max;
			OnMaxSet?.Invoke(max);
		}

		public void AddHealth(float amount)
		{
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

		public void RemoveHealth (float amount, DamageSource source = DamageSource.Misc)
		{
			// The actual amount removed (capped at 0).
			float delta = Mathf.Min(Current, amount);

			// Remove HP.
			Current = Mathf.Max(0f, Current - amount);

			OnDamaged?.Invoke(delta);

			if (Current < Max * CriticalPercent)
			{
				OnCritical?.Invoke();
			}

			if (Current == 0f)
			{
				OnDeath?.Invoke(source);
			}
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

		private void OnEnterCritical()
		{
			if (DisplayCriticalIndicator)
			{
				ShowCriticalIndicator();

				DOVirtual.DelayedCall(0.38f, () =>
				{
					RuntimeManager.PlayOneShotAttached("event:/EnterCritical", _criticalIndicator);
				}, ignoreTimeScale: false);

				_characterRenderer.material = _criticalMat;
			}
		}

		private void OnLeaveCritical()
		{
			if (_criticalIndicator)
			{
				HideCriticalIndicator();
				_characterRenderer.material = _healthyMat;
			}
		}

		private void ShowCriticalIndicator()
		{
			var bounds = GetComponent<Collider2D>().bounds;
			Vector3 offset = CriticalIndicatorOffset;

			const string key = "CriticalHealth.prefab";
			Vector2 pos = bounds.center + (Vector3.up * bounds.extents.y) + offset;

			_criticalIndicator = Addressables.InstantiateAsync(key, pos, Quaternion.identity, transform)
				.WaitForCompletion();
		}

		private void HideCriticalIndicator()
		{
			Destroy(_criticalIndicator);
		}
	}
}
