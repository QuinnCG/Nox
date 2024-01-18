using UnityEngine;

namespace Game.DamageSystem
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Vector2 Direction;
		public DamageSource Source;

		public DamageInfo() { }
		public DamageInfo(float damage)
		{
			Damage = damage;
		}
		public DamageInfo(float damage, Vector2 direction)
		{
			Damage = damage;
			Direction = direction;
		}
		public DamageInfo(float damage, DamageSource source)
		{
			Damage = damage;
			Source = source;
		}
		public DamageInfo(float damage, Vector2 direction, DamageSource source)
		{
			Damage = damage;
			Direction = direction;
			Source = source;
		}
	}
}
