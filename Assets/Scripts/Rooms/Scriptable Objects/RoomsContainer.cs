using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomsContainer", menuName = "Scriptable Objects/Rooms Container")]
public class RoomsContainer : ScriptableObject
{
    public List<GameObject> northRooms;
    public List<GameObject> eastRooms;
    public List<GameObject> southRooms;
    public List<GameObject> westRooms;

    private List<Door.Direction> _allDirections = new()
    {
        Door.Direction.North,
        Door.Direction.East,
        Door.Direction.South,
        Door.Direction.West,
    };

    public GameObject GetAppropriateRoom(List<Door.Direction> openingDirections)
    {
        // Make sure at least one direction is given
        if (openingDirections.Count == 0) return null;

        // Initialize candidates with rooms from the first listed direction
        List<GameObject> candidates = GetRoomsWithDirection(openingDirections[0]);

        // Find the intersection with other specified directions
        foreach (var dir in openingDirections)
        {
            // Skip the first direction since it is already in candidates
            if (dir == openingDirections[0]) continue;

            List<GameObject> dirRooms = GetRoomsWithDirection(dir);
            candidates = candidates.Where(room => dirRooms.Contains(room)).ToList();
        }

        // Determine which directions should not have a door
        List<Door.Direction> excludedDirections = _allDirections.Where(dir => !openingDirections.Contains(dir)).ToList();

        // Take out the rooms with excluded directions
        foreach (Door.Direction dir in excludedDirections)
        {
            List<GameObject> excludedRooms = GetRoomsWithDirection(dir);
            candidates = candidates.Where(room => !excludedRooms.Contains(room)).ToList();
        }

        // List should be narrowed down to 1 room to return
        return candidates.Count > 0 ? candidates[0] : null;
    }

    private List<GameObject> GetRoomsWithDirection(Door.Direction dir)
    {
        if (dir == Door.Direction.North)
            return northRooms;
        else if (dir == Door.Direction.East)
            return eastRooms;
        else if (dir == Door.Direction.South)
            return southRooms;
        else
            return westRooms;
    }
}