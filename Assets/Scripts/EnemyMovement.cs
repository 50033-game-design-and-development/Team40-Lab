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

    private bool isStomped = false;
    public bool IsStomped() => isStomped;
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

    public void Stomp()
    {
        if (isStomped) return;
        isStomped = true;
        if (enemyBody)
        {
            enemyBody.linearVelocity = Vector2.zero;
            enemyBody.constraints = RigidbodyConstraints2D.FreezeAll; //jank way to stop movement
        }
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;
        animator.SetTrigger("onStomp");
        //gameObject.layer = LayerMask.NameToLayer("DeadEnemy"); //change layer to not have collision with player
        Destroy(gameObject, 0.7f);
    }

    // public void Stomp() //Player facing collider
    // {
    //     if (isStomped) return;
    //     isStomped = true;

    //     if (enemyBody) enemyBody.linearVelocity = Vector2.zero;
    //     animator.SetTrigger("onStomp");

    //     // Disable only the collider that interacts with the player
    //     Collider2D hitbox = GetComponent<Collider2D>();
    //     if (hitbox != null) hitbox.enabled = false;

    //     // Keep a ground collider (like a BoxCollider2D on child object)
    //     Destroy(gameObject, 0.7f);
    // }


    public void GameRestart()
    {
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
    }

}