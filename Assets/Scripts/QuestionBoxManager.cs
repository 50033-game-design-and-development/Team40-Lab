using UnityEngine;
using UnityEngine.UIElements;

public class QuestionBoxManager : MonoBehaviour
{
    public Animator boxAnimator;
    public Animator coinAnimator;
    public AudioSource audioSource;
    public AudioClip coinSound;

    Rigidbody2D rb;
    SpringJoint2D boxSpring;
    private Vector3 startPos;
    private bool hit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxAnimator = GetComponent<Animator>();
        boxSpring = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        startPos = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    if (!hit)
                    {
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                        if (boxSpring) boxSpring.enabled = true;
                        rb.AddForce(Vector2.up * 20f, ForceMode2D.Impulse);
                        hit = true;
                        boxAnimator.SetBool("isHit", true);

                        if (coinAnimator)
                            coinAnimator.SetTrigger("popCoin");

                        // play sound
                        if (audioSource && coinSound)
                            audioSource.PlayOneShot(coinSound);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (hit && boxSpring != null)
        {
            if (Mathf.Abs(transform.position.y - startPos.y) < 0.01f && rb.linearVelocity.magnitude < 0.01f)
            {
                Destroy(boxSpring);

                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                transform.position = startPos;
            }
        }
    }

    public void ResetBox()
    {
        hit = false;
        if (boxAnimator) boxAnimator.SetBool("isHit", false);
        if (coinAnimator)
            coinAnimator.ResetTrigger("popCoin");

        boxAnimator = GetComponent<Animator>();
        boxSpring = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        startPos = transform.position;
    }
}


