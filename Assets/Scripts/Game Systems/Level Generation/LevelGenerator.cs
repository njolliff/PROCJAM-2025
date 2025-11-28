using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public RoomsContainer roomsContainer;
    public Transform grid;

    [Header("Spawning")]
    public int roomCount;
    public float branchChance, extraDoorChance, treasureRoomChance;
    public Vector2Int roomSpacing;

    private List<RoomNode> _roomGraph;
    private Room _startingRoom;

    void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        // Build a graph of RoomNodes
        _roomGraph = RoomGraphGenerator.GenerateGraph(roomCount, branchChance, extraDoorChance, treasureRoomChance);

        // Place each room with the appropriate prefab and set their types
        foreach (RoomNode roomNode in _roomGraph)
        {
            // Get spawn position
            Vector3 spawnPos = (Vector3)((Vector2)roomNode.gridPosition * roomSpacing) + (Vector3.right * 0.5f);
            
            // Get the appropriate room prefab based on neighbor directions
            List<Door.Direction> doorDirections = GetDoorDirections(roomNode);
            GameObject roomPrefab = roomsContainer.GetAppropriateRoom(doorDirections);

            // Instantiate the room
            if (roomPrefab != null) 
            {
                GameObject room = Instantiate(roomPrefab, spawnPos, Quaternion.identity, grid);
            
                // Set the room's type
                Room roomScript = room.GetComponent<Room>();
                if (roomScript != null)
                {
                    roomScript.roomType = roomNode.roomType;

                    // Store the starting room
                    if (roomScript.roomType == Room.RoomType.Start)
                        _startingRoom = roomScript;
                }
            }
        }

        // After spawning is finished, enable the player then lift the shroud on the starting room and set its playerInRoom bool
        player.SetActive(true);
        _startingRoom.LiftShroud();
        _startingRoom.playerInRoom = true;
    }

    private List<Door.Direction> GetDoorDirections(RoomNode roomNode)
    {
        List<Door.Direction> directions = new();

        foreach (RoomNode neighbor in roomNode.neighbors)
        {
            Vector2Int dir = neighbor.gridPosition - roomNode.gridPosition;

            if (dir == Vector2Int.up) directions.Add(Door.Direction.North);
            else if (dir == Vector2Int.right) directions.Add(Door.Direction.East);
            else if (dir == Vector2Int.down) directions.Add(Door.Direction.South);
            else if (dir == Vector2Int.left) directions.Add(Door.Direction.West);
        }

        return directions;
    }
}