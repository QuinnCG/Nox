using System;
using UnityEngine;

namespace Game.DamageSystem
{
	public class Health : MonoBehaviour
	{
		[field: SerializeField, Tooltip("Can be set via code to another value.")]
		public float Max { get; private set; } = 100f;
		public float Current { get; private set; }

		public event Action<float> OnMaxSet;

		public event Action<float> OnHealthRemoved;
		public event Action<DamageSource> OnDeath;

		public event Action<float> OnHealAdded;
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

			OnHealAdded?.Invoke(amount);

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

			OnHealthRemoved?.Invoke(delta);

			if (Current == 0f)
			{
				OnDeath?.Invoke(source);
			}
		}
	}
}
