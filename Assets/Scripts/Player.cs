using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float acceleration = 30f;
    [Space]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashBufferTime = 0.1f;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Vector2 lastNonZeroMovementDirection;
    private float dashRemainingTime = 0f;
    private float dashBufferRemainingTime = 0f;
    private float dashCooldownRemainingTime = 0f;
    private Vector2 targetVelocity = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    private void HandleDash()
    {
        if (dashBufferRemainingTime > 0f && dashCooldownRemainingTime < 0f)
        {
            dashRemainingTime = dashDuration;
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
