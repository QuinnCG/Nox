using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

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
        Debug.Log("Clicked!");
        StartSubtlePulsating();
    }

    void EnlargeOnHover()
    {
        transform.DOScale(maxScale, 0.2f);
    }

    void ShrinkOnHoverEnd()
    {
        transform.DOKill();
        Debug.Log("DOKill");
        transform.DOScale(originalScale, 0.2f);
    }

    void StartSubtlePulsating()
    {
        transform.DOScale(maxScale, pulseDuration)
            .SetEase(curve)
            .SetLoops(-1);
    }
}
