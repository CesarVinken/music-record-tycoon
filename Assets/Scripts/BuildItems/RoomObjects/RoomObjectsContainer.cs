using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectsContainer : MonoBehaviour
{
    public List<RoomObjectGO> RoomObjects = new List<RoomObjectGO>();
    public Room Room;

    public void Awake()
    {
        if (Room == null)
            Logger.Error(Logger.Initialisation, "Could not find Room on {0}", gameObject.name);

        for (int i = 0; i < RoomObjects.Count; i++)
        {
            Room.RoomObjects.Add(RoomObjects[i]);
        }
    }

    public void InitialiseRoomObjects()
    {
        if (RoomObjects.Count == 0)
        {
            Logger.Warning("Found 0 Room objects for room {0}. Possibly the room objects need to be added to the container.", Room.Id);
        }

        for (int i = 0; i < RoomObjects.Count; i++)
        {
            if (RoomObjects[i].RoomObject == null)
            {
                Logger.Warning("Could not find a scriptable room object for the object {0}", RoomObjects[i].gameObject.name );
                continue;
            }
            RoomObjectBlueprint roomObjectBlueprint = RoomObjectBlueprint.CreateBlueprint(RoomObjects[i].RoomObject.RoomObjectName);
            RoomObjects[i].Initialise(roomObjectBlueprint, Room.RoomRotation, Room);
        }
    }

    public void AddRoomObject(RoomObjectGO roomObject)
    {
        RoomObjects.Add(roomObject);
    }
}
