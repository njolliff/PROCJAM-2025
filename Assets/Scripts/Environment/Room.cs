using UnityEngine;

public class Room : MonoBehaviour
{
    #region Variables
    [Header("Room Values")]
    public bool isActiveRoom = false;

    [Header("References")]
    public Door[] doors;
    public RoomShroud shroud;
    #endregion

    #region Player Enter/Exit Room
    public void OnPlayerEnteredRoom()
    {
        isActiveRoom = true;
        shroud.FadeFromBlack();
    }
    public void OnPlayerExitedRoom()
    {
        isActiveRoom = false;
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