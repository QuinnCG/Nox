using DG.Tweening;
using System;
using UnityEngine;

namespace Game
{
	public class Timer
	{
		public float Start { get; private set; }
		public float End { get; private set; }
		public float Duration { get; private set; }
		public float Elapsed => Time.time - Start;
		public float Remaining => End - Time.time;
		public bool IsDone => Remaining <= 0f;

		public bool StartFinished { get; set; } = false;

		public event Action OnFinish;

		private Tween _tween;

		public Timer(float duration, bool startFinished = false)
		{
			Duration = duration;
			StartFinished = startFinished;
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
			End = StartFinished ? Start : Start + Duration;

			_tween?.Kill();
			_tween = DOVirtual.DelayedCall(Duration, () =>
			{
				OnFinish?.Invoke();
			}, ignoreTimeScale: false);
		}
	}
}
