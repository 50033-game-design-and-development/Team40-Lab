using UnityEngine;

public class FollowShoot : MonoBehaviour
{
    public Transform target;
    public Vector2 offset = new Vector2(0f, 4f); // hover above/near Mario
    public float followSpeed = 4f;
    public bool followOnlyX = true;

    void Update()
    {
        if (!target) return;

        Vector3 desired = target.position + (Vector3)offset;

        if (followOnlyX)
            desired = new Vector3(desired.x, transform.position.y, transform.position.z);
        else
            desired = new Vector3(desired.x, desired.y, transform.position.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            desired,
            followSpeed * Time.deltaTime
        );
    }
}