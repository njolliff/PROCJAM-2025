using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "RoomLayout", menuName = "Scriptable Objects/Room Layout")]
public class RoomLayout : ScriptableObject
{
    public List<TileEntry> tiles = new();
    
    [System.Serializable]
    public class TileEntry
    {
        public Vector2Int position;
        public Tile tile;
        public Tile debugTile;
    }
}