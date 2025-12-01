using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Animator _animator;

    [Header("Animation Values")]
    [SerializeField] private float _movementThreshold = 0.1f;
    [ReadOnly] public MovementState movementState;

    public enum MovementState { Idle, Up, Down, Right, Left}

    void Update()
    {
        SetMovementState();
        UpdateAnimator();
    }

    #region Animations
    private void SetMovementState()
    {
        Vector2 velocity = PlayerMovement.Instance.rb.linearVelocity;

        // Player is not moving (or barely moving)
        if (Math.Abs(velocity.x) <= _movementThreshold && Math.Abs(velocity.y) <= _movementThreshold)
            movementState = MovementState.Idle;

        // Determine if player is moving more vertically or horizontally (defualt to horizontal)
        // Horizontal
        else if (Math.Abs(velocity.x) >= Math.Abs(velocity.y))
        {
            if (velocity.x > 0)
                 movementState = MovementState.Right;
            else
                movementState = MovementState.Left;
        }
        // Vertical
        else
        {
            if (velocity.y > 0)
                movementState = MovementState.Up;
            else
                movementState = MovementState.Down;
        }
    }
    private void UpdateAnimator()
    {
        // Animator Bools:
        // isAlive (not set by movement state)
        // isMovingUp
        // isMovingDown
        // isMovingHorizontal

        // Horizontal
        if (movementState == MovementState.Right || movementState == MovementState.Left)
        {
            // Set animator bools
            _animator.SetBool("isMovingUp", false);
            _animator.SetBool("isMovingDown", false);
            _animator.SetBool("isMovingHorizontal", true);

            // Flip sprite if moving left
            _sprite.flipX = (movementState == MovementState.Left) ? true : false;
        }

        // Up
        else if (movementState == MovementState.Up)
        {
            // Set animator bools
            _animator.SetBool("isMovingUp", true);
            _animator.SetBool("isMovingDown", false);
            _animator.SetBool("isMovingHorizontal", false);

            // Unflip sprite
            _sprite.flipX = false;
        }

        // Down
        else if (movementState == MovementState.Down)
        {
             // Set animator bools
            _animator.SetBool("isMovingUp", false);
            _animator.SetBool("isMovingDown", true);
            _animator.SetBool("isMovingHorizontal", false);

            // Unflip sprite
            _sprite.flipX = false;
        }

        // Idle
        else if (movementState == MovementState.Idle)
        {
            // Set animator bools
            _animator.SetBool("isMovingUp", false);
            _animator.SetBool("isMovingDown", false);
            _animator.SetBool("isMovingHorizontal", false);

            // Unflip sprite
            _sprite.flipX = false;
        }
    }
    #endregion
}