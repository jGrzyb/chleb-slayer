using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 30f;
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashBufferTime = 0.1f;
    [Header("Attack")]
    [SerializeField] private float attackBufferTime = 0.1f;
    [Header("Tower")]
    [SerializeField] private Tower towerPrefab;

    private GameManager.PlayerStats Stats => GameManager.I.playerStats;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Vector2 lastNonZeroMovementDirection = Vector2.right;
    private float dashRemainingTime = 0f;
    private float dashBufferRemainingTime = 0f;
    private float dashCooldownRemainingTime = 0f;
    private Vector2 targetVelocity = Vector2.zero;
    private float invincibilityRemainingTime = 0f;
    private float attackBufferRemainingTime = 0f;
    private float attackCooldownRemainingTime = 0f;
    private Collider2D[] attackColliderBuffer = new Collider2D[16];
    private float currentHealth;
    private int towerResource = 7;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = Stats.maxHealth;
    }

    void FixedUpdate()
    {
        UpdateState();
        HandleDash();
        HandleWalking();
        HandleAttack();
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void UpdateState()
    {
        dashRemainingTime -= Time.fixedDeltaTime;
        dashBufferRemainingTime -= Time.fixedDeltaTime;
        dashCooldownRemainingTime -= Time.fixedDeltaTime;
        invincibilityRemainingTime -= Time.fixedDeltaTime;
        attackBufferRemainingTime -= Time.fixedDeltaTime;
        attackCooldownRemainingTime -= Time.fixedDeltaTime;
    }

    private void HandleDash()
    {
        if (dashBufferRemainingTime > 0f && dashCooldownRemainingTime < 0f)
        {
            dashRemainingTime = dashDuration;
            invincibilityRemainingTime = dashDuration;
            dashCooldownRemainingTime = Stats.dashCooldown;
            dashBufferRemainingTime = 0f;
            targetVelocity = lastNonZeroMovementDirection * dashSpeed;
        }
    }

    private void HandleWalking()
    {
        if (dashRemainingTime <= 0f)
        {
            targetVelocity = movementDirection * Stats.movementSpeed;
        }
    }

    private void HandleAttack()
    {
        if (attackBufferRemainingTime > 0f && attackCooldownRemainingTime <= 0f)
        {
            attackCooldownRemainingTime = Stats.attackCooldown;
            attackBufferRemainingTime = 0f;
            int count = Physics2D.OverlapCircle(transform.position, Stats.attackRange, ContactFilter2D.noFilter, attackColliderBuffer);
            Debug.Log($"Player attacks: {{ {string.Join(", ", attackColliderBuffer.Take(count).Select(c => c.name)) } }}");
        }
    }

    public void TakeDamage(float damage)
    {
        if (invincibilityRemainingTime > 0f) return;
        Debug.Log($"Player takes {damage} damage.");
        invincibilityRemainingTime = Stats.damageInvincibilityDuration;
        float previousHealth = currentHealth;
        currentHealth -= damage * (1f - Stats.damageResistance);
        if (currentHealth <= 0f && previousHealth > 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }

    public void OnPlace(InputAction.CallbackContext context)
    {
        if (context.started && towerResource >= Stats.towerCost)
        {
            Instantiate(towerPrefab, Vector3Int.RoundToInt(transform.position), Quaternion.identity);
            towerResource -= Stats.towerCost;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackBufferRemainingTime = attackBufferTime;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashBufferRemainingTime = dashBufferTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>().normalized;
        if (movementDirection != Vector2.zero)
        {
            lastNonZeroMovementDirection = movementDirection;
        }
    }
}
