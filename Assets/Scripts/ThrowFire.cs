using System.Collections;
using UnityEngine;

public class ThrowFire : MonoBehaviour
{
    public GameObject projectilePrefab;  // assign a prefab with Rigidbody2D + Collider2D
    public Transform throwPoint;
    public float interval = 1.0f;
    public Vector2 throwVelocity = new Vector2(-4f, -6f); // left & up
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        StartCoroutine(ThrowLoop());
    }

    IEnumerator ThrowLoop()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (projectilePrefab && throwPoint)
            {
                // Right shot
                var pRight = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
                var rbR = pRight.GetComponent<Rigidbody2D>();
                if (rbR) rbR.linearVelocity = throwVelocity;

                // Left shot
                var pLeft = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
                var rbL = pLeft.GetComponent<Rigidbody2D>();
                if (rbL) rbL.linearVelocity = throwVelocity * new Vector2(-1, 1);
            }
            yield return wait;
        }
    }
}
