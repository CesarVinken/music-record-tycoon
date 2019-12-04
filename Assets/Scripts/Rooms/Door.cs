using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string Id = "";
    public Room Room;
    public GameObject[] DisabledDoorWallPieces; // Wall to show when room is disabled
    public GameObject[] EnabledDoorWallPieces; // Wall to show when room is enabled
    public bool IsAccessible;
    public PolygonCollider2D DisabledDoorCollider;
    public Door DoorConnection = null;

    void Awake()
    {
        if (Room == null)
        {
            Debug.LogError("Could not find parent room for door");
        }
        if (DisabledDoorWallPieces == null || DisabledDoorWallPieces.Length == 0)
        {
            Debug.LogError("Could not find DisabledDoorWallPieces for door");
        }
        if (EnabledDoorWallPieces == null || EnabledDoorWallPieces.Length == 0)
        {
            Debug.LogError("Could not find EnabledDoorWallPieces for door");
        }
        if (DisabledDoorCollider == null)
        {
            Debug.LogError("Could not find DisabledDoorCollider for door");
        }
        IsAccessible = false;
        Room.AddDoorToRoom(this);
        Id = Guid.NewGuid().ToString();
    }

    public void EnableDoor()
    {
        for (int i = 0; i < EnabledDoorWallPieces.Length; i++)
        {
            EnabledDoorWallPieces[i].SetActive(true);
        }
        for (int j = 0; j < DisabledDoorWallPieces.Length; j++)
        {
            DisabledDoorWallPieces[j].SetActive(false);
        }
        DisabledDoorCollider.enabled = false;
        IsAccessible = true;
    }

    public void DisableDoor()
    {
        for (int i = 0; i < EnabledDoorWallPieces.Length; i++)
        {
            EnabledDoorWallPieces[i].SetActive(false);
        }
        for (int j = 0; j < DisabledDoorWallPieces.Length; j++)
        {
            DisabledDoorWallPieces[j].SetActive(true);
        }
        DisabledDoorCollider.enabled = true;
        IsAccessible = false;
    }
}
