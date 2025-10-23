using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class SwordAuraProjectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float lifetime = 3f;
    [SerializeField] int damage = 2;
    [SerializeField] float knockback = 3f;
    [SerializeField] bool destroyOnHit = true;

    Rigidbody2D rb;
    GameObject owner;
    Vector2 initialDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Init(Vector2 dir, float speed, GameObject src)
    {
        owner = src;
        initialDir = dir.normalized;

        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * speed;

        float angle = Mathf.Atan2(initialDir.y, initialDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other || other.gameObject == owner) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            dmg.TakeDamage(damage, dir, knockback, owner);
            if (destroyOnHit) Destroy(gameObject);
        }
    }
}
