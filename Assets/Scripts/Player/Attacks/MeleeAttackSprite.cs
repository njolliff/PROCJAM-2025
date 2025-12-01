using UnityEngine;

public class MeleeAttackSprite : MonoBehaviour
{
    public float radius;
    [SerializeField] private Animator _animator;

    public void Swing(Vector2 attackDirection, float attackDuration)
    {
        // Update position and rotation
        transform.position = PlayerMovement.Instance.rb.position + (attackDirection * radius);
        transform.up = attackDirection;

        // Update animation speed and trigger animation
        _animator.speed = 1 / attackDuration;
        _animator.SetTrigger("Swing");
    }
}