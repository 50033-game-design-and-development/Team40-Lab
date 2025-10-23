using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [SerializeField] private GameEvent gameRestartEvent;

    #region PLAYER DATA
    [Header("Player Data")]
    [SerializeField] private PlayerData data;
    [SerializeField] private Health health; // handles HP + Mana
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameManager gameManager;

    #endregion

    #region COMPONENTS
    [Header("Player Components")]
    [SerializeField] public  Animator anim;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] public Transform groundCheckPoint;
    [SerializeField] private Transform projectileSpawnPoint;
    public Transform projSpawnPoint => projectileSpawnPoint;
    private Rigidbody2D rb;
    #endregion

    #region ATTACK
    [Header("Attack Settings")]
    [SerializeField] private Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] private Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    private float Damage => data ? data.baseDamage : 1f;
    private float HitForce => data ? data.hitForce : 5f;
    private float AttackCooldown => data ? data.attackCooldown : 0.5f;
    private float ManaSpellCost => data ? data.manaSpellCost : 0.3f;
    private float ManaGain => data ? data.manaGain : 0.1f;
    private GameObject ProjectilePrefab => data ? data.projectilePrefab : null;
    private float timeSinceAttack = 0f;
    private bool isAttacking = false;
    #endregion

    #region PASSIVES
    public bool hasHoming = false;
    #endregion

    #region SKILL
    private SkillData currentActiveSkill;
    #endregion

    #region INTERNAL STATES
    // Movement
    private Vector2 moveInput;
    private float facing = 1f;
    public float Facing => facing;

    // State Flags
    private bool isDashing = false;
    private bool canDash = true;
    private bool isJumping = false;

    // Jump System
    private float lastGroundedTime = -1f;
    private float lastJumpInputTime = -1f;
    private bool hasDoubleJumped = false;

    // Physics
    private float defaultGravityScale;

    // Time Manipulation
    private bool restoreTime;
    private float restoreTimeSpeed;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        if (!health) health = GetComponent<Health>();
    }

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;

        // Subscribe to health events
        health.OnDamaged += OnPlayerDamaged;
        health.OnDeath += OnPlayerDeath;
    }

    private void Update()
    {
        UpdateGroundedState();
        UpdateFacingDirection();
        UpdateAnimator();
        UpdateTimers();

        CheckJumpBuffer();
        RestoreTimeScale();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    private void OnEnable()
    {
        if (gameRestartEvent != null)
            gameRestartEvent.RegisterListener(RestartPlayer);
    }

    private void OnDisable()
    {
        if (gameRestartEvent != null)
            gameRestartEvent.UnregisterListener(RestartPlayer);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SceneTransition"))
        {
            SceneManager.LoadScene("Lab5-2");
        }
    }
    #endregion

    #region HEALTH CALLBACKS
    private void OnPlayerDamaged()
    {
        anim?.SetTrigger("onHit");
    }

    private void OnPlayerDeath()
    {
        anim?.SetTrigger("onDie");
        enabled = false; // disable player control
        rb.linearVelocity = Vector2.zero;
    }

    public void AE_GameOver()
    {
        gameManager.GameOver();
    }
    #endregion

    #region TIME MANIPULATION
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    private IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    private void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1f;
                restoreTime = false;
            }
        }
    }
    #endregion

    #region MOVEMENT
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        float targetVelocityX = moveInput.x * data.walkSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // Fast fall when pressing down
        if (moveInput.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -data.dropSpeed);
        }
    }
    #endregion

    #region JUMP
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || isDashing)
            return;

        lastJumpInputTime = Time.time;

        if (CanJump())
            Jump();
        else if (CanDoubleJump())
            DoubleJump();
    }

    private void CheckJumpBuffer()
    {
        if (Time.time < lastJumpInputTime + data.jumpBufferTime && CanJump())
        {
            Jump();
        }
    }

    private bool CanJump()
    {
        return IsGrounded() || Time.time < lastGroundedTime + data.coyoteTime;
    }

    private bool CanDoubleJump()
    {
        return data.canDoubleJump && !IsGrounded() && !hasDoubleJumped;
    }

    private void Jump()
    {
        lastJumpInputTime = -1f;
        lastGroundedTime = -1f;
        isJumping = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
    }

    private void DoubleJump()
    {
        hasDoubleJumped = true;
        lastJumpInputTime = -1f;
        isJumping = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * data.doubleJumpForce, ForceMode2D.Impulse);
    }

    private void UpdateGroundedState()
    {
        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            hasDoubleJumped = false;
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, data.groundCheckSize, 0, data.groundLayer);
    }
    #endregion

    #region DASH
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
            StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        if (data.disableGravityDuringDash)
            rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(facing * data.dashForce, 0f);

        yield return new WaitForSeconds(data.dashDuration);

        if (data.disableGravityDuringDash)
            rb.gravityScale = defaultGravityScale;

        isDashing = false;

        yield return new WaitForSeconds(data.dashCooldown);
        canDash = true;
    }
    #endregion

    #region ATTACK
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || timeSinceAttack < AttackCooldown)
            return;

        timeSinceAttack = 0f;
        isAttacking = true;
        anim?.SetTrigger("isAttack");

        // Determine attack direction
        if (moveInput.y == 0 || (moveInput.y < 0 && IsGrounded()))
            Hit(SideAttackTransform, SideAttackArea);
        else if (moveInput.y > 0)
            Hit(UpAttackTransform, UpAttackArea);
        else if (moveInput.y < 0)
            Hit(DownAttackTransform, DownAttackArea);
    }

    public void OnRangeAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || timeSinceAttack < AttackCooldown)
            return;

        timeSinceAttack = 0f;
        isAttacking = true;

        // Attempt to cast spell (check mana inside Health)
        if (health.UseMana(ManaSpellCost))
        {
            anim?.SetTrigger("onRangedAttack");
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void OnSkillone(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerSkillManager.Instance?.UseEquippedSkill(0, gameObject);
        }
    }

    public void OnSkilltwo(InputAction.CallbackContext context)
    {
        if (context.performed)
            PlayerSkillManager.Instance?.UseEquippedSkill(1, gameObject);
    }

    public void OnSkillthree(InputAction.CallbackContext context)
    {
        if (context.performed)
            PlayerSkillManager.Instance?.UseEquippedSkill(2, gameObject);
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(
            _attackTransform.position,
            _attackArea,
            0f,
            attackableLayer
        );

        foreach (Collider2D target in objectsToHit)
        {
            if (target == null || target.gameObject == gameObject) continue;

            // Try to damage anything with IDamageable
            IDamageable dmgTarget = target.GetComponent<IDamageable>();
            if (dmgTarget != null)
            {
                Vector2 hitDir = (target.transform.position - transform.position).normalized;
                dmgTarget.TakeDamage(Damage, hitDir, HitForce, gameObject);
                // HitStopTime(0f, 2, 0f); // timescale, restorespeed,, delay
                Debug.Log($"[Player] Hit {target.name} for {Damage} dmg with {HitForce} force.");
            }
        }
    }

    private void ShootProjectile()
    {

        if (ProjectilePrefab == null) return;

        var projObj = Instantiate(ProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        var projectile = projObj.GetComponent<TailsmanProjectile>();
        if (projectile != null && hasHoming)
        {
            float damage = 1f;
            float knockback = 2f;
            float speed = 8f;
            float lifetime = 3f;
            bool homing = hasHoming;
            float turnSpeed = 720f;
            float detectionRadius = 10f;

            projectile.SetStats(damage, knockback, speed, lifetime, turnSpeed, detectionRadius, homing, gameObject, data);
            projectile.Launch(facing, gameObject);
        }
        if (projectile != null && !hasHoming)
        {
            projectile.Launch(facing, gameObject);
        }
        else
            Debug.LogWarning("Projectile prefab has no TailsmanProjectile script!");
    }

    public void AE_ShootProjectile()
    {
        ShootProjectile();
    }
    public void SetActiveSkill(SkillData skill)
    {
        currentActiveSkill = skill;
    }

    // public void AE_ShootHomingProjectile()
    // {
    //     HomingAttackSkill skill = (HomingAttackSkill)currentActiveSkill;
    //     var projObj = Instantiate(skill.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    //     var homingProj = projObj.GetComponent<HomingProjectile>();
    //     homingProj.SetStats(skill, gameObject);

    //     if (homingProj != null)
    //     {
    //         homingProj.Launch(facing, gameObject);
    //     }
    // }

    private void FlipAttackPoints(float facingDir)
    {
        Vector3 pos = SideAttackTransform.localPosition;
        pos.x = Mathf.Abs(pos.x) * facingDir;
        SideAttackTransform.localPosition = pos;
    }

    private void UpdateTimers()
    {
        timeSinceAttack += Time.deltaTime;
    }

    private void UpdateAnimator()
    {
        if (anim)
        {
            anim.SetFloat("xSpeed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetFloat("ySpeed", rb.linearVelocity.y);
            anim.SetFloat("facing", facing);
            anim.SetBool("onGround", IsGrounded());
            anim.SetBool("onDash", isDashing);
        }
    }

    private void UpdateFacingDirection()
    {
        if (Mathf.Abs(moveInput.x) > 0.01f)
            facing = moveInput.x > 0 ? 1f : -1f;

        if (sr) sr.flipX = facing < 0;

        BoxCollider2D rbCollider = rb.GetComponent<BoxCollider2D>();
        if (rbCollider)
        {
            Vector2 offset = rbCollider.offset;
            offset.x = Mathf.Abs(offset.x) * -facing;
            rbCollider.offset = offset;
        }

        FlipAttackPoints(facing);

        if (projectileSpawnPoint)
        {
            Vector3 spawnPos = projectileSpawnPoint.localPosition;
            spawnPos.x = Mathf.Abs(spawnPos.x) * facing;
            projectileSpawnPoint.localPosition = spawnPos;
        }
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        if (data && groundCheckPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheckPoint.position, data.groundCheckSize);
        }
    }

    private void OnDrawGizmos()
    {
        if (SideAttackTransform && UpAttackTransform && DownAttackTransform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
            Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
            Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
        }
    }
    #endregion

    #region Player Restart
    public void RestartPlayer()
    {
        // Reset player position, health, and states
        transform.position = spawnPoint ? spawnPoint.position : Vector3.zero;
        health.ResetHealth();
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        canDash = true;
        hasDoubleJumped = false;
        enabled = true; // re-enable player control

        anim.Rebind();    // Resets all parameters to default
        anim.Update(0f);  // Force Animator to update immediately
    }
    #endregion
}