using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ChargeAttack")]
public class ChargeAttackSkill : SkillData
{
    public GameObject chargeProjectile;
    public float chargeTime = 1f;
    public float damage = 10f;
    public float projectileSpeed = 10f;
    public LayerMask enemyLayer;
    public bool isCharging = false;
    public float manaCost = 1f;
    private float chargeTimer;
    private GameObject User;
    private Transform spawnPoint;
    private Animator anim;
    private Health health;
    private Rigidbody2D rb;
    public override void Activate(GameObject user)
    {
        User = user;
        rb = User.GetComponent<Rigidbody2D>();
        health = User.GetComponent<Health>();
        anim = User.GetComponentInChildren<Animator>();
        spawnPoint = User.GetComponent<PlayerController>().projSpawnPoint;
        StartCharge();
    }

    public void StartCharge()
    {
        if (!health.UseMana(manaCost))
        if (isCharging)
        {
            Debug.Log("Currently Charging");
            return;
        }
        rb.linearVelocity = new Vector2(0, 0);
        User.GetComponent<PlayerController>().enabled = false;
        isCharging = true;
        chargeTimer = 0f;
        Debug.Log("Starting Charging Sequence");
        anim.SetTrigger("startCharge");
    }
    
    public void updateChargeTimer()
    {
        if (!isCharging) return;

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeTime)
        {
            Debug.Log("Charging Complete");
            ReleaseCharge();
        }
    }

    public void ReleaseCharge()
    {
        isCharging = false;

        Animator anim = User.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("onRelease");

        if (PlayerSkillManager.Instance != null)
            PlayerSkillManager.Instance.activeChargeSkill = null;
    }
    public void launchChargeProjectile()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("ChargeAttackSkill: No projectileSpawnPoint found!");
            return;
        }

        Debug.Log("ChargeAttack Released");

        GameObject proj = Instantiate(chargeProjectile, spawnPoint.position, Quaternion.identity);
        var talismanProj = proj.GetComponent<TailsmanProjectile>();
        if (talismanProj)
        {
            float facing = User.GetComponent<PlayerController>().Facing;
            talismanProj.Launch(facing, User);
        }
        User.GetComponent<PlayerController>().enabled = true;
        Debug.Log("[ChargeSkill] Projectile spawned and launched.");
    }
}
