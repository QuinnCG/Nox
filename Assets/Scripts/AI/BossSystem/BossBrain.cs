using Game.RoomSystem;
using UnityEngine;

namespace Game.AI.BossSystem
{
	public abstract class BossBrain : EnemyBrain
	{
		[field: SerializeField]
		public string Title { get; private set; } = "Boss Title";

		public Room Room { get; set; }


		/// <summary>
		/// Counts from 1 (e.g. 0 is invalid).
		/// </summary>
		public int Phase { get; protected set; } = 1;
	}
}
