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
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Animator _animator;

    // NON-SERIALIZED
    private Vector2 _newRoomPos;
    private Vector2 _movementInput;
    private bool _dashRequested = false, _isDashing = false, _isMovingBetweenRooms = false;
    private float _dashDurationTimer = 0f, _dashCooldownTimer = 0f;
    #endregion

    #region Initialization / Destruction
    void OnEnable()
    {
        Door.onLockAnimationFinished += () => canMove = true;
    }
    void OnDisable()
    {
        Door.onLockAnimationFinished -= () => canMove = true;
    }
    #endregion

    #region Update
    void Update()
    {
        HandleTimers();

        OrientSprite();
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
        else if (_isMovingBetweenRooms)
        {
            // Update player velocity in the direction of the new room
            if (Vector2.Distance(_rb.position, _newRoomPos) > 0.05)
            {
                Vector2 updatedVelocity = moveBetweenRoomSpeed * (_newRoomPos - _rb.position).normalized;
                _rb.linearVelocity = updatedVelocity;
            }

            // Halt velocity when target position reached
            else
            {
                _rb.linearVelocity = Vector2.zero;
                _isMovingBetweenRooms = false;
            }
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
    public void MovePlayerTo(Vector2 pos)
    {
        _newRoomPos = pos;
        _isMovingBetweenRooms = true;
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

    #region Helper Methods
    private void OrientSprite()
    {
        // Animator Bools:
        // isAlive
        // isMovingUp
        // isMovingDown
        // isMovingHorizontal

        // Player is not moving (or barely moving)
        if (Math.Abs(_rb.linearVelocityX) <= 0.1 && Math.Abs(_rb.linearVelocityY) <= 0.1)
        {
            // Set animator bools
            _animator.SetBool("isMovingUp", false);
            _animator.SetBool("isMovingDown", false);
            _animator.SetBool("isMovingHorizontal", false);

            // Unflip sprite if necessary
            if (_sprite.flipX) 
                _sprite.flipX = false;
        }
        // For now (with only idle and walking side animations) walk if not idle
        else
        {
            // Set animator bools
            _animator.SetBool("isMovingUp", false);
            _animator.SetBool("isMovingDown", false);
            _animator.SetBool("isMovingHorizontal", true);

            // Flip sprite if necessary (faces right by default)
            if (_rb.linearVelocityX >= 0 && _sprite.flipX) 
                _sprite.flipX = false;
            else if (_rb.linearVelocityX < 0 && !_sprite.flipX)
                _sprite.flipX = true;
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