using UnityEngine;
using System.Collections.Generic;

public class RoomGraphRuntimeDebugger : MonoBehaviour
{
    public List<RoomNode> rooms;

    [Header("Generation")]
    public int numRooms;
    public float branchChance, extraConnectionChance, treasureRoomChance;
    public float timeUntilNewRoom;

    [Header("Debug")]
    public float gizmoSize = 0.25f;
    public Color startRoomColor = Color.green, enemyRoomColor = Color.orange, treasureRoomColor = Color.yellow, bossRoomColor = Color.red;
    public Color connectionColor = Color.blue;

    private float _newRoomTimer = 0;
    private bool _needsNewRoom = false;

    void Start()
    {
        rooms = RoomGraphGenerator.GenerateGraph(numRooms, branchChance, extraConnectionChance, treasureRoomChance);
    }

    void Update()
    {
        if (!_needsNewRoom)
        {
            _newRoomTimer += Time.deltaTime;
            if (_newRoomTimer >= timeUntilNewRoom)
            {
                _needsNewRoom = true;
                _newRoomTimer = 0;
            }
        }

        if (_needsNewRoom) 
        {
            rooms = RoomGraphGenerator.GenerateGraph(numRooms, branchChance, extraConnectionChance, treasureRoomChance);
            _needsNewRoom = false;
        }
        if (rooms == null) return;

        foreach (var room in rooms)
        {
            Vector3 roomPos = new Vector3(room.gridPosition.x, room.gridPosition.y, 0f);

            // Draw connections to neighbors first (Z = 0)
            foreach (var neighbor in room.neighbors)
            {
                Vector3 neighborPos = new Vector3(neighbor.gridPosition.x, neighbor.gridPosition.y, 0f);
                Debug.DrawLine(roomPos, neighborPos, connectionColor);
            }

            // Draw "room" as a small cross on top with slight Z offset
            Vector3 crossPos = roomPos + new Vector3(0f, 0f, -1f); // slightly in front
            Color roomColor = GetRoomColor(room);
            Debug.DrawLine(crossPos + Vector3.up * gizmoSize, crossPos - Vector3.up * gizmoSize, roomColor);
            Debug.DrawLine(crossPos + Vector3.right * gizmoSize, crossPos - Vector3.right * gizmoSize, roomColor);
        }
    }

    private Color GetRoomColor(RoomNode room)
    {
        if (room.roomType == Room.RoomType.Enemy) return enemyRoomColor;
        else if (room.roomType == Room.RoomType.Treasure) return treasureRoomColor;
        else if (room.roomType == Room.RoomType.Start) return startRoomColor;
        else return bossRoomColor;
    }
}