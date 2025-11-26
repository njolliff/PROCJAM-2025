using UnityEngine;

public class Room : MonoBehaviour
{
    #region Variables
    [Header("Room Values")]
    public RoomType roomType;
    public bool playerInRoom = false;

    [Header("References")]
    public RoomContentSpawner contentSpawner;
    public RoomShroud shroud;
    public Door[] doors;

    public enum RoomType { Enemy, Treasure, Start, Boss };
    private bool _contentSpawned = false;
    #endregion

    #region Player Enter/Exit Room
    public void OnPlayerEnteredRoom()
    {
        playerInRoom = true;
        
        if (!_contentSpawned)
        {
            contentSpawner.SpawnRoomContent(roomType);
            _contentSpawned = true;
        }

        shroud.FadeFromBlack();
    }
    public void OnPlayerExitedRoom()
    {
        playerInRoom = false;
        shroud.FadeToBlack();
    }
    #endregion

    #region Shroud
    [ContextMenu("Lift Shroud")]
    public void LiftShroud()
    {
        shroud.FadeFromBlack();
    }
    [ContextMenu("Lower Shroud")]
    public void LowerShroud()
    {
        shroud.FadeToBlack();
    }
    public void FinishedFadingFromBlack()
    {
        // If the room is enemy/boss, lock the doors, movement will be unlocked once locking finishes
        if (roomType == RoomType.Enemy || roomType == RoomType.Boss)
            LockDoors();
        // Otherwise, the doors don't need to lock and movement can be unlocked
        else
            PlayerMovement.Instance.canMove = true;
    }
    #endregion

    #region Doors
    [ContextMenu("Lock Doors")]
    public void LockDoors()
    {
        foreach (var door in doors)
            door.Lock();
    }
    [ContextMenu("Unlock Doors")]
    public void UnlockDoors()
    {
        foreach (var door in doors)
            door.Unlock();
    }
    #endregion
}