using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReflectZone : MonoBehaviour
{
    [SerializeField] BossMovement boss;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!boss || !boss.IsReflecting) return;

        if (other.CompareTag("PlayerProjectile"))
        {
            if (other.TryGetComponent(out TailsmanProjectile proj))
            {
                Debug.Log("Reflecting Tailsman Projectile");
                proj.Reflect();
            }
        }
    }

}
