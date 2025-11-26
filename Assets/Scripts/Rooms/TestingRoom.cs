using UnityEngine;

/// <summary>
/// Small child class of Room that spawns content immediately, and can optionally be given a layout instead of randomly picking one.
/// </summary>
public class TestingRoom : Room
{
    [Header("Layout")]
    public RoomLayout roomLayout;

    void Awake()
    {
        OnPlayerEnteredRoom();
    }

    public override void OnPlayerEnteredRoom()
    {
        playerInRoom = true;
        
        if (!_contentSpawned)
        {
            if (roomLayout == null)
                contentSpawner.SpawnRoomContent(roomType);
            else
                contentSpawner.SetTilemapLayout(roomLayout);
            _contentSpawned = true;
        }

        shroud.FadeFromBlack();
    }
}