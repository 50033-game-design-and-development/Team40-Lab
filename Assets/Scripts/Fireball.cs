using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifeTime = 2.0f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
