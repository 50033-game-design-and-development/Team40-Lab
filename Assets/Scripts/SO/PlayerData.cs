using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Health and Mana Settings")]
    public int maxHealth = 10;
    public int maxMana = 10;
    public float manaGainRate = 1f;

    [Header("Movement Settings")]
    public float walkSpeed = 10f;
    public float dropSpeed = 10f;

    [Header("Combat Settings")]
    [Tooltip("Base melee damage of player attacks")]
    public float baseDamage = 1f;

    [Tooltip("Knockback force applied to enemies when hit")]
    public float hitForce = 5f;

    [Tooltip("Time between consecutive melee attacks")]
    public float attackCooldown = 0.5f;

    [Tooltip("Mana cost per spell cast")]
    public float manaSpellCost = 1f;

    [Tooltip("Mana gained per successful hit")]
    public float manaGain = 1f;

    [Tooltip("Fireball prefab used for ranged attacks")]
    public GameObject projectilePrefab;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.04f);
    public LayerMask groundLayer;
    [Tooltip("Time window after leaving ground where jump is still allowed")]
    public float coyoteTime = 0.2f;
    [Tooltip("Time window to buffer jump input before landing")]
    public float jumpBufferTime = 0.2f;
    [Tooltip("Enable double jump ability")]
    public bool canDoubleJump = true;
    [Tooltip("Force applied for the second jump")]
    public float doubleJumpForce = 10f;

    [Header("Attack Settings")]
    // public GameObject ammoPrefab;
    // public Transform ammoSpawnPoint;
    public int ammoPoolSize = 5;
    public float ammoCooldown = 0.5f;
    public int maxAmmo = 5;
    public float reloadTime = 5f;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public bool disableGravityDuringDash = true;

    [Header("Knockback Settings")]
    public float knockbackResistance = 0.5f;
}
