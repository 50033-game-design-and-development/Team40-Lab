using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0;
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public TextMeshProUGUI[] scoreTexts;
    public GameObject enemies;
    public GameObject gameOverUI;
    public GameObject gameUI;
    private bool gameOverState = false;

    public Camera mainCamera;
    private UnityEngine.Vector3 startPosition;
    private UnityEngine.Quaternion startRotation;

    private UnityEngine.Vector3 startCameraPos;
    // Start is called before the first frame update
    void Start()
    {
        marioSprite = GetComponent<SpriteRenderer>();
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();

        onGroundState = true;
        marioAnimator.SetBool("onGround", onGroundState);

        startPosition = transform.position;
        startRotation = transform.rotation;

        // camera initial state

        startCameraPos = mainCamera.transform.position;

        marioAnimator.SetBool("onGround", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x > -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    public float upSpeed = 10;
    private bool onGroundState = true;
    int collisionLayerMask = (1 << 3 | 1 << 6 | 1 << 7); 

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.gameObject.layer)) > 0) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    public float maxSpeed = 20;

    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(moveHorizontal) > 0)
            {
                UnityEngine.Vector2 movement = new UnityEngine.Vector2(moveHorizontal, 0);
                // check if it doesn't go beyond maxSpeed
                if (marioBody.linearVelocity.magnitude < maxSpeed)
                    marioBody.AddForce(movement * speed);
            }

            // stop
            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                // stop
                marioBody.linearVelocity = UnityEngine.Vector2.zero;
            }

            if (Input.GetKeyDown("space") && onGroundState)
            {
                marioBody.AddForce(UnityEngine.Vector2.up * upSpeed, ForceMode2D.Impulse);
                onGroundState = false;
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemies") && alive)
        {
            Debug.Log("Collided with Goomba!");

            // play death animation
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
        }
    }

    public void RestartButtonCallback(int input)
    {
        if (gameOverState == true)
        {
            gameOverState = false;
        }
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    public JumpOverGoomba jumpOverGoomba;

    private void ResetGame()
    {
        // reset position
        marioBody.transform.position = startPosition;
        marioBody.transform.rotation = startRotation;
        // reset velocity
        marioBody.linearVelocity = UnityEngine.Vector2.zero;
        marioBody.angularVelocity = 0f;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        jumpOverGoomba.score = 0;
        jumpOverGoomba.ResetScore();
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        // reset UI
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);

        // reset camera position
        mainCamera.transform.position = startCameraPos;

        //reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        foreach (var block in UnityEngine.Object.FindObjectsByType<QuestionBlock>(FindObjectsSortMode.None))
        {
            block.ResetBlock();
        }
        foreach (var block in UnityEngine.Object.FindObjectsByType<BrickBlock>(FindObjectsSortMode.None))
        {
            block.ResetBlock();
        }
    }

    //animators

    public Animator marioAnimator;

    // for audio
    public AudioSource marioAudio;

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    public AudioClip marioDeath;
    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;
    void PlayDeathImpulse()
    {
        marioBody.AddForce(UnityEngine.Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
    public AudioSource gameOverSong;
    void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;
        // set gameover scene
        // change the UI
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
        // play death song
        gameOverSong.PlayOneShot(gameOverSong.clip);
    }
}

