using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float health = 3f;

    [Header("Recoil")]
    [SerializeField] float recoilForce = 5f;
    [SerializeField] float recoilDuration = 0.2f;
    [SerializeField] float damage;
    private bool isRecoiling = false;
    private float recoilTimer = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        // Handle recoil timer
        if (isRecoiling)
        {
            recoilTimer += Time.deltaTime;
            if (recoilTimer >= recoilDuration)
            {
                isRecoiling = false;
                recoilTimer = 0f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Apply recoil when hit
        if (!isRecoiling && PlayerController.Instance != null)
        {
            ApplyRecoil();
        }
    }

    private void ApplyRecoil()
    {
        isRecoiling = true;
        recoilTimer = 0f;

        // Calculate direction away from player
        Vector2 playerPos = PlayerController.Instance.transform.position;
        Vector2 enemyPos = transform.position;
        Vector2 recoilDirection = (enemyPos - playerPos).normalized;

        // Apply recoil force
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset velocity first
            rb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
        }
    }
    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            Debug.Log("Enemy Attacking Player");
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.5f);
        }
    }
    protected virtual void Attack()
    {
        return;
    }
}
