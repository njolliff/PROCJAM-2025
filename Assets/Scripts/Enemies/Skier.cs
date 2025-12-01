using UnityEngine;

public class Skier : Enemy
{
    #region Variables
    [Header("Ice Skater Settings")]
    public float movementSpeed = 5;

    private Vector2 _movementDirection;
    private Vector2 _frameNormal;
    private bool _collidedThisFrame;
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
    void Start()
    {
        // Start on a random diagonal
        _movementDirection = new Vector2((Random.value < 0.5f) ? -1f : 1f,
                                         (Random.value < 0.5f) ? -1f : 1f).normalized;
    }
    #endregion

    #region Update
    void FixedUpdate()
    {
        if (canMove)
        {
            // Bounce
            if (_collidedThisFrame)
            {
                Vector2 normal = _frameNormal.normalized;
                _movementDirection = Vector2.Reflect(_movementDirection, normal);

                _collidedThisFrame = true;
                _frameNormal = Vector2.zero;
            }

            // Move at constant velocity
            _rb.linearVelocity = _movementDirection * movementSpeed;
        }
    }
    #endregion

    #region Collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        _frameNormal += collision.GetContact(0).normal;
        _collidedThisFrame = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit player
        if (collision.gameObject.CompareTag("Player Hurt"))
        {
            // Calculate knockback direction
            Vector2 knockbackDir = (collision.attachedRigidbody.position - _rb.position).normalized;

            // Apply hit
            PlayerCombat.Instance.TakeHit(incomingAttack: attack, incomingKB: knockback, knockbackDir: knockbackDir, hitstunDuration: hitstun);
        }
    }
    #endregion
}