using UnityEngine;
using System.Collections.Generic;

public class RoomGraphRuntimeDebugger : MonoBehaviour
{
    public List<RoomNode> rooms;

    [Header("Generation")]
    public int numRooms;
    public float branchChance, extraConnectionChance;

    [Header("Debug")]
    public float gizmoSize = 0.25f;
    public Color roomColor = Color.green;
    public Color connectionColor = Color.yellow;

    void Awake()
    {
        rooms = RoomGraphGenerator.GenerateGraph(numRooms, branchChance, extraConnectionChance);
    }

    void Update()
    {
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
            Debug.DrawLine(crossPos + Vector3.up * gizmoSize, crossPos - Vector3.up * gizmoSize, roomColor);
            Debug.DrawLine(crossPos + Vector3.right * gizmoSize, crossPos - Vector3.right * gizmoSize, roomColor);
        }
    }

}
