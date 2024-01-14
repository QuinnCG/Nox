using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.DamageSystem
{
	public class Health : MonoBehaviour
	{
		[field: SerializeField, ReadOnly]
		public float Current { get; private set; }

		[field: SerializeField, Tooltip("Can be set via code to another value.")]
		public float Max { get; private set; } = 100f;

		[SerializeField, Unit(Units.Percent), Tooltip("How much HP (in percent) should be left for this character to be in critical health.")]
		private float CriticalPercent = 0.3f;

		[Space, SerializeField, BoxGroup("Tools")]
		private bool StartCritical;

		public bool IsCritical => Current / Max <= CriticalPercent;

		public event Action<float> OnMaxSet;

		public event Action<float> OnDamaged;
		public event Action<DamageSource> OnDeath;

		public event Action<float> OnHealed;
		public event Action OnReachMaxHealth;

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

			if (StartCritical)
			{
				MakeCritical();
			}
		}

		[Button, BoxGroup("Tools")]
		public void MakeCritical()
		{
			RemoveHealth(Current * 0.95f);
		}

		public void SetMax(float max)
		{
			Max = max;
			OnMaxSet?.Invoke(max);
		}

		public void AddHealth(float amount)
		{
			bool isMax = Current == Max;

			// Remove HP.
			Current = Mathf.Min(Max, Current + amount);

			OnHealed?.Invoke(amount);

			// If was less than max before but now max.
			if (!isMax && Current == Max)
			{
				OnReachMaxHealth?.Invoke();
			}
		}

		public void RemoveHealth (float amount, DamageSource source = DamageSource.Misc)
		{
			// The actual amount removed (capped at 0).
			float delta = Mathf.Min(Current, amount);

			// Remove HP.
			Current = Mathf.Max(0f, Current - amount);

			OnDamaged?.Invoke(delta);

			if (Current == 0f)
			{
				OnDeath?.Invoke(source);
			}
		}
	}
}
