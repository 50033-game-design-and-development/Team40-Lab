using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;      // Mario 
    public GameObject platform;
    public float yOffset = 0f;        // lock camera Y
    public float zOffset = -10f;      // keep camera behind
    public float smoothSpeed = 0.125f;

    private float minX, maxX;
    private float halfCamWidth;


    void Start()
    {
        var sr = platform.GetComponent<SpriteRenderer>();
        Bounds bounds = sr.bounds;

        // Get half of the camera's width in world units
        Camera cam = GetComponent<Camera>();
        halfCamWidth = cam.orthographicSize * cam.aspect;

        // Clamp 
        minX = bounds.min.x + halfCamWidth;
        maxX = bounds.max.x - halfCamWidth;

        Debug.Log($"Platform length = {bounds.size.x}, minX={minX}, maxX={maxX}");
    }
    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(
                player.transform.position.x,  // follow only X
                yOffset,                         // fixed Y
                zOffset                          // fixed Z
            );

            float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.x = clampedX;

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }


    }
}
