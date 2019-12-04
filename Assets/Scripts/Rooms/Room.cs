using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string Id = "";

    private List<BuildingTile> _roomEdgeTiles = new List<BuildingTile>();

    public List<BuildingTile> RoomEdgeTiles = new List<BuildingTile>();

    public List<Room> AdjacentRooms = new List<Room>();

    public Dictionary<Direction, Vector2> RoomCorners;
    public List<Door> Doors = new List<Door>();

    public void Awake()
    {
        Id = Guid.NewGuid().ToString();
        AdjacentRooms.Clear();
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
                if (otherRoom != this && !this.AdjacentRooms.Contains(otherRoom))
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
        AdjacentRooms.Add(adjacentRoom);
    }

    public void RemoveThisRoomFromAdjacentRooms()
    {
        for (int i = 0; i < AdjacentRooms.Count; i++)
        {
            Room adjacentRoom = AdjacentRooms[i];  //adjacent to me

            List<Room> _tempAdjacentRooms = new List<Room>();

            for (int j = 0; j < adjacentRoom.AdjacentRooms.Count; j++)  // find removed room in this room
            {
                if (adjacentRoom.AdjacentRooms[j].Id != Id)
                {
                    _tempAdjacentRooms.Add(adjacentRoom.AdjacentRooms[j]);
                }
            }

            adjacentRoom.AdjacentRooms = _tempAdjacentRooms;
        }
        AdjacentRooms.Clear();
    }

    public void EnableDoors()
    {
        Debug.LogWarning("Enable doors for " + Id + ", the NEW room");
        for (int i = 0; i < Doors.Count; i++)
        {
            Vector3 doorPosition = Doors[i].transform.position;
            for (int j = 0; j < AdjacentRooms.Count; j++)
            {
                Room adjacentRoom = AdjacentRooms[j];
                for (int k = 0; k < adjacentRoom.Doors.Count; k++)
                {
                    Door otherDoor = adjacentRoom.Doors[k];

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
            Doors[i].DoorConnection.DisableDoor();
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

    // Make tiles available again for building
    public void CleanUpDeletedRoomTiles()
    {
        List<BuildingTile> roomSquareTiles = BuilderManager.Instance.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= RoomCorners[Direction.Left].x &&
            tile.StartingPoint.x <= RoomCorners[Direction.Right].x &&
            tile.StartingPoint.y <= RoomCorners[Direction.Up].y &&
            tile.StartingPoint.y >= RoomCorners[Direction.Down].y
            );

        for (int i = 0; i <= RoomBlueprint.RightUpAxisLength; i += 3)
        {
            List<BuildingTile> tilesThatIncludeDeletedRoom = new List<BuildingTile>();
            for (int j = RoomBlueprint.LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = BuilderManager.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                if (tile.BuildingTileRooms.Count == 1)
                {
                    if (tile.BuildingTileRooms[0].Id == Id)
                    {
                        tile.IsAvailable = true;
                        tilesThatIncludeDeletedRoom.Add(tile);
                        continue;
                    }
                }
                if (tile.BuildingTileRooms.Count > 1)
                {
                    for (int p = 0; p < tile.BuildingTileRooms.Count; p++)
                    {
                        if (tile.BuildingTileRooms[p].Id == Id)
                        {
                            tilesThatIncludeDeletedRoom.Add(tile);
                            continue;
                        }
                    }
                }
            }
            for (int k = 0; k < tilesThatIncludeDeletedRoom.Count; k++)
            {
                BuildingTile tile = tilesThatIncludeDeletedRoom[k];
                List<Room> tempBuildingTileRooms = new List<Room>();

                for (int l = 0; l < tile.BuildingTileRooms.Count; l++)
                {
                    if (tile.BuildingTileRooms[l].Id != Id) tempBuildingTileRooms.Add(tile.BuildingTileRooms[l]);
                }
                tile.BuildingTileRooms = tempBuildingTileRooms;
            }
        }

    }

    public void AddDoorToRoom(Door door)
    {
        Doors.Add(door);
    }
    
    public void DeleteRoom()
    {
        RoomManager.Instance.RemoveRoom(this);
        Destroy(gameObject);
        Destroy(this);
    }
}