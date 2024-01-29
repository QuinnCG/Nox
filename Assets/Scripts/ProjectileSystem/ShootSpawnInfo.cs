namespace Game.ProjectileSystem
{
	[System.Serializable]
	public class ShootSpawnInfo
	{
		public int Count = 1;
		public ShootMethod Method = ShootMethod.Straight;
		public float SpreadAngle = 45f;
	}
}
