using Game.AI.BehaviorTree;
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
		/// Counts from 1.
		/// </summary>
		[Expose]
		public BTProperty<int> Phase { get; protected set; } = new(1);
	}
}
