using DG.Tweening;

namespace Game.AI
{
	public static class TweenYieldExtension
	{
		public static YieldTween YieldTween (this Tween tween)
		{
			return new YieldTween(tween);
		}
	}
}
