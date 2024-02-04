using DG.Tweening;
using UnityEngine;

namespace Game.UI
{
	public class Breathe : MonoBehaviour
	{
		[SerializeField]
		private float MaxScale = 1.1f, CycleDuration = 10f;

		private void Start()
		{
			float dur = CycleDuration / 2f;

			var sequence = DOTween.Sequence();
			sequence.Append(transform.DOScale(MaxScale, dur).SetEase(Ease.InOutSine));
			sequence.Append(transform.DOScale(1f, dur).SetEase(Ease.InOutSine));
			sequence.SetLoops(-1);
		}

		private void OnDestroy()
		{
			DOTween.KillAll();
		}
	}
}
