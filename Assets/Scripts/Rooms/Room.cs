﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
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
    public Dictionary<Door, bool> Doors;

    public void Awake()
    {
        Doors = new Dictionary<Door, bool>();
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

    public void EnableDoors()
    {
        foreach (KeyValuePair<Door, bool> door in Doors)
        {
            Vector3 doorPosition = door.Key.transform.position;
            for (int i = 0; i < _adjacentRooms.Count; i++)
            {
                foreach (KeyValuePair<Door, bool> otherDoor in _adjacentRooms[i].Doors)
                {
                    if (doorPosition == otherDoor.Key.transform.position)
                    {
                        door.Key.EnableDoor();
                        otherDoor.Key.EnableDoor(); //TODO: There should not be double wall pieces in the same location.
                    }
                }
            }
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

    public void AddDoorToDictionary(Door door, bool IsAccessible)
    {
        Doors.Add(door, IsAccessible);
    }
}