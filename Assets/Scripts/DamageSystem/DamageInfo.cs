using UnityEngine;

namespace Game.DamageSystem
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Vector2 Direction;
		public DamageType Type;
		public Character Source;

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
		public DamageInfo(float damage, DamageType type)
		{
			Damage = damage;
			Type = type;
		}
		public DamageInfo(float damage, Vector2 direction, DamageType type)
		{
			Damage = damage;
			Direction = direction;
			Type = type;
		}
	}
}
