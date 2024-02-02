using UnityEngine;

namespace Game.AI
{
	public class YieldAnimation
	{
		public const float DurationBuffer = 0.01f;

		public float EndTime { get; }

		public YieldAnimation(AnimationClip clip)
		{
			EndTime = Time.time + clip.length - DurationBuffer;
		}
	}
}
