using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Room _startingRoom;

    [Header("Spawning")]
    public int roomCount;
    public float branchChance, extraDoorChance, treasureRoomChance;
    public Vector2 roomSpacing;

    private List<RoomNode> _roomGraph;

    private void GenerateLevel()
    {
        // Build a graph of RoomNodes
        _roomGraph = RoomGraphGenerator.GenerateGraph(roomCount, branchChance, extraDoorChance, treasureRoomChance);
    }
}