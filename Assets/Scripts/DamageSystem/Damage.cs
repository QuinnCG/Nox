using System;
using UnityEngine;

namespace Game.DamageSystem
{
	/// <summary>
	/// Becuase damage and health are separated,
	/// you could have a grass tuft that can be destroyed in one hit
	/// without excess FX unique to character's taking health damage.
	/// </summary>
	public class Damage : MonoBehaviour
	{
		public event Action<DamageInfo> OnDamage;

		public void TakeDamage(float damage, Vector2 direction, DamageSource source = DamageSource.Misc)
		{
			OnDamage?.Invoke(new DamageInfo()
			{
				Damage = damage,
				Direction = direction,
				Source = source
			});
		}
	}
}
