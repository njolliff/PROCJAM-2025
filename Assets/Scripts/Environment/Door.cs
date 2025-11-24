using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private Animator _animator;

    [ContextMenu("Lock")]
    public void Lock()
    {
        _animator.SetTrigger("Lock");
    }
    [ContextMenu("Unlock")]
    public void Unlock()
    {
        _animator.SetTrigger("Unlock");
    }

    /// <summary>
    /// Enables door collider. Called by event at the end of door Lock animation.
    /// </summary>
    private void LockAnimationFinished()
    {
        _collider.enabled = true;
    }
    /// <summary>
    /// Disables door collider. Called by event at the end of door Unlock animation.
    /// </summary>
    private void UnlockAnimationFinished()
    {
        _collider.enabled = false;
    }
}