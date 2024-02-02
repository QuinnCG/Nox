using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private float initialScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float followSpeed = 5f;

    private Vector2 target;
    private float distanceToPlayer;

    public void SetTarget(Vector2 playerPosition)
    {
        // Use the player's position as the target
        target = playerPosition;
        distanceToPlayer = Vector2.Distance(transform.position, target);
        UpdateScale();
    }

    private void Update()
    {
        // Smoothly move the shadow towards the player's position
        Vector3 targetPosition = new Vector3(target.x, transform.position.y, target.y);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Update the scale based on the distance to the player
        float newDistanceToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), target);
        if (newDistanceToPlayer != distanceToPlayer)
        {
            distanceToPlayer = newDistanceToPlayer;
            UpdateScale();
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






