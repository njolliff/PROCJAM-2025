using UnityEngine;

public class Room : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public Door[] doors;
    public RoomShroud shroud;
    #endregion

    #region Initialization / Destruction
    void OnEnable()
    {
        // Subscribe to events
        shroud.onFadeFromBlackFinished += LockDoors;
    }
    void OnDisable()
    {
        // Unsubscribe from events
        shroud.onFadeFromBlackFinished -= LockDoors;
    }
    #endregion

    #region Player Enter/Exit Room
    public void OnPlayerEnteredRoom()
    {
        shroud.FadeFromBlack();
    }
    public void OnPlayerExitedRoom()
    {
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