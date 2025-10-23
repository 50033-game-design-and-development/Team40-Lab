using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Boss Data", fileName = "NewBossData")]
public class BossData : ScriptableObject
{
    [Header("General Stats")]
    public int maxHealth = 50;
    public float moveSpeed = 3f;

    [Tooltip("Resistance to knockback force (0 = full knockback, 1 = no knockback).")]
    [Range(0f, 1f)] public float knockbackResistance = 0.2f;

    [Header("Attack Settings")]
    public float meleeDamage = 2f;
    public float rangedDamage = 1.5f;
    public float meleeRange = 1.6f;
    public float rangedRange = 6f;
    public float knockbackForce = 5f;
    public float projectileSpeed = 7f;

    [Header("AI Preferences")]
    public bool canReflect = false;
    public bool canShield = false;
    public bool prefersRanged = false;
    public bool prefersMelee = false;
    [Range(0, 1)] public float rangedChance = 0.3f;
    [Range(0, 1)] public float retreatChance = 0.15f;

    [Header("Special Skills")]
    public string[] meleeAttacks;
    public string[] rangedAttacks;
    public GameObject projectilePrefab;

    [Tooltip("Cooldown time between boss attacks.")]
    public float meleeAttackCooldown = 1.2f;
    public float rangedAttackCooldown = 2f;

    [Header("Damage Scaling")]
    [Tooltip("Additional damage multipliers per phase.")]
    public float phase2DamageMultiplier = 1.25f;

    public int GetHealthForPhase(string phaseId)
    {
        return maxHealth;
    }

    public float GetDamageMultiplier(string phaseId)
    {
        switch (phaseId)
        {
            case "P2": return phase2DamageMultiplier;
            default: return 1f;
        }
    }
}
