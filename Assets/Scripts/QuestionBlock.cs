using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    public Animator animator;
    private bool onHitState = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Check if the contact normal points upward (player is below the block)
            if (contact.normal.y > 0.5f)
            {
                animator.SetTrigger("onHit");
                animator.SetBool("hitStatus", true);
                onHitState = true;
                // Spawn item, play sound, etc.
                break;
            }
        }
    }

    public AudioSource coinCollectSound;

    void PlayCoinCollectSound()
    {
        coinCollectSound.PlayOneShot(coinCollectSound.clip);
    }

    public void ResetBlock()
    {
        onHitState = false;
        animator.ResetTrigger("onHit");
        animator.SetBool("hitStatus", false);
    }
}