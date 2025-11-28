using UnityEngine;

public class MeleeAttackSprite : MonoBehaviour
{
    public float radius;
    [SerializeField] private Animator _animator;

    private Vector2 _offset = new();

    public void Swing(Vector2 attackDirection)
    {
        // Rotate sprite
        transform.up = attackDirection;

        // Update offset
        _offset = attackDirection * radius;

        // Trigger animation
        _animator.SetTrigger("Swing");
    }

    void Update()
    {
        // Move with player
        transform.position = PlayerMovement.Instance.rb.position + _offset;
    }
}