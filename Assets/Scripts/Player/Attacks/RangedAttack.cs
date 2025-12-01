using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    [ReadOnly] public bool isAttacking = false;
    [ReadOnly] public float attackCooldown;

    private bool _onCooldown = false;
    private float _cooldownTimer = 0;

    void Update()
    {
        // Handle cooldown timer
        if (_onCooldown)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= attackCooldown)
            {
                _onCooldown = false;
                _cooldownTimer = 0;
            }
        }
    }

    public void PerformAttack(Vector2 dir, float dmg, float kb, float projSpeed, float cd)
    {
        if (!isAttacking && !_onCooldown)
        {
            // Disable attack and update cooldown time
            isAttacking = true;
            attackCooldown = cd;

            // Spawn and launch projectile
            PlayerProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<PlayerProjectile>();
            if (projectile != null) 
                projectile.Launch(
                    attackDir: dir, 
                    atk: dmg, 
                    kb: kb, 
                    projSpeed: projSpeed);

            // Re-enable attack but go on cooldown
            _onCooldown = true;
            isAttacking = false;
        }
    }
}