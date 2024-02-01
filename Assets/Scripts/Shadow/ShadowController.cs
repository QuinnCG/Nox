using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private float initialScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float followSpeed = 5f;

    private Vector2 targetPosition;

    public void SetTarget(Vector2 playerPosition)
    {
        // Use the player's position as the target
        targetPosition = playerPosition;
        UpdateScale(Vector2.Distance(transform.position, targetPosition)); // Pass the initial distance
    }

    private void Update()
    {
        if (targetPosition != null)
        {
            // Smoothly move the shadow towards the player's position
            Vector3 newTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.y);
            transform.position = Vector3.Lerp(transform.position, newTargetPosition, followSpeed * Time.deltaTime);

            // Update the scale based on the distance to the player
            float newDistanceToPlayer = Vector3.Distance(transform.position, newTargetPosition);
            if (newDistanceToPlayer != 0f)
            {
                UpdateScale(newDistanceToPlayer);
            }
        }
    }

    private void UpdateScale(float distanceToPlayer)
    {
        // Adjust the scale based on the distance to the player
        float scaleRatio = Mathf.Clamp01(distanceToPlayer / maxScale);
        float newScale = Mathf.Lerp(initialScale, maxScale, scaleRatio);
        transform.localScale = new Vector3(newScale, newScale, 1f);
    }
}





