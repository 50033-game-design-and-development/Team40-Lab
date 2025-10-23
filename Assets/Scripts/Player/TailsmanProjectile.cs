using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class TailsmanProjectile : MonoBehaviour
{
    [Header("Prefabs")]

    [Header("Stats")]
    [SerializeField] float damage = 1f;
    [SerializeField] float knockbackForce = 2f;
    [SerializeField] float speed = 8f;
    [SerializeField] float lifetime = 1f;

    [Header("Homing Settings")]
    [SerializeField] bool hasHoming = false;
    [SerializeField] float turnSpeed = 360f;
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] LayerMask targetLayers;

    Rigidbody2D rb;
    float direction = 1f;
    GameObject owner;
    Transform target;
    PlayerData data;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        if (hasHoming && target == null)
            AcquireTarget();

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (hasHoming && target)
        {
            HomingMovement();
        }
        else
        {
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }
    }

    public void Launch(float facing, GameObject src)
    {
        owner = src;
        direction = Mathf.Sign(facing);
        rb.linearVelocity = new Vector2(direction * speed, 0f);

        var sr = GetComponent<SpriteRenderer>();
        if (sr && direction < 0) sr.flipX = true;
    }

    public void SetStats(float dmg, float knockback, float spd, float life, float tSpeed = 360f, float detectRad = 10f, bool homing = false, GameObject projOwner = null, PlayerData playerData = null)
    {
        damage = dmg;
        knockbackForce = knockback;
        speed = spd;
        lifetime = life;
        turnSpeed = tSpeed;
        detectionRadius = detectRad;
        hasHoming = homing;
        owner = projOwner;
        data = playerData;
    }

    void AcquireTarget()
    {
        LayerMask mask = CompareTag("PlayerProjectile") 
            ? LayerMask.GetMask("Enemy") 
            : LayerMask.GetMask("Player");

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, mask);
        if (hit)
        {
            var bossMovement = hit.GetComponent<BossMovement>();
            target = (bossMovement && bossMovement.VisualRoot != null) ? bossMovement.VisualRoot : hit.transform;
            Debug.Log($"{gameObject.name} acquired target: {target.name}", target);
        }
    }

    void HomingMovement()
    {
        Vector2 toTarget = ((Vector2)target.position - rb.position).normalized;
        Vector2 currentDir = rb.linearVelocity != Vector2.zero ? rb.linearVelocity.normalized : toTarget;

        float maxRadiansDelta = turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        Vector3 cur3 = new Vector3(currentDir.x, currentDir.y, 0f);
        Vector3 tgt3 = new Vector3(toTarget.x, toTarget.y, 0f);
        Vector3 newDir3 = Vector3.RotateTowards(cur3, tgt3, maxRadiansDelta, 0f);
        Vector2 newDir = new Vector2(newDir3.x, newDir3.y).normalized;

        rb.linearVelocity = newDir * speed;

        float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
        rb.SetRotation(angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Projectile] Trigger entered {other.name} (this={tag}, other={other.tag})");

        if ((CompareTag("PlayerProjectile") && other.CompareTag("Enemy")) ||
            (CompareTag("EnemyProjectile") && other.CompareTag("Player")))
        {
            HitTarget(other);
        }
        else if (other.CompareTag("PlayerProjectile") || other.CompareTag("EnemyProjectile"))
        {
            return;
        }
    }

    void HitTarget(Collider2D other)
    {
        var dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            Vector2 hitDir = (other.transform.position - transform.position).normalized;
            dmg.TakeDamage(damage, hitDir, knockbackForce, gameObject);
        }

        if (owner && owner.TryGetComponent(out Health health))
        {
            float manaReward = data ? data.manaGainRate : 1f;
            health.GainMana(manaReward);
        }

        Destroy(gameObject);
    }

    public void Reflect()
    {
        direction = -direction;
        rb.linearVelocity = new Vector2(direction * speed, 0f);

        var sr = GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.flipX = !sr.flipX;
            sr.color = Color.red;
        }

        gameObject.tag = "EnemyProjectile";
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        owner = null;

        transform.position += (Vector3)(Vector2.right * direction * 0.2f);

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        StartCoroutine(TemporarilyDisableCollider());
        Destroy(gameObject, lifetime);
    }

    IEnumerator TemporarilyDisableCollider()
    {
        var col = GetComponent<Collider2D>();
        if (col)
        {
            col.enabled = false;
            yield return new WaitForSeconds(0.1f);
            col.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hasHoming)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
