using UnityEngine;

namespace Game.AI.BossSystem
{
	public abstract class BossBrain : EnemyBrain
	{
		public int Phase { get; private set; }
	}
}
