using DG.Tweening;
using System;
using UnityEngine;

namespace Game
{
	public class Timer
	{
		public float Start { get; private set; }
		public float End => Start + Duration;
		public float Duration { get; private set; }
		public float Elapsed => Time.time - Start;
		public float Remaining => End - Time.time;

		public event Action OnFinish;

		private Tween _tween;

		public Timer(float duration)
		{
			Duration = duration;
			Reset();
		}

		public static Action Wait(float duration)
		{
			var timer = new Timer(duration);
			return timer.OnFinish;
		}

		public void Reset()
		{
			Start = Time.time;

			_tween?.Kill();
			_tween = DOVirtual.DelayedCall(Duration, () =>
			{
				OnFinish?.Invoke();
			}, ignoreTimeScale: false);
		}
	}
}
