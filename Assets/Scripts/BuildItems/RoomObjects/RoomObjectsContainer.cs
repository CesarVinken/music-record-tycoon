using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectsContainer : MonoBehaviour
{
    public List<RoomObject> RoomObjects = new List<RoomObject>();

    public void AddRoomObject(RoomObject roomObject)
    {
        RoomObjects.Add(roomObject);
    }
}
