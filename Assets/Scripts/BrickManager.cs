using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [Header("Coin")]
    public Animator coinAnimator;
    public Animator boxAnimator;
    public AudioSource audioSource;
    public AudioClip coinSound;

    [Header("Bounce Tuning")]
    public float bounceImpulse = 10f;
    public float hitCooldown = 0.08f;

    Rigidbody2D rb;
    SpringJoint2D boxSpring;
    Vector3 startPos;

    bool coinSpawned = false;
    public bool haveCoin = true;

    void Start()
    {
        boxSpring = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        startPos = transform.position;

        if (boxSpring) boxSpring.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {

                boxAnimator.SetTrigger("onHit");

                if (!coinSpawned && haveCoin)
                {
                    coinSpawned = true;

                    if (coinAnimator)
                        coinAnimator.SetTrigger("popCoin");

                    if (audioSource && coinSound)
                        audioSource.PlayOneShot(coinSound);
                }


                break;
            }
        }
    }

    void Update()
    {

    }
}
