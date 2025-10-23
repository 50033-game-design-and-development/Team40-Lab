using Unity.VisualScripting;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    #region REFS
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private BossData bossData;
    [SerializeField] private string phaseId = "P1";

    [Header("Hitboxes")]
    [SerializeField] private AttackHitbox[] hitboxes;
    [SerializeField] private float hitboxActiveDuration = 0.3f;

    [Header("Projectile")]
    private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    #endregion

    #region INTERNAL
    [SerializeField] private Transform player;
    private Health health;
    private bool attacking;
    #endregion

    #region UNITY
    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    #endregion

    #region ATTACK CONTROL
    public void PlayAttack(string attackName)
    {
        // Debug.Log($"[{name}] Playing attack: {attackName}");
        if (attacking) return;
        attacking = true;

        if (!anim) return;
        anim.ResetTrigger(attackName);
        anim.SetTrigger(attackName);
    }

    public void TriggerHitbox(string hitboxName)
    {
        if (!bossData) return;
        float dmgMultiplier = bossData.GetDamageMultiplier(phaseId);
        float baseDamage = bossData.meleeDamage * dmgMultiplier;

        foreach (var hb in hitboxes)
        {
            if (!hb || hb.name != hitboxName) continue;
            // Debug.Log($"[{name}] Activating hitbox: {hitboxName} with damage {Mathf.RoundToInt(baseDamage)}");
            float knockback = bossData ? bossData.knockbackForce : 3f;
            hb.Activate(hitboxActiveDuration, Mathf.RoundToInt(baseDamage), gameObject, true, knockback);
            // Debug.Log($"[{name}] Hitbox {hitboxName} activated, hitting {gameObject.name}.");
        }
    }

    public void FireProjectileAtPlayer(Transform player)
    {
        GameObject prefab = bossData.projectilePrefab ? bossData.projectilePrefab : projectilePrefab;
        if (!prefab || !projectileSpawnPoint || !player) return;

        Vector2 dir = (player.position - projectileSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(prefab, projectileSpawnPoint.position, Quaternion.Euler(0f, 0f, angle));

        var script = proj.GetComponent<SwordAuraProjectile>();
        if (script) script.Init(dir, bossData.projectileSpeed, gameObject);
    }


    public void EndAttack()
    {
        attacking = false;
    }

    #endregion
}
