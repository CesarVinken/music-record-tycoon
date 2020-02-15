using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string Id = "";
    public Room Room;
    public GameObject[] ClosedDoorWallPieces; // Wall to show when room is disabled
    public GameObject[] OpenDoorWallPieces; // Wall to show when room is enabled
    public bool IsAccessible;
    public PolygonCollider2D ClosedDoorCollider;
    public Door DoorConnection = null;

    void Awake()
    {
        if (Room == null)
        {
            Logger.Error(Logger.Initialisation, "Could not find parent room for door");
        }
        if (ClosedDoorWallPieces == null || ClosedDoorWallPieces.Length == 0)
        {
            Logger.Error(Logger.Initialisation, "Could not find ClosedDoorWallPieces for door");
        }
        if (OpenDoorWallPieces == null || OpenDoorWallPieces.Length == 0)
        {
            Logger.Error(Logger.Initialisation, "Could not find OpenDoorWallPieces for door");
        }
        if (ClosedDoorCollider == null)
        {
            Logger.Error(Logger.Initialisation, "Could not find ClosedDoorCollider for door");
        }
        IsAccessible = false;
        Room.AddDoorToRoom(this);
        Id = Guid.NewGuid().ToString();
    }

    public void OpenDoor()
    {
        Logger.Log("open door");
        Logger.Log("OpenDoorWallPieces length is {0}", OpenDoorWallPieces.Length);
        for (int i = 0; i < OpenDoorWallPieces.Length; i++)
        {
            OpenDoorWallPieces[i].SetActive(true);
        }
        for (int j = 0; j < ClosedDoorWallPieces.Length; j++)
        {
            ClosedDoorWallPieces[j].SetActive(false);
        }
        ClosedDoorCollider.enabled = false;    

        IsAccessible = true;
    }

    public void CloseDoor()
    {
        for (int i = 0; i < OpenDoorWallPieces.Length; i++)
        {
            OpenDoorWallPieces[i].SetActive(false);
        }
        for (int j = 0; j < ClosedDoorWallPieces.Length; j++)
        {
            ClosedDoorWallPieces[j].SetActive(true);
        }
        ClosedDoorCollider.enabled = true;

        IsAccessible = false;
    }
}
