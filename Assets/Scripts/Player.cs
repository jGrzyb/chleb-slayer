using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float acceleration = 30f;
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashBufferTime = 0.1f;
    [Header("Health")]
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] private float currentHealth = 1f;
    [SerializeField] private float damageInvincibilityDuration = 0.5f;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Vector2 lastNonZeroMovementDirection = Vector2.right;
    private float dashRemainingTime = 0f;
    private float dashBufferRemainingTime = 0f;
    private float dashCooldownRemainingTime = 0f;
    private Vector2 targetVelocity = Vector2.zero;
    private float invincibilityRemainingTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        UpdateState();
        HandleDash();
        HandleWalking();
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void UpdateState()
    {
        dashRemainingTime -= Time.fixedDeltaTime;
        dashBufferRemainingTime -= Time.fixedDeltaTime;
        dashCooldownRemainingTime -= Time.fixedDeltaTime;
        invincibilityRemainingTime -= Time.fixedDeltaTime;
    }

    private void HandleDash()
    {
        if (dashBufferRemainingTime > 0f && dashCooldownRemainingTime < 0f)
        {
            dashRemainingTime = dashDuration;
            invincibilityRemainingTime = dashDuration;
            dashCooldownRemainingTime = dashCooldown;
            dashBufferRemainingTime = 0f;
            targetVelocity = lastNonZeroMovementDirection * dashSpeed;
        }
    }

    private void HandleWalking()
    {
        if (dashRemainingTime <= 0f)
        {
            targetVelocity = movementDirection * walkingSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        if (invincibilityRemainingTime > 0f) return;
        Debug.Log($"Player takes {damage} damage.");
        invincibilityRemainingTime = damageInvincibilityDuration;
        float previousHealth = currentHealth;
        currentHealth -= damage;
        if (currentHealth <= 0f && previousHealth > 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
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
