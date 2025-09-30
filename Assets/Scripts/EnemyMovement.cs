using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool isStomped;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        originalX = transform.position.x;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {// move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         foreach (var contact in collision.contacts)
    //         {
    //             if (contact.normal.y < 0.5f)
    //             {
    //                 Debug.Log("Player stomped the enemy");
    //                 Stomp();
    //                 return;
    //             }
    //         }
    //         FindFirstObjectByType<GameManager>()?.InvokeDeath();
    //     }
    // }

    public void Stomp()
    {
        if (isStomped) return;
        isStomped = true;
        if (enemyBody) enemyBody.linearVelocity = Vector2.zero;
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