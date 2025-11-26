using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum Direction { North, East, South, West }
    [SerializeField] private Direction _direction;

    [Header("References")]
    [SerializeField] private Room _room;
    [SerializeField] private Transform _entryPosition;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private Animator _animator;

    private Vector3 _nextRoomPos;
    public static Action onLockAnimationFinished;

    void Awake()
    {
        // Calculate the position to send player to in the next room
        if (_direction == Direction.North)
            _nextRoomPos = _entryPosition.position + (Vector3.up * 6.5f);
        if (_direction == Direction.East)
            _nextRoomPos = _entryPosition.position + (Vector3.right * 6.5f);
        if (_direction == Direction.South)
            _nextRoomPos = _entryPosition.position + (Vector3.down * 6.5f);
        if (_direction == Direction.West)
            _nextRoomPos = _entryPosition.position + (Vector3.left * 6.5f);
    }

    #region Lock / Unlock
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
    #endregion

    #region Animation Events
    /// <summary>
    /// Enables door collider. Called by event at the end of door Lock animation.
    /// </summary>
    private void LockAnimationFinished()
    {
        _collider.enabled = true;
        onLockAnimationFinished?.Invoke();
    }
    /// <summary>
    /// Disables door collider. Called by event at the end of door Unlock animation.
    /// </summary>
    private void UnlockAnimationFinished()
    {
        _collider.enabled = false;
    }
    #endregion

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player Physics"))
        {
            // Player leaving room
            if (_room.playerInRoom)
            {
                // Get player movement script off of player parent game object
                Transform parentTransform = collision.transform;
                while (parentTransform.parent != null)
                {
                    parentTransform = parentTransform.parent;
                }
                PlayerMovement playerMovement = parentTransform.gameObject.GetComponent<PlayerMovement>();
                if (playerMovement == null) 
                    return;
                
                // Pause player movement
                playerMovement.canMove = false;

                // Call player exited method
                _room.OnPlayerExitedRoom();

                // Move player to new room
                playerMovement.MovePlayerTo(_nextRoomPos);
            }

            // Player entering room
            else
            {
                // Call player entered method
                _room.OnPlayerEnteredRoom();
            }
        }
    }
}