using DG.Tweening;
using UnityEngine;

namespace Game.AI
{
	public static class StateMachineYieldExtensions
	{
		public static YieldTween Yield (this Tween tween)
		{
			return new YieldTween(tween);
		}

		public static YieldAnimation Yield(this AnimationClip clip)
		{
			return new YieldAnimation(clip);
		}
	}
}
