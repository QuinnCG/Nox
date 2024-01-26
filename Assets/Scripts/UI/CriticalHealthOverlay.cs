using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class CriticalHealthOverlay : MonoBehaviour
	{
		public Image criticalOverlay;
		public float fadeInDuration = 0.5f;
		public float fadeOutDuration = 0.5f;
		public float pulsateDuration = 0.5f;
		public float pulsateScale = 1.2f;

		private Tween pulsateTween;
		private Tween fadeInOutTween;

		private void Start()
		{
			// Ensure the critical overlay is initially invisible
			criticalOverlay.enabled = false;
		}

		public void ShowCriticalOverlay()
		{
			criticalOverlay.enabled = true;

			// Fade In
			fadeInOutTween = criticalOverlay.DOFade(1f, fadeInDuration)
				.OnComplete(() =>
				{
					// Pulsate
					PulsateOverlay();
				});
		}

		public void HideCriticalOverlay()
		{
			StopAnimations();

			// Fade Out
			fadeInOutTween = criticalOverlay.DOFade(0f, fadeOutDuration)
				.OnComplete(() =>
				{
					criticalOverlay.enabled = false;
				});
		}

		private void PulsateOverlay()
		{
			pulsateTween = criticalOverlay.rectTransform.DOScale(pulsateScale, pulsateDuration / 2)
				.OnComplete(() =>
				{
					PulsateOut();
				})
				.SetEase(Ease.InOutSine);
		}

		private void PulsateOut()
		{
			pulsateTween = criticalOverlay.rectTransform.DOScale(1f, pulsateDuration / 2)
				.OnComplete(() =>
				{
					PulsateOverlay();
				})
				.SetEase(Ease.InOutSine);
		}

		private void StopAnimations()
		{
			if (pulsateTween != null && pulsateTween.IsActive())
			{
				pulsateTween.Kill();
			}

			if (fadeInOutTween != null && fadeInOutTween.IsActive())
			{
				fadeInOutTween.Kill();
			}
		}
	}
}
