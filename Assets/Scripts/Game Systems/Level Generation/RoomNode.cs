using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public Vector2Int gridPosition;
    public List<RoomNode> neighbors = new();
    public List<RoomNode> potentialNeighbors = new();
    public enum RoomType { Normal, Treasure, Start, Boss };
    public RoomType roomType;
}