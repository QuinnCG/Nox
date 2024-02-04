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
		public DamageInfo(float damage, Character source)
		{
			Damage = damage;
			Source = source;
		}
	}
}
