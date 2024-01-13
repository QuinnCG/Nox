using UnityEngine;

namespace Game.DamageSystem
{
	[System.Serializable]
	public class DamageInfo
	{
		public float Damage;
		public Vector2 Direction;
		public DamageSource Source;
	}
}
