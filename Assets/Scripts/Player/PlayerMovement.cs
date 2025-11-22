using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    // SERIALIZED
    [Header("Movement Values")]
    public bool canMove = true;
    public float acceleration = 1f, maxVelocity = 3f;
    public bool canDash = true;
    public float dashStrength = 5f, dashDuration = 0.5f, dashCooldown = 2f;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;

    // NON-SERIALIZED
    private Vector2 _movementInput;
    private bool _dashRequested = false, _isDashing = false;
    private float _dashDurationTimer = 0f, _dashCooldownTimer = 0f;
    #endregion

    #region Update
    void Update()
    {
        HandleTimers();
    }
    void FixedUpdate()
    {
        if (canMove)
        {
            // Dash if player requested and there is movement input
            if (_dashRequested && _movementInput != Vector2.zero)
            {
                _dashRequested = false;
                Dash();
            }

            // Otherwise move like normal
            else
                MovePlayer();
        }
    }
    #endregion

    #region Movement Methods
    private void MovePlayer()
    {
        // Calculate target velocity
        Vector2 targetVelocity = _movementInput * maxVelocity;

        // Calculate updated velocity
        Vector2 updatedVelocity = Vector2.MoveTowards(_rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Apply updated velocity
        _rb.linearVelocity = updatedVelocity;
    }
    private void Dash()
    {
        // Disable dash and movement
        canDash = false;
        canMove = false;

        // Enable dashing state
        _isDashing = true;

        // Halt velocity so dash strength is the same in every direction every time
        _rb.linearVelocity = Vector2.zero;

        // Apply force
        _rb.AddForce(_movementInput * dashStrength, ForceMode2D.Impulse);
    }
    #endregion

    #region Timer Methods
    private void HandleTimers()
    {
        // Dash duration
        HandleDashDurationTimer();

        // Dash cooldown
        HandleDashCooldownTimer();
    }
    private void HandleDashDurationTimer()
    {
        if (_isDashing)
        {
            // Increment timer
            _dashDurationTimer += Time.deltaTime;

            // Timer finished?
            if (_dashDurationTimer >= dashDuration)
            {
                _isDashing = false; // Disable dashing state
                canMove = true; // Enable movement
                _dashDurationTimer = 0f; // Reset timer
            }
        }
    }
    private void HandleDashCooldownTimer()
    {
        if (!canDash)
        {
            // Increment timer
            _dashCooldownTimer += Time.deltaTime;

            // Timer finished?
            if (_dashCooldownTimer >= dashCooldown)
            {
                canDash = true; // Enable dashing
                _dashCooldownTimer = 0f; // Reset timer
            }
        }
    }
    #endregion

    #region Input Methods
    public void OnMove(InputValue value)
    {
        // Get & set movement input
        _movementInput = value.Get<Vector2>();
    }
    public void OnDash()
    {
        if (canDash)
            _dashRequested = true;
    }
    #endregion
}