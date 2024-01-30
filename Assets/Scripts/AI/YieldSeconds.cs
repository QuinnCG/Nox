using UnityEngine;

namespace Game.AI
{
	public class YieldSeconds
	{
		public float End { get; }

		public YieldSeconds(float duration)
		{
			End = Time.time + duration;
		}
	}
}
