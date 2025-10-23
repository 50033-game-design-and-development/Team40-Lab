using UnityEngine;

public class Shield : MonoBehaviour
{
    private Transform player;
    private float lifetime;
    [SerializeField] float followSpeed = 10f; // how quickly it sticks to player

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;

        Debug.Log("ASD");

        Destroy(gameObject, 1.0f);
    }

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject, 1.0f);
            return;
        }

        // Smooth follow player
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed * Time.deltaTime);
    }

    // Optional: block or destroy enemy projectiles
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
        }
    }
}
