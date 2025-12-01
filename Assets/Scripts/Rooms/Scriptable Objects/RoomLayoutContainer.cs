using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomLayoutContainer", menuName = "Scriptable Objects/Room Layout Container")]
public class RoomLayoutContainer : ScriptableObject
{
    public List<RoomLayout> roomLayouts = new();

    public RoomLayout GetRandomLayout()
    {
        if (roomLayouts.Count == 0) return null;

        return roomLayouts[Random.Range(0, roomLayouts.Count)];
    }
}