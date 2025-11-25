using System.Collections.Generic;
using UnityEngine;

public static class RoomGraphGenerator
{
    /// <summary>
    /// Returns a list of RoomNodes. All rooms will be reachable, but not all adjacent rooms will connect.
    /// </summary>
    /// <param name="roomCount">The number of rooms to generate.</param>
    /// <param name="branchChance">(0-1) Chance to branch instead of continuing walk.</param>
    /// <param name="extraDoorChance">Chance to create extra connections between rooms after Prim's.</param>
    /// <returns></returns>
    public static List<RoomNode> GenerateGraph(int roomCount, float branchChance, float extraConnectionChance)
    {
        // Get a graph of randomly placed rooms
        List<RoomNode> roomGraph = BuildBaseGraph(roomCount, branchChance);

        // Build MST connections using Prim's algorithm
        BuildMSTConnections(roomGraph);

        // Add extra connections
        AddExtraConnections(roomGraph, extraConnectionChance);

        // Return finalized graph
        return roomGraph;
    }

    private static List<RoomNode> BuildBaseGraph(int roomCount, float branchChance)
    {
        Dictionary<Vector2Int, RoomNode> placed = new(); // Keeps track of placed nodes
        List<RoomNode> freeRooms = new(); // Keeps track of rooms that have an open direction to walk in
        List<RoomNode> allRooms = new(); // Graph of nodes to return

        // Array of Vector2Int directions for random movement selection
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        // Create the starting RoomNode
        Vector2Int startPos = Vector2Int.zero;
        RoomNode startRoom = new RoomNode { gridPosition = startPos };
        placed[startPos] = startRoom;
        freeRooms.Add(startRoom);
        allRooms.Add(startRoom);

        // Place remaining nodes by randomly walking/branching from the start
        while (allRooms.Count < roomCount)
        {
            // Randomly branch or continue walk
            RoomNode baseRoom = (Random.value < branchChance)
            ? freeRooms[Random.Range(0, freeRooms.Count)] // If branching, set baseRoom to a random room
            : freeRooms[freeRooms.Count - 1]; // Otherwise, continue from the most recently placed room

            // Collect free directions
            List<Vector2Int> freeNeighbors = new();
            foreach (var dir in directions)
            {
                Vector2Int newPos = baseRoom.gridPosition + dir;
                if (!placed.ContainsKey(newPos))
                    freeNeighbors.Add(newPos);
            }

            // If there are no free directions, remove the node from freeRooms and try again from a new node
            if (freeNeighbors.Count == 0)
            {
                freeRooms.Remove(baseRoom);
                continue;
            }

            // Pick a free position
            Vector2Int chosenPos = freeNeighbors[Random.Range(0, freeNeighbors.Count)];

            // Create room
            RoomNode newRoom = new RoomNode { gridPosition = chosenPos };
            placed[chosenPos] = newRoom;
            freeRooms.Add(newRoom);
            allRooms.Add(newRoom);

            // Record adjacency for potential connections
            foreach (var dir in directions)
            {
                Vector2Int neighborPos = chosenPos + dir;
                if (placed.TryGetValue(neighborPos, out RoomNode neighbor))
                {
                    newRoom.potentialNeighbors.Add(neighbor);
                    neighbor.potentialNeighbors.Add(newRoom);
                }
            }
        }

        return allRooms;
    }

    private static void BuildMSTConnections(List<RoomNode> roomGraph)
    {
        HashSet<RoomNode> connected = new();
        HashSet<RoomNode> frontier = new();

        RoomNode start = roomGraph[0];
        connected.Add(start);

        foreach(RoomNode room in start.potentialNeighbors)
            frontier.Add(room);

        while (connected.Count < roomGraph.Count)
        {
            // Pick a random frontier room
            RoomNode next = GetRandom(frontier);

            // Connect to one of its connected potential neighbors
            List<RoomNode> candidates = next.potentialNeighbors.FindAll(n => connected.Contains(n));
            RoomNode chosenNeighbor = candidates[Random.Range(0, candidates.Count)];

            // Create connection
            CreateConnection(next, chosenNeighbor);

            // Move into connected set
            connected.Add(next);
            frontier.Remove(next);

            // Add new frontier options
            foreach (RoomNode pn in next.potentialNeighbors)
                if (!connected.Contains(pn))
                    frontier.Add(pn);
        }
    }

    private static void AddExtraConnections(List<RoomNode> roomGraph, float extraConnectionChance)
    {
        // Roll for a chance to create a connection to each potential neighbor for each room
        foreach (RoomNode room in roomGraph)
        {
            List<RoomNode> potentialNeighborsCopy = new List<RoomNode>(room.potentialNeighbors);

            foreach (RoomNode pn in potentialNeighborsCopy)
                if (Random.value < extraConnectionChance)
                    CreateConnection(room, pn);
        }
    }

    #region Helper Methods
    private static RoomNode GetRandom(HashSet<RoomNode> set)
    {
        int i = Random.Range(0, set.Count);
        foreach (RoomNode roomNode in set)
        {
            if (i-- == 0)
                return roomNode;
        }
        return null;
    }
    private static void CreateConnection(RoomNode n1, RoomNode n2)
    {
        if (!n1.potentialNeighbors.Contains(n2) || !n2.potentialNeighbors.Contains(n1))
        {
            Debug.Log("ERROR: Tried to create connection between two nodes that do not consider each other potential neighbors.");
            return;
        }

        // Move n2 from n1's potentialNeighbors list to its neighbors list
        n1.potentialNeighbors.Remove(n2);
        n1.neighbors.Add(n2);

        // Move n1 from n2's potentialNeighbors list to its neighbors list
        n2.potentialNeighbors.Remove(n1);
        n2.neighbors.Add(n1);
    }
    #endregion
}