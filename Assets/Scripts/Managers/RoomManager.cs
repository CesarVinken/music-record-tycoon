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

    public void DeleteAllRooms()
    {
        for (int i = Rooms.Count - 1; i >= 0; i--)
        {
            BuilderManager.Instance.DeleteRoom(Rooms[i]);
        }
    }
}
