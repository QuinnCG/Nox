using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private float initialScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float followSpeed = 5f;

    private Transform target;
    private float distanceToPlayer;

    public void SetTarget(Vector2 targetPosition)
    {
        // Convert Vector2 to a new Transform with the given position
        target = new GameObject("ShadowTarget").transform;
        target.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.y);

        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        UpdateScale();
    }

    private void Update()
    {
        if (target != null)
        {
            // Smoothly move the shadow towards the player's position
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Update the scale based on the distance to the player
            float newDistanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (newDistanceToPlayer != distanceToPlayer)
            {
                distanceToPlayer = newDistanceToPlayer;
                UpdateScale();
            }
        }
    }

    private void UpdateScale()
    {
        // Adjust the scale based on the distance to the player
        float scaleRatio = Mathf.Clamp01(distanceToPlayer / maxScale);
        float newScale = Mathf.Lerp(initialScale, maxScale, scaleRatio);
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}


