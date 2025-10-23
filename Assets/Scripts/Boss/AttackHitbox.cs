using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AttackHitbox : MonoBehaviour
{
    GameObject owner;
    int damage;
    bool singleHit = true;
    BoxCollider2D box;
    bool consumed;

    float knockbackForce;

    void Awake() => box = GetComponent<BoxCollider2D>();

    public void Activate(float duration, int dmg, GameObject src, bool single = true, float knockback = 0f)
    {
        consumed = false;
        damage = dmg;
        owner = src;
        singleHit = single;
        knockbackForce = knockback;

        gameObject.SetActive(true);
        box.enabled = true;
        StopAllCoroutines();
        StartCoroutine(DeactivateAfter(duration));

        CheckInitialOverlaps();
    }
    void CheckInitialOverlaps()
    {
        var hits = new System.Collections.Generic.List<Collider2D>();
        int count = box.Overlap(hits);
        for (int i = 0; i < count; i++)
        {
            var other = hits[i];
            if (!other) continue;
            if (other.gameObject == owner) continue;

            var dmg = other.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                dmg.TakeDamage(damage, dir, knockbackForce, owner);
                if (singleHit) consumed = true;
                break;
            }
        }
    }

    System.Collections.IEnumerator DeactivateAfter(float s)
    {
        yield return new WaitForSeconds(s);
        box.enabled = false;
        gameObject.SetActive(false);
        owner = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!owner || consumed) return;
        if (other.gameObject == owner) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            dmg.TakeDamage(damage, dir, 0f);
            if (singleHit) consumed = true;
        }
    }
}
