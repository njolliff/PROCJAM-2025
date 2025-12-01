using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [ReadOnly] public float speed;
    [ReadOnly] public float attack;
    [ReadOnly] public float knockback;

    private bool _launched = false;

    void FixedUpdate()
    {
        if (_launched)
        {
            // Move projectile at constant speed
            _rb.linearVelocity = transform.up * speed;
        }
    }

    public void Launch(Vector2 attackDir, float atk, float kb, float projSpeed)
    {
        // Rotate projectile
        transform.up = attackDir;

        // Set projectile values
        attack = atk;
        knockback = kb;
        speed = projSpeed;

        // Launch projectile
        _launched = true;
    }

    private void Hit(GameObject hit)
    {
        Enemy enemy = hit.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector2 knockbackDir = _rb.linearVelocity.normalized; // Ranged projectiles deal knockback in their direction of travel
            enemy.TakeHit(incomingAttack: attack, incomingKB: knockback, knockbackDir: knockbackDir);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Hit(collision.gameObject);
        }

        Destroy(gameObject);
    }
}