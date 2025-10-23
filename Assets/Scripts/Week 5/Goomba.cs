using UnityEngine;

public class Goomba : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        MarioController mario = collision.GetComponent<MarioController>();
        if (mario != null)
        {
            mario.gotHit = true;
        }
        Destroy(gameObject);
    }
}
