using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public static List<Room> Rooms;
    public static List<RoomObjectGO> RoomObjectGOs;

    void Awake()
    {
        Instance = this;

        Rooms = new List<Room>();
        RoomObjectGOs = new List<RoomObjectGO>();
    }

    public void AddRoom(Room Room)
    {
        Rooms.Add(Room);

        for (int i = 0; i < Room.RoomObjects.Count; i++)
        {
            RoomObjectGO roomObjectGO = Room.RoomObjects[i];
            roomObjectGO.Register();
        }
    }

    public void RemoveRoom(Room Room)
    {
        Rooms.Remove(Room);

        for (int i = Room.RoomObjects.Count - 1; i >= 0; i--)
        {
            RoomObjectGO roomObjectGO = Room.RoomObjects[i];
            roomObjectGO.Deregister();
        }
    }

    public void DeleteAllRooms()
    {
        for (int i = Rooms.Count - 1; i >= 0; i--)
        {
            BuilderManager.Instance.DeleteRoom(Rooms[i]);
        }
    }
}
