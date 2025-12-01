using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    // SERIALIZED
    [Header("Movement Values")]
    public bool canMove = true;
    public float acceleration = 1f, maxVelocity = 3f, moveBetweenRoomSpeed = 0.5f;
    public bool canDash = true;
    public float dashStrength = 5f, dashDuration = 0.5f, dashCooldown = 2f;

    [Header("References")]
    public Rigidbody2D rb;

    // NON-SERIALIZED
    public static PlayerMovement Instance;
    [NonSerialized] public Vector2 movementInput;
    private Vector2 _newRoomPos;
    private bool _dashRequested = false, _isDashing = false, _isMovingBetweenRooms = false, _isHitstunned = false; // Movement states
    private float _dashDurationTimer = 0f, _dashCooldownTimer = 0f, _hitstunTimer = 0f; // Timers
    private float _hitstunDuration;
    #endregion

    #region Initialization / Destruction
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    void OnEnable()
    {
        // Subscribe to events
        Door.onLockAnimationFinished += () => canMove = true;
    }
    void OnDisable()
    {
        // Unsubscribe from events
        Door.onLockAnimationFinished -= () => canMove = true;
    }
    void OnDestroy()
    {
        // Singleton
        if (Instance == this)
            Instance = null;
    }
    #endregion

    #region Update
    // Non Physics-based Logic
    void Update()
    {
        // Dash duration / CD / Hitstun
        HandleTimers();
    }

    // Physics-based logic
    void FixedUpdate()
    {
        // Player movement
        if (canMove)
        {
            // Dash if player requested and there is movement input
            if (_dashRequested && movementInput != Vector2.zero)
            {
                _dashRequested = false;
                Dash();
            }

            // Otherwise move like normal
            else
                MovePlayer();
        }

        // Moving player between rooms
        else if (_isMovingBetweenRooms)
        {
            // Update player velocity in the direction of the new room
            if (Vector2.Distance(rb.position, _newRoomPos) > 0.05)
            {
                Vector2 updatedVelocity = moveBetweenRoomSpeed * (_newRoomPos - rb.position).normalized;
                rb.linearVelocity = updatedVelocity;
            }

            // Halt velocity when target position reached
            else
            {
                rb.linearVelocity = Vector2.zero;
                _isMovingBetweenRooms = false;
            }
        }
    }
    #endregion

    #region Movement
    private void MovePlayer()
    {
        // Calculate target velocity
        Vector2 targetVelocity = movementInput * maxVelocity;

        // Calculate updated velocity
        Vector2 updatedVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Apply updated velocity
        rb.linearVelocity = updatedVelocity;
    }
    private void Dash()
    {
        // Disable dash and movement
        canDash = false;
        canMove = false;

        // Enable dashing state
        _isDashing = true;

        // Halt velocity so dash strength is the same in every direction every time
        rb.linearVelocity = Vector2.zero;

        // Apply force
        rb.AddForce(movementInput * dashStrength, ForceMode2D.Impulse);
    }
    public void MovePlayerTo(Vector2 pos)
    {
        // Set new room position and start moving player
        _newRoomPos = pos;
        _isMovingBetweenRooms = true;
    }
    public void TakeKnockback(float knockbackStrength, Vector2 knockbackDir)
    {
        // Halt velocity so force is always the same, then apply force
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackStrength, ForceMode2D.Impulse);
    }
    public void TakeHitstun(float hitstunDuration)
    {
        // Reset hitstun timer with new duration
        _hitstunTimer = 0f;
        _hitstunDuration = hitstunDuration;

        // Disable movement and enable timer
        canMove = false;
        _isHitstunned = true;
    }
    #endregion

    #region Timers
    private void HandleTimers()
    {
        // Hitstun
        HandleHitstunTimer();

        // Dash duration
        HandleDashDurationTimer();

        // Dash cooldown
        HandleDashCooldownTimer();
    }
    private void HandleHitstunTimer()
    {
        if (_isHitstunned)
        {
            _hitstunTimer += Time.deltaTime;
            if (_hitstunTimer >= _hitstunDuration)
            {
                canMove = true;

                _hitstunTimer = 0f;
                _hitstunDuration = 0f;
                _isHitstunned = false;
            }
        }
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

    #region Input
    public void OnMove(InputValue value)
    {
        // Get & set movement input
        movementInput = value.Get<Vector2>().normalized;
    }
    public void OnDash()
    {
        if (canDash)
            _dashRequested = true;
    }
    #endregion
}