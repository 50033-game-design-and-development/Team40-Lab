using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBall : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] float lifetime;

    private Vector2 direction = Vector2.right;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(direction.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Enemy") || _other.CompareTag("Ground"))
        {
            Debug.Log("Kill Teleport");
            Destroy(gameObject);
        }
    }

    // Called by PlayerController to set teleport ball direction
    public void Launch(Vector2 facingDir, float facing)
    {
        float facingDirection = Mathf.Sign(facing);
        if (facingDirection > 0 && facingDir.x == 0 && facingDir.y == 0) direction = Vector2.right;
        else if (facingDir.x == 0 && facingDir.y == 0)
        {
            direction = Vector2.left;
        }
        else
        {
            direction = facingDir;
        }
            
    }

    public void kill()
    {
        Destroy(gameObject);
    }
}
