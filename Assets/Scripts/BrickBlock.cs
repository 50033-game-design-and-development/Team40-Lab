using UnityEngine;

public class BrickBlock : MonoBehaviour
{
    public Animator animator;
    public bool hasCoin = true;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetBool("hasCoin", hasCoin);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Check if the contact normal points upward (player is below the block)
            if (contact.normal.y > 0.5f)
            {
                animator.SetTrigger("onHit");
                // Spawn item, play sound, etc.
                break;
            }
        }
    }

    public void OnCoinAnimationEnd() {
        if (hasCoin)
        {
            hasCoin = false;
            animator.SetBool("hasCoin", false);
        }
    }
    

    public AudioSource coinCollectSound;

    void PlayCoinCollectSound()
    {
        coinCollectSound.PlayOneShot(coinCollectSound.clip);
    }
}