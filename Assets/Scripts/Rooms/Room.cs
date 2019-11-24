using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string Id = "";

    private List<BuildingTile> _roomEdgeTiles = new List<BuildingTile>();
    private List<Room> _adjacentRooms = new List<Room>();

    public List<Room> AdjacentRooms
    {
        get { return _adjacentRooms; }
        private set
        {
            AdjacentRooms = _adjacentRooms;
        }
    }

    public Dictionary<Direction, Vector2> RoomCorners;
    //public Dictionary<Door, Door> Doors;    // this door, connectedDoor. ConnectedDoor is null by default
    public List<Door> Doors = new List<Door>();

    public void Awake()
    {
        Doors = new List<Door>();
        Id = Guid.NewGuid().ToString();
    }

    public void SetupCorners(Dictionary<Direction, Vector2> roomCorners)
    {
        if(RoomBlueprint.RightUpAxisLength % 3 != 0 || RoomBlueprint.LeftUpAxisLength % 3 != 0)
        {
            Debug.LogError($"RightUpAxisLength ({RoomBlueprint.RightUpAxisLength}) and LeftUpAxisLength ({RoomBlueprint.LeftUpAxisLength}) of room should always be divisible by 3");
        }

        if (roomCorners.Count < 4)
        {
            Debug.LogError("There should be 4 roomCorners for this room");
        }

        RoomCorners = roomCorners;
    }

    public void setAdjacentRooms()
    {
        Debug.Log("Set adjacent rooms");
        for (int i = 0; i < _roomEdgeTiles.Count; i++)
        {
            if (_roomEdgeTiles[i].BuildingTileRooms.Count < 2) continue;
            for (int j = 0; j < _roomEdgeTiles[i].BuildingTileRooms.Count; j++)
            {
                Room otherRoom = _roomEdgeTiles[i].BuildingTileRooms[j];
                if (otherRoom != this && !this._adjacentRooms.Contains(otherRoom))
                {
                    AddAdjacentRoom(otherRoom);
                    if (!otherRoom.AdjacentRooms.Contains(this))
                    {
                        otherRoom.AddAdjacentRoom(this);
                    }
                }
            }
        }
    }

    public void AddAdjacentRoom(Room adjacentRoom)
    {
        _adjacentRooms.Add(adjacentRoom);
    }

    public void RemoveFromAdjacentRooms(Room room)
    {
        List<Room> _tempAdjacentRooms = new List<Room>();

        for (int i = 0; i < _adjacentRooms.Count; i++)
        {
            if (_adjacentRooms[i].Id != room.Id) _tempAdjacentRooms.Add(_adjacentRooms[i]);
        }

        _adjacentRooms = _tempAdjacentRooms;
    }

    public void UpdateAdjacentRooms()
    {
        for (int i = 0; i < _adjacentRooms.Count; i++)
        {
            RemoveFromAdjacentRooms(this);
        }
    }

    public void EnableDoors()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Vector3 doorPosition = Doors[i].transform.position;
            for (int j = 0; j < _adjacentRooms.Count; j++)
            {
                for (int k = 0; k < _adjacentRooms[j].Doors.Count; k++)
                {
                    Door otherDoor = _adjacentRooms[j].Doors[k];
                    if (doorPosition == otherDoor.transform.position)
                    {
                        Doors[i].DoorConnection = otherDoor;
                        otherDoor.DoorConnection = Doors[i];

                        Doors[i].EnableDoor();
                        otherDoor.EnableDoor(); //TODO: There should not be double wall pieces in the same location.
                    }
                }
            }
        }
    }

    public void RemoveDoorConnectionFromAdjacentRooms()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].IsAccessible = false;

            if (Doors[i].DoorConnection == null) continue;
            Doors[i].DoorConnection.IsAccessible = false;
            Doors[i].DoorConnection.DoorConnection = null;
            Doors[i].DoorConnection = null;

            // This is probably not needed
            //foreach (KeyValuePair<Door, Door>otherDoor in door.Value.Room.Doors)
            //{
            //    if (door.Key.Id == otherDoor.Key.Id)
            //    {
            //        otherDoor.Key.IsAccessible = false;
            //        otherDoor.Value.IsAccessible = false;
            //    }
            //}
        }
    }

    public List<BuildingTile> GetRoomEdgeTiles()
    {
        // Get all building tiles in the location of the room and make them UNAVAILABLE
        List<BuildingTile> roomSquareTiles = BuilderManager.Instance.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= RoomCorners[Direction.Left].x &&
            tile.StartingPoint.x <= RoomCorners[Direction.Right].x &&
            tile.StartingPoint.y <= RoomCorners[Direction.Up].y &&
            tile.StartingPoint.y >= RoomCorners[Direction.Down].y
        );

        for (int i = 0; i <= RoomBlueprint.RightUpAxisLength; i += 3)
        {
            for (int j = RoomBlueprint.LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = BuilderManager.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                tile.IsAvailable = false;
                tile.BuildingTileRooms.Add(this);
                if (i == 0 || i == RoomBlueprint.RightUpAxisLength || j == 0 || j == RoomBlueprint.LeftUpAxisLength)
                    _roomEdgeTiles.Add(tile);
            }
        }

        return _roomEdgeTiles;
    }

    public void AddDoorToRoom(Door door)
    {
        Doors.Add(door);
    }
    
    public void DeleteRoom()
    {
        RoomManager.Instance.RemoveRoom(this);
        Destroy(gameObject);
    }
}