using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simply sets the player's position to a randomly selected entry point from a given list of transforms on Awake().
/// </summary>
public class PlayerRandomEntryStart : MonoBehaviour
{
    public List<Transform> entryPoints;

    void Awake()
    {
        transform.position = entryPoints[Random.Range(0, entryPoints.Count - 1)].position;
    }
}