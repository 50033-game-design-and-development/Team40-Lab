using UnityEngine;

public class BossAERelay : MonoBehaviour
{
    [SerializeField] BossAttackController bossAttack;

    public void TriggerHitbox(string attackName)
    {
        if (bossAttack)
        {
            bossAttack.TriggerHitbox(attackName);
        }

    }
    public void EndAttack()
    {
        var bossMovement = GetComponentInParent<BossMovement>();
        if (bossMovement != null)
            bossMovement.EndAttack();

        var bossAttackCtrl = GetComponentInParent<BossAttackController>();
        if (bossAttackCtrl != null)
            bossAttackCtrl.EndAttack();
    }

    public void SwordAura()
    {
        if (!bossAttack) return;
        var player = GameObject.FindGameObjectWithTag("Player")?.transform;
        bossAttack.FireProjectileAtPlayer(player);
    }

    public void StartReflect()
    {
        var bossMovement = GetComponentInParent<BossMovement>();
        if (bossMovement != null)
            bossMovement.AE_StartReflect();
    }

    public void StopReflect()
    {
        var bossMovement = GetComponentInParent<BossMovement>();
        if (bossMovement != null)
            bossMovement.AE_StopReflect();
    }
}
