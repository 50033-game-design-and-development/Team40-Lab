using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyMovement : MonoBehaviour
{


    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;

    [SerializeField] Animator animator;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    public Vector3 startPosition = new Vector3(10.0f, 0.0f, 0.0f);
    private AudioSource sfx;

    private bool isStomped = false;
    public bool IsStomped() => isStomped;



    private void OnEnable()
    {
        GameEvents.OnStompDetected += HandleStompDetected;
    }

    private void OnDisable()
    {
        GameEvents.OnStompDetected -= HandleStompDetected;
    }

    private void HandleStompDetected(Collider2D stompedCollider)
    {
        if (isStomped) return;

        if (stompedCollider != null && stompedCollider.transform.IsChildOf(transform))
        {
            Stomp();
        }
    }

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        sfx = GetComponent<AudioSource>();
        originalX = transform.position.x;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        if (isStomped) return;
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    public void Stomp()
    {
        if (isStomped) return;
        isStomped = true;

        sfx.PlayOneShot(sfx.clip);

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        GameEvents.RaiseEnemyStomped(1);

        animator.SetTrigger("onStomp");
        Destroy(gameObject, 0.7f);
    }


    public void GameRestart()
    {
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
    }

}