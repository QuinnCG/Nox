using DG.Tweening;

namespace Game.AI
{
	public class YieldTween
	{
		public Tween Tween { get; }

		public YieldTween(Tween tween)
		{
			Tween = tween;
		}
	}
}
