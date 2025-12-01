using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private protected Rigidbody2D _rb;
    [SerializeField] private GameObject _halfHeartPrefab, _fullHeartPrefab;
    [ReadOnly] public Room room;

    [Header("Enemy Values")]
    public float hp;
    public float attack, knockback, hitstun;
    public float defense, kbResistance;
    [ReadOnly] public bool isAlive = true;
    [ReadOnly] public bool canMove = false;
    public float heartDropChance, fullHeartChance;

    void OnDestroy()
    {
        // Alert room of death
        if (!isAlive)
            room.EnemyDied(this);

        // Possibly spawn a heart
        if (Random.value < heartDropChance)
        {
            if (Random.value < fullHeartChance)
                Instantiate(_fullHeartPrefab, _rb.position - Vector2.up * 0.5f, Quaternion.identity);
            else
                Instantiate(_halfHeartPrefab, _rb.position - Vector2.up * 0.5f, Quaternion.identity);
        }
    }

    public virtual void TakeHit(float incomingAttack, float incomingKB, Vector2 knockbackDir)
    {
        // Calculate damage, taking at least half a heart
        float damage = incomingAttack - defense;
        if (damage < 0.5) damage = 0.5f;

        // Check for death
        if (hp - damage <= 0)
            Die();

        // If not dead, take damage and knockback normally
        else
        {
            // Calculate knockback
            float knockbackStrength = incomingKB - kbResistance;
            if (knockbackStrength < 0) knockbackStrength = 0;

            // Take damage
            hp -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. HP: {hp}");

            // Take knockback
            _rb.AddForce(knockbackDir * knockbackStrength, ForceMode2D.Impulse);
        }
    }

    [ContextMenu("Die")]
    private protected void Die()
    {
        hp = 0;
        isAlive = false;

        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
    }
}