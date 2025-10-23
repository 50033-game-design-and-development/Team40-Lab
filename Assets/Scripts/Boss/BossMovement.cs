using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class BossMovement : MonoBehaviour, IDamageable
{
    #region REFS
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform visualRoot;
    public Transform VisualRoot => visualRoot;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private BossData bossData;
    [SerializeField] private string phaseId = "P1";

    [Header("Attack Controller")]
    [SerializeField] private BossAttackController attackCtrl;
    [SerializeField] float decisionInterval = 0.8f;
    private string[] meleeAttacks => bossData ? bossData.meleeAttacks : null;
    private string[] rangedAttacks => bossData ? bossData.rangedAttacks : null;

    [Header("Reflect Settings")]
    [SerializeField] float reflectDuration = 0.55f;
    [SerializeField] float detectRadius = 5f;
    [SerializeField] float reflectCooldown = 3f;
    [SerializeField] Transform reflectZone;
    bool reflecting;
    float reflectEndTime;
    float lastReflectTime = 0f;
    public bool IsReflecting => reflecting;

    #endregion

    #region GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region INTERNAL
    private Rigidbody2D rb;
    private Health health;
    private int facing = -1;
    private bool grounded;
    private bool isAttacking;
    private bool initialized;
    private float nextAttackTime;
    private float spawnTime;
    private const float firstFlipLock = 0.08f;
    private Health playerHealth;
    private bool playerDead;
    Collider2D reflectCollider;

    float rangedAttackCooldown;
    float meleeAttackCooldown;
    float lastDecisionTime;

    enum BossState { Idle, Chase, Melee, Ranged }
    BossState currentState = BossState.Idle;

    // cached initial transforms
    private Vector3 initVisualScale, initVisualLocalPos;
    private Vector2 initColliderOffset;
    private Vector3 initGroundLocalPos;
    #endregion

    #region UNITY
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        if (bossData) health.SetBossPhase(phaseId);

        // rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (anim) anim.applyRootMotion = false;
        if (visualRoot)
        {
            initVisualScale = visualRoot.localScale;
            initVisualLocalPos = visualRoot.localPosition;
        }
        if (bodyCollider) initColliderOffset = bodyCollider.offset;
        if (groundCheck) initGroundLocalPos = groundCheck.localPosition;

        spawnTime = Time.time;
        initialized = false;

        meleeAttackCooldown = bossData ? bossData.meleeAttackCooldown : 1.2f;
        rangedAttackCooldown = bossData ? bossData.rangedAttackCooldown : 2.5f;

        if (reflectZone)
            reflectCollider = reflectZone.GetComponent<Collider2D>();
        if (reflectCollider)
            reflectCollider.enabled = false;
    }

    void Start()
    {
        if (player)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth)
                playerHealth.OnDeath += OnPlayerDeath;
        }
    }

    void OnDestroy()
    {
        if (playerHealth)
            playerHealth.OnDeath -= OnPlayerDeath;
    }

    void Update()
    {
        if (!player || health.IsDead) return;

        grounded = groundCheck && Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (!initialized)
        {
            facing = visualRoot && visualRoot.localScale.x >= 0 ? 1 : -1;
            initialized = true;
        }

    }

    void FixedUpdate()
    {
        if (playerDead || !player || health.IsDead) return;

        if (reflecting)
        {
            rb.linearVelocity = Vector2.zero;
            anim?.SetBool("isMoving", false);
            anim?.SetFloat("xSpeed", 0f);
            return;
        }

        if (bossData.canReflect && !reflecting && Time.time - lastReflectTime > reflectCooldown && IsPlayerProjectileNear())
        {
            rb.linearVelocity = Vector2.zero;
            anim?.SetFloat("xSpeed", 0f);
            TryReflect();
            lastReflectTime = Time.time;
            return;
        }

        float diffX = player.position.x - transform.position.x;
        int desiredFacing = diffX > 0 ? 1 : -1;
        if (desiredFacing != facing) SetFacing(desiredFacing);

        if (isAttacking) return;

        float dist = Vector2.Distance(player.position, transform.position);
        float meleeRange = bossData ? bossData.meleeRange : 1.5f;
        float rangedRange = bossData ? bossData.rangedRange : 6f;
        float moveSpeed = bossData ? bossData.moveSpeed : 3f;

        if (Time.time - lastDecisionTime > decisionInterval)
        {
            lastDecisionTime = Time.time;

            // Force immediate melee attack if player is very close
            if (dist <= meleeRange * 1.1f)
            {
                currentState = BossState.Melee;
            }
            else
            {
                DecideNextState(dist, meleeRange, rangedRange);
            }
        }
        switch (currentState)
        {
            case BossState.Chase:
                ChasePlayer(moveSpeed);
                break;

            case BossState.Melee:
                rb.linearVelocity = Vector2.zero;
                anim?.SetFloat("xSpeed", 0f);
                TryAttack(dist);
                break;

            case BossState.Ranged:
                rb.linearVelocity = Vector2.zero;
                anim?.SetFloat("xSpeed", 0f);
                TryAttack(dist);
                break;

            default:
                rb.linearVelocity = Vector2.zero;
                anim?.SetFloat("xSpeed", 0f);
                break;
        }

        if (reflecting && Time.time >= reflectEndTime)
            StopReflect();
    }

    bool IsPlayerProjectileNear()
    {

        Collider2D proj = Physics2D.OverlapCircle(
            transform.position,
            detectRadius,
            LayerMask.GetMask("PlayerProjectile")
        );

        return proj != null;
    }

    void DecideNextState(float dist, float meleeRange, float rangedRange)
    {
        float rangedChance = bossData ? bossData.rangedChance : 0.25f;
        float retreatChance = bossData ? bossData.retreatChance : 0.15f; // chance to do ranged instead of melee

        if (bossData && bossData.prefersRanged) // If the boss prefers ranged attacks
            rangedChance += 0.3f;

        if (bossData && bossData.prefersMelee)  // If the boss prefers melee attacks
            retreatChance = Mathf.Max(0, retreatChance - 0.1f);

        if (dist > rangedRange) // Too far, must chase
        {
            currentState = BossState.Chase;
            return;
        }

        if (dist > meleeRange && dist <= rangedRange) // In ranged range
        {
            currentState = (Random.value < rangedChance)
                ? BossState.Ranged
                : BossState.Chase;
            return;
        }

        if (dist <= meleeRange) // In melee range
        {
            currentState = (Random.value < retreatChance)
                ? BossState.Ranged
                : BossState.Melee;
        }
    }

    #endregion

    #region MOVEMENT
    void ChasePlayer(float moveSpd)
    {
        if (!player) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        Vector2 vel = rb.linearVelocity;
        vel.x = Mathf.Lerp(vel.x, dir * moveSpd, 5f * Time.fixedDeltaTime);
        rb.linearVelocity = vel;

        anim?.SetBool("isMoving", true);
        anim?.SetFloat("xSpeed", Mathf.Abs(vel.x));
        anim?.SetBool("onGround", grounded);
    }

    public void SetPhase(string newPhaseId)
    {
        phaseId = newPhaseId;
        Debug.Log($"[Boss] Phase changed to {phaseId}");
    }
    #endregion

    #region ATTACKS
    void TryAttack(float distanceToPlayer)
    {
        if (!attackCtrl || isAttacking || playerDead) return;

        float meleeRange = bossData ? bossData.meleeRange : 1.5f;
        float rangedRange = bossData ? bossData.rangedRange : 6f;

        if (Time.time < nextAttackTime) return;

        if (distanceToPlayer <= meleeRange && meleeAttacks.Length > 0)
        {
            string chosen = meleeAttacks[Random.Range(0, meleeAttacks.Length)];
            StartAttack(chosen, meleeAttackCooldown);
            nextAttackTime = Time.time + meleeAttackCooldown;
        }
        else if (distanceToPlayer > meleeRange && distanceToPlayer <= rangedRange && rangedAttacks.Length > 0)
        {
            string chosen = rangedAttacks[Random.Range(0, rangedAttacks.Length)];
            StartAttack(chosen, rangedAttackCooldown);
            nextAttackTime = Time.time + rangedAttackCooldown;
        }
    }
    void StartAttack(string attackName, float cooldown)
    {
        StartCoroutine(DoAttack(attackName, cooldown));
    }

    IEnumerator DoAttack(string attackName, float cooldown)
    {
        isAttacking = true;
        Debug.Log($"[Boss] Starting attack: {attackName}");

        if (attackName == "Slash")
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * 5f, rb.linearVelocity.y);
            yield return new WaitForSeconds(0.15f); // dash for 0.15s
        }

        rb.linearVelocity = Vector2.zero;
        anim?.SetFloat("xSpeed", 0f);
        attackCtrl.PlayAttack(attackName);
        nextAttackTime = Time.time + cooldown;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public virtual void TryReflect()
    {
        if (!bossData || !bossData.canReflect) return;
        if (reflecting) return;

        StartReflect();
    }

    void StartReflect()
    {
        reflecting = true;
        reflectEndTime = Time.time + reflectDuration;

        if (reflectCollider) reflectCollider.enabled = true;

        rb.linearVelocity = Vector2.zero;
        anim?.SetFloat("xSpeed", 0f);

        anim?.SetTrigger("Reflect");
    }

    void StopReflect()
    {
        reflecting = false;
        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
        if (reflectCollider) reflectCollider.enabled = false;
    }

    public void AE_StartReflect() => TryReflect();
    public void AE_StopReflect() => StopReflect();

    void TryShield() { /* handled by animation or AI trigger */ }

    private void OnPlayerDeath()
    {
        playerDead = true;
        rb.linearVelocity = Vector2.zero;
        anim?.SetBool("isMoving", false);
        anim?.SetFloat("xSpeed", 0f);
        Debug.Log("[Boss] Player is dead — stopping combat.");
    }

    #endregion

    #region FACING
    void SetFacing(int f)
    {
        if (f == facing) return;
        facing = f;

        if (visualRoot)
        {
            visualRoot.localScale = new Vector3(Mathf.Abs(initVisualScale.x) * f, initVisualScale.y, initVisualScale.z);
            visualRoot.localPosition = initVisualLocalPos;
        }

        if (bodyCollider)
            bodyCollider.offset = new Vector2(initColliderOffset.x * -f, initColliderOffset.y);

        if (groundCheck)
            groundCheck.localPosition = new Vector3(Mathf.Abs(initGroundLocalPos.x) * f, initGroundLocalPos.y, initGroundLocalPos.z);
    }

    public int FacingSign => facing;
    #endregion

    #region DAMAGE
    public void TakeDamage(float amount, Vector2 dir, float knockback = 0f, GameObject src = null)
    {
        health.TakeDamage(amount, dir, knockback, src);
        anim?.SetTrigger("Hit");
    }

    public bool IsDead => health.IsDead;
    #endregion

    #region GIZMOS
    void OnDrawGizmos()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (detectRadius > 0f)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
    #endregion
}