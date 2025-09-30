using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float maxSpeed = 20f;
    public float upSpeed = 1f;

    [Header("Refs")]
    public UnityEvent onDeath;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public Transform gameCamera;


    [Header("Death")]
    public float deathImpulse = 10f;

    [System.NonSerialized] public bool alive = true;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;

    private bool faceRightState = true;
    private bool onGroundState = true;

    private Vector2 moveVec;
    private bool jumpQueued;

    void OnEnable()
    {
        FindFirstObjectByType<GameManager>()?.playerDeath.AddListener(Die);
    }

    void OnDisable()
    {
        FindFirstObjectByType<GameManager>()?.playerDeath.RemoveListener(Die);
    }

    void Start()
    {
        Application.targetFrameRate = 30;

        marioSprite = GetComponent<SpriteRenderer>();
        marioBody = GetComponent<Rigidbody2D>();
        Time.timeScale = 1f;

        marioAnimator.SetBool("onGround", onGroundState);
    }


    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var c in collision.contacts)
        {
            if (c.normal.y > 0.5f)
            {
                onGroundState = true;
                jumpedState = false;
                marioAnimator.SetBool("onGround", true);
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && alive)
        {
            Die();
        }
    }

    public void Die()
    {
        alive = false;
        marioAnimator.SetTrigger("onDeath");
        marioAudio.PlayOneShot(marioDeath);
        marioBody.linearVelocity = Vector2.zero;
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        FindFirstObjectByType<GameManager>()?.GameOver();
    }

    private bool moving = false;
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }
    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    private bool jumpedState = false;

    public void Jump()
    {
        if (alive && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }

    private void playJumpSound()
    {
        marioAudio.Play();
    }


    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }

    public void GameRestart()
    {
        marioBody.transform.position = new Vector3(-39.26f, -6.38f, 0.0f);
        faceRightState = true;
        marioSprite.flipX = false;
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
    }


    public float GetVerticalVelocity()
    {
        return marioBody.linearVelocity.y;
    }

    public void Bounce(float impulse)
    {
        var v = marioBody.linearVelocity;
        if (v.y < 0f) v.y = 0f;
        marioBody.linearVelocity = v;
        marioBody.AddForce(Vector2.up * impulse, ForceMode2D.Impulse);
    }






}
