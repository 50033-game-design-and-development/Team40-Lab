using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody2D marioBody;
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public JumpOverGoomba jumpOverGoomba;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    private bool isGameOver = false;
    public Animator marioAnimator;

    public AudioSource marioAudio;

    public AudioClip marioDeath;
    public float deathImpulse = 10;
    // state
    [System.NonSerialized]
    public bool alive = true;



    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioSprite = GetComponent<SpriteRenderer>();
        marioBody = GetComponent<Rigidbody2D>();

        if (gameOverPanel) gameOverPanel.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1.0f; // resume time

        marioAnimator.SetBool("onGround", onGroundState);

    }

    // Update is called once per frame
    void Update()
    {
        // flip the sprite
        if (Input.GetKeyDown("a") && faceRightState)
        {
            marioSprite.flipX = true;
            faceRightState = false;
            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }
        else if (Input.GetKeyDown("d") && !faceRightState)
        {
            marioSprite.flipX = false;
            faceRightState = true;
            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (isGameOver && Input.GetKeyDown("r"))
        {
            RestartButtonCallback();
            return;
        }
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));

    }


    public float maxSpeed = 20;
    public float upSpeed = 1;
    private bool onGroundState = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacles") && !onGroundState)
        {
            onGroundState = true;
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void gameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0.0f; // pause time
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (gameOverScoreText) gameOverScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Game Over");
            GameManager.Instance?.RegisterDeath();

            marioAnimator.Play("Death");
            alive = false;

        }

    }
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (!alive) return;
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            marioBody.linearVelocity = Vector2.zero;
        }
        // jump
        if (Input.GetKeyDown(KeyCode.Space) && onGroundState)
        {
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGroundState = false;
            marioAnimator.SetBool("onGround", onGroundState);
        }


    }

    public void RestartButtonCallback()
    {
        Debug.Log("Restart!");
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // reset everything
        ResetGame();

        isGameOver = false;
        Time.timeScale = 1.0f;

        GameManager.Instance?.TrySpawnThrower();
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.position = new Vector3(-41.81f, -6.27f, 0.0f);
        faceRightState = true;
        marioSprite.flipX = false;

        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        jumpOverGoomba.score = 0;

        var throwers = GameObject.FindGameObjectsWithTag("Thrower");
        if (throwers.Length > 0)
        {
            foreach (var t in throwers) Destroy(t);
        }

        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        var boxes = FindObjectsByType<QuestionBoxManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var box in boxes)
            box.ResetBox();

    }

    void PlayDeathImpulse()
    {
        marioAudio.PlayOneShot(marioDeath);
        marioBody.linearVelocityX = 0f;
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }


}