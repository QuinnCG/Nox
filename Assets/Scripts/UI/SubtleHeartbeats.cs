using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Game.UI
{
	public class SubtleHeartbeats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public AnimationCurve curve;
		public float minScale = 0.9f;
		public float maxScale = 1.1f;
		public float pulseDuration = 1f;

		private Vector3 originalScale;

		void Start()
		{
			originalScale = transform.localScale;
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			EnlargeOnHover();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ShrinkOnHoverEnd();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			StartSubtlePulsating();
		}

		void EnlargeOnHover()
		{
			transform.DOScale(maxScale, 0.2f);
		}

		void ShrinkOnHoverEnd()
		{
			transform.DOKill();
			transform.DOScale(originalScale, 0.2f);
		}

		void StartSubtlePulsating()
		{
			transform.DOScale(maxScale, pulseDuration)
				.SetEase(curve)
				.SetLoops(-1);
		}
	}
}
