using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomContentSpawner : MonoBehaviour
{
    [Header("References")]
    public Room room;
    public Tilemap contentTilemap;

    [Header("Room Layouts")]
    public RoomLayoutContainer enemyRooms;
    public RoomLayoutContainer treasureRooms;
    public RoomLayoutContainer bossRooms;
    
    public void SpawnRoomContent(Room.RoomType type)
    {
        RoomLayout chosenLayout = GetRoomLayout(type);
        if (chosenLayout != null)
            SetTilemapLayout(chosenLayout);
    }

    private RoomLayout GetRoomLayout(Room.RoomType type)
    {
        return type switch
        {
            Room.RoomType.Enemy => enemyRooms.GetRandomLayout(),
            Room.RoomType.Treasure => treasureRooms.GetRandomLayout(),
            Room.RoomType.Boss => bossRooms.GetRandomLayout(),
            _ => null,
        };
    }

    public void SetTilemapLayout(RoomLayout roomLayout)
    {
        // Clear tilemap for safety
        contentTilemap.ClearAllTiles();

        // Place each tile in the room layout
        foreach (var tile in roomLayout.tiles)
        {
            // Place tile
            contentTilemap.SetTile((Vector3Int)tile.position, tile.tile);

            // If tile spawned an enemy, set that enemy's room and add it to the room's enemy set
            GameObject obj = contentTilemap.GetInstantiatedObject((Vector3Int)tile.position);
            if (obj != null && obj.CompareTag("Enemy"))
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.room = room;
                    room.EnemySpawned(enemy);
                }
            }
        }
    }
}