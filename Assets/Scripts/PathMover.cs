using UnityEngine;

public class PathMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public bool loop = true;
    int i = 0;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();   // cache once
    }


    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[i];

        if (target.position.x < transform.position.x)
        {
            sr.flipX = false;   // moving left
        }
        else if (target.position.x > transform.position.x)
        {
            sr.flipX = true;  // moving right
        }

        // --- Move towards target ---
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            i++;
            if (i >= waypoints.Length)
            {
                if (loop)
                {
                    i = 0;
                }
                else
                {
                    i = waypoints.Length - 1;
                }
            }
        }
    }
}

