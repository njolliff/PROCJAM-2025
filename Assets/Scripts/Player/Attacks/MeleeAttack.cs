using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Swing Settings")]
    public float attackDuration = 0.3f;
    public int steps = 10; // How many positions along the arc
    public float radius = 1f; // Distance from player pivot
    public float arcAngle = 90f; // Total sweep of the arc in degrees
    public Vector2 hitboxSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private MeleeAttackSprite _attackSprite;

    [ReadOnly] public bool isAttacking = false;
    private HashSet<Collider2D> _alreadyHit = new HashSet<Collider2D>();

    public void PerformAttack(float attackDMG, float attackKB, Vector2 attackDir)
    {
        if (!isAttacking)
            StartCoroutine(AttackCoroutine(attackDMG, attackKB, attackDir));
    }

    private IEnumerator AttackCoroutine(float attackDMG, float attackKB, Vector2 attackDir)
    {
        isAttacking = true;
        _alreadyHit.Clear();

        float stepTime = attackDuration / steps;

        // Rotate to match attack direction
        transform.up = attackDir;

        // Compute arc start/end angles from the direction
        float baseAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle + arcAngle / 2f;
        float endAngle = baseAngle - arcAngle / 2f;

        // Trigger animation
        _attackSprite.Swing(attackDir);

        // Swing loop
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;

            // Current sweep angle of the attack (this drives both pos + rotation)
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = angle * Mathf.Deg2Rad;

            // Hitbox rotation (does NOT affect transform)
            float boxAngle = angle;

            // Sweep position along the arc
            Vector2 attackPos = (Vector2)transform.position +
                                new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            // Rotated hitbox collision
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                attackPos,
                hitboxSize,
                boxAngle,
                _enemyLayer
            );

            foreach (Collider2D hit in hits)
            {
                if (!_alreadyHit.Contains(hit))
                {
                    Hit(hit, attackDMG, attackKB);
                    _alreadyHit.Add(hit);
                }
            }

            // --- Debug rotated box ---
            Vector2 half = hitboxSize * 0.5f;

            // Local corner offsets before rotation
            Vector2[] corners = new Vector2[]
            {
                new Vector2(-half.x,  half.y),
                new Vector2( half.x,  half.y),
                new Vector2( half.x, -half.y),
                new Vector2(-half.x, -half.y),
            };

            Quaternion rot = Quaternion.Euler(0, 0, boxAngle);

            Vector2 tl = attackPos + (Vector2)(rot * corners[0]);
            Vector2 tr = attackPos + (Vector2)(rot * corners[1]);
            Vector2 br = attackPos + (Vector2)(rot * corners[2]);
            Vector2 bl = attackPos + (Vector2)(rot * corners[3]);

            Debug.DrawLine(tl, tr, Color.red, stepTime);
            Debug.DrawLine(tr, br, Color.red, stepTime);
            Debug.DrawLine(br, bl, Color.red, stepTime);
            Debug.DrawLine(bl, tl, Color.red, stepTime);
            // --------------------------

            yield return new WaitForSeconds(stepTime);
        }


        isAttacking = false;
    }

    private void Hit(Collider2D hit, float attackDMG, float attackKB)
    {
        if (hit.CompareTag("Enemy"))
        {
            Debug.Log($"Enemy {hit.name} hit by melee attack, attacking with {attackDMG} damage and {attackKB} knockback.");
        }
    }
}