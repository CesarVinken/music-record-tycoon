using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public static List<Room> Rooms;

    void Awake()
    {
        Instance = this;

        Rooms = new List<Room>();
    }

    public void AddRoom(Room Room)
    {
        Rooms.Add(Room);
    }

    public void RemoveRoom(Room Room)
    {
        Rooms.Remove(Room);
    }

    public void EnableDoor(Door newDoor)
    {
        foreach (Room room in Rooms)
        {
            foreach (KeyValuePair<Door, bool> door in room.Doors)
            {
                Debug.Log("find doors and enable when needed");
            }
        }
    }
}
