using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SubtleHeartbeats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public AnimationCurve curve;
    public float minScale = 0.06f;
    public float maxScale = 0.09f;
    public float pulseDuration = 0.5f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        StartSubtlePulsating();
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        StopPulsating();
    }

    void StartSubtlePulsating()
    {
        transform.DOScale(maxScale, pulseDuration)
            .SetEase(curve)
            .SetLoops(-1, LoopType.Restart)
            .SetId("SubtlePulsatingTween");
    }

    void StopPulsating()
    {

        DOTween.Kill("SubtlePulsatingTween");
        transform.DOScale(originalScale, 0.2f);
    }
}
