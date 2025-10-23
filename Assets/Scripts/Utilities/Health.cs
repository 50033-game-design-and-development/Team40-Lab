using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    #region DATA
    [Header("Assign One Data Type")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private BossData bossData;
    [SerializeField] private string bossPhaseId = "P1";

    public PlayerData PlayerData => playerData;
    #endregion

    #region FX & FEEDBACK
    [Header("FX & Feedback")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject deathFX;
    [SerializeField] private bool destroyOnDeath = false;
    #endregion

    #region INVINCIBILITY
    [Header("Invincibility")]
    [SerializeField] private float invincibleDuration = 1f;
    [SerializeField] private float hitFlashSpeed = 10f;
    #endregion

    #region UI
    [Header("UI (Player Only)")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    #endregion

    #region INTERNAL
    private Rigidbody2D rb;
    private Color baseColor = Color.white;

    private int currentHealth;
    private float currentMana;
    private bool dead;
    private bool invincible;
    #endregion

    #region PROPERTIES
    public int CurrentHealth => currentHealth;
    public int MaxHealth => ResolveMaxHealth();
    public int MaxMana => ResolveMaxMana();
    public float Mana => currentMana;
    public bool IsDead => dead;
    public bool Invincible => invincible;

    public event System.Action OnDamaged;
    public event System.Action OnDeath;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr) baseColor = sr.material.color;

        currentHealth = Mathf.Max(1, MaxHealth);
        currentMana = MaxMana;

        UpdateHealthUI();
        UpdateManaUI();
    }

    void Update()
    {
        FlashWhileInvincible();
    }

    #region HEALTH / DAMAGE
    public void TakeDamage(float damage, Vector2 hitDir, float knockback = 0f, GameObject src = null)
    {
        if (dead || invincible) return;

        currentHealth -= Mathf.RoundToInt(damage);
        currentHealth = Mathf.Max(currentHealth, 0);
        OnDamaged?.Invoke();

        if (rb && knockback > 0f)
        {
            StartCoroutine(ApplyKnockbackDelay());
            float resist = ResolveKnockbackResistance();
            Vector2 impulse = hitDir.normalized * knockback * (1f - resist);
            rb.AddForce(impulse, ForceMode2D.Impulse);
            Debug.Log($"[{name}] Received damage {damage}, knockback {impulse}, dir {hitDir}");
        }

        if (src != null && src.TryGetComponent(out PlayerController player))
        {
            Health srcHealth = src.GetComponent<Health>();
            if (srcHealth)
            {
                float reward = srcHealth.playerData ? srcHealth.playerData.manaGainRate : 0.5f;
                srcHealth.GainMana(reward);
                Debug.Log($"[{name}] Hit by Player → Player gains {reward} mana");
            }
        }

        IEnumerator ApplyKnockbackDelay()
        {
            if (rb) rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (TryGetComponent(out BossMovement bossMove)) bossMove.enabled = false;
            if (TryGetComponent(out PlayerController playerCtrl)) playerCtrl.enabled = false;

            yield return new WaitForSeconds(0.2f);

            if (TryGetComponent(out BossMovement bossMove2)) bossMove2.enabled = true;
            if (TryGetComponent(out PlayerController playerCtrl2)) playerCtrl2.enabled = true;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibleRoutine());
        }
    }

    IEnumerator InvincibleRoutine()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        invincible = false;
        if (sr) sr.material.color = baseColor;
    }

    private void FlashWhileInvincible()
    {
        if (sr)
        {
            sr.material.color = invincible
                ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f))
                : baseColor;
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;
        OnDeath?.Invoke();

        if (deathFX)
            Instantiate(deathFX, transform.position, Quaternion.identity);

        if (destroyOnDeath)
            Destroy(gameObject);
        else
            enabled = false;
    }

    public void Heal(float amount)
    {
        if (dead) return;
        currentHealth = Mathf.Min(currentHealth + Mathf.RoundToInt(amount), MaxHealth);
        UpdateHealthUI();
    }
    #endregion

    #region MANA
    private void RegenerateMana()
    {
        if (playerData && !dead)
        {
            currentMana += playerData.manaGainRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, MaxMana);
            UpdateManaUI();
        }
    }

    public bool UseMana(float amount)
    {
        if (currentMana < amount) return false;
        currentMana -= amount;
        UpdateManaUI();
        return true;
    }

    public void GainMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, MaxMana);
        UpdateManaUI();
    }
    #endregion

    #region UI UPDATES
    private void UpdateHealthUI()
    {
        if (healthBar)
            healthBar.fillAmount = (float)currentHealth / MaxHealth;
    }

    private void UpdateManaUI()
    {
        if (manaBar)
            manaBar.fillAmount = currentMana / MaxMana;
    }
    #endregion

    #region DATA HELPERS
    public void SetBossPhase(string phaseId)
    {
        bossPhaseId = phaseId;
        currentHealth = Mathf.Clamp(currentHealth, 1, MaxHealth);
        UpdateHealthUI();
    }

    private int ResolveMaxHealth()
    {
        if (playerData)
            return Mathf.Max(1, playerData.maxHealth);

        if (bossData)
            return Mathf.Max(1, Mathf.RoundToInt(bossData.GetHealthForPhase(bossPhaseId)));

        Debug.LogWarning($"[{name}] Health has no PlayerData or BossData assigned!");
        return 1;
    }

    private int ResolveMaxMana()
    {
        if (playerData)
            return Mathf.Max(1, playerData.maxMana);
        return 1;
    }

    private float ResolveKnockbackResistance()
    {
        if (bossData)
            return bossData.knockbackResistance;
        if (playerData)
            return playerData.knockbackResistance;
        return 0f;
    }
    #endregion

    #region RESET
    public void ResetHealth()
    {
        dead = false;
        invincible = false;

        currentHealth = MaxHealth;
        currentMana = MaxMana;

        UpdateHealthUI();
        UpdateManaUI();

        // Reset sprite color if flashing
        if (sr) sr.material.color = baseColor;

        Debug.Log($"[{name}] Health and Mana have been reset.");
    }
    #endregion

}
