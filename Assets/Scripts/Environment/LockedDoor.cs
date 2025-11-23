using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private Animator _animator;

    [ContextMenu("Unlock")]
    private void Unlock()
    {
        _animator.SetTrigger("Unlock");
    }
    private void UnlockAnimationFinished()
    {
        _collider.enabled = false;
    }
}