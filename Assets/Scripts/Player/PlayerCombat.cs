using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    #region Variables
    [Header("Default Values")]
    public float maxHP = 10;
    public float maxPossibleHP = 20;
    public float startingDefense = 0;
    public float startingMeleeDMG = 1, startingRangedDMG = 1;
    public float startingMeleeKB = 1, startingRangedKB = 0.25f;
    public float startingRangedAttackCooldown = 0.25f, startingProjectileSpeed = 1f;
    
    [Header("Live Values")]
    [ReadOnly] public bool isAlive = true;
    [ReadOnly] public float hp, defense;
    [ReadOnly] public float meleeAttackDMG, rangedAttackDMG;
    [ReadOnly] public float meleeAttackKB, rangedAttackKB;
    [ReadOnly] public float rangedAttackCooldown, projectileSpeed;

    [Header("References")]
    [SerializeField] private MeleeAttack _meleeAttack;
    [SerializeField] private RangedAttack _rangedAttack;
    [SerializeField] private PlayerInput _playerInput;

    public static PlayerCombat Instance;
    [NonSerialized] public Vector2 aimDirection = new();
    private bool _isAttacking => _meleeAttack.isAttacking || _rangedAttack.isAttacking;
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

        // Set default values
        // HEALTH
        hp = maxHP;
        defense = startingDefense;

        // MELEE
        meleeAttackDMG = startingMeleeDMG;
        meleeAttackKB = startingMeleeKB;

        // RANGED
        rangedAttackDMG = startingRangedDMG;
        rangedAttackKB = startingRangedKB;
        rangedAttackCooldown = startingRangedAttackCooldown;
        projectileSpeed = startingProjectileSpeed;
    }
    void OnDestroy()
    {
        // Singleton
        if (Instance == this)
            Instance = null;
    }
    #endregion

    public void TakeDamage(float attack)
    {
        // Calculate damage
        float damage = defense - attack;
        
        // Check for player death
        if (hp - damage <= 0)
        {
            hp = 0;
            Debug.Log($"HP: {hp}");
            isAlive = false;
            PlayerController.Instance.KillPlayer();
        }

        // if not dead, take damage normally
        else
        {
            hp -= damage;
            Debug.Log($"HP: {hp}");
        }
    }

    #region Input Methods
    public void OnAim(InputValue value)
    {
        aimDirection = value.Get<Vector2>();
    }
    public void OnMeleeAttack()
    {
        if (!_isAttacking)
        {
            // Get attack direction
            Vector2 attackDir = DetermineAttackDirection();

            // If attack direction is (0,0) default to up
            // Should only occur when there is no controller input, the player clicked exactly on top of the player, or the player is on an unknown control scheme
            if (attackDir == Vector2.zero) attackDir = Vector2.down;

            // Perform attack
            _meleeAttack.PerformAttack(
                attackDir: attackDir,
                attackDMG: meleeAttackDMG, 
                attackKB: meleeAttackKB
                );
        }
    }
    public void OnRangedAttack()
    {
        if (!_isAttacking) 
        {
            // Get attack direction
            Vector2 attackDir = DetermineAttackDirection();

            // If attack direction is (0,0) default to up
            // Should only occur when there is no controller input, the player clicked exactly on top of the player, or the player is on an unknown control scheme
            if (attackDir == Vector2.zero) attackDir = Vector2.up;

            // Perform attack
            _rangedAttack.PerformAttack(
                dir: attackDir, 
                dmg: rangedAttackDMG, 
                kb: rangedAttackKB, 
                projSpeed: projectileSpeed, 
                cd: rangedAttackCooldown);
        }
    }

    private Vector2 DetermineAttackDirection()
    {
        // If on controller, use aimDirection
        if (_playerInput.currentControlScheme == "Gamepad")
        {
            return aimDirection;
        }
        // If KBM, use the mouse pos
        else if (_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            // Determine attack direction from mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mouseDirection = ((Vector2)(mousePos - transform.position)).normalized;
            
            return mouseDirection;
        }

        // If on an unknown control scheme, return (0,0), which will default to (0,1) in attack logic
        return Vector2.zero;
    }
    #endregion
}