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