using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomContentSpawner : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap contentTilemap;
    public Tilemap debugTilemap;

    [Header("Room Layouts")]
    public RoomLayoutContainer enemyRooms;
    public RoomLayoutContainer treasureRooms;
    public RoomLayoutContainer bossRooms;
    
    public void SpawnRoomContent(Room.RoomType type)
    {
        if (type == Room.RoomType.Enemy)
        {
            RoomLayout chosenLayout = enemyRooms.GetRandomLayout();
            if (chosenLayout != null)
                SetTilemapLayout(chosenLayout);
        }
        else if (type == Room.RoomType.Treasure)
        {
            RoomLayout chosenLayout = treasureRooms.GetRandomLayout();
            if (chosenLayout != null)
                SetTilemapLayout(chosenLayout);
        }
        else if (type == Room.RoomType.Boss)
        {
            RoomLayout chosenLayout = bossRooms.GetRandomLayout();
            if (chosenLayout != null)
                SetTilemapLayout(chosenLayout);
        }
    }

    private void SetTilemapLayout(RoomLayout roomLayout)
    {
        // Clear tilemaps for safety
        contentTilemap.ClearAllTiles();
        debugTilemap.ClearAllTiles();

        // Place each tile in the room layout
        foreach (var tile in roomLayout.tiles)
        {
            contentTilemap.SetTile((Vector3Int)tile.position, tile.tile);
            debugTilemap.SetTile((Vector3Int)tile.position, tile.debugTile);
        }
    }
}