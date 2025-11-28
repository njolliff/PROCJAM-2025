using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [ReadOnly] public float speed;
    [ReadOnly] public float damage;
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

    public void Launch(Vector2 attackDir, float dmg, float kb, float projSpeed)
    {
        // Rotate projectile
        transform.up = attackDir;

        // Set projectile values
        damage = dmg;
        knockback = kb;
        speed = projSpeed;

        // Launch projectile
        _launched = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Hit enemy {collision.gameObject.name} for {damage} damage with {knockback} knockback.");
        }

        Destroy(gameObject);
    }
}