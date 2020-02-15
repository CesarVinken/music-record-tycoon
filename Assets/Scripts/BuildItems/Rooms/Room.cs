using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RoomRotation
{
    Rotation0,
    Rotation90,
    Rotation180,
    Rotation270
}
public class Room : BuildItem
{
    private List<BuildingTile> _roomEdgeTiles = new List<BuildingTile>();

    public Dictionary<Direction, Vector2> RoomCorners;
    public RoomRotation RoomRotation;
    public PolygonCollider2D Collider;
    public RoomBlueprint RoomBlueprint;

    public List<Door> Doors = new List<Door>();
    public List<Room> AdjacentRooms = new List<Room>();
    public List<BuildingTile> RoomEdgeTiles = new List<BuildingTile>(); // possible optimisation: already divide pieces into correct side: upleft, downleft etc.
    public List<PlayerCharacter> CharactersInRoom = new List<PlayerCharacter>();
    public List<WallPiece> WallPieces;  // possible optimisation: already divide pieces into correct wall side: upleft, downleft etc.

    private DeleteRoomTrigger _deleteRoomTrigger;

    new public void Awake()
    {
        base.Awake();
        AdjacentRooms.Clear();
        CharactersInRoom.Clear();

        if (WallPieces.Count == 0) Logger.Warning("There are no wall pieces found for this room. Maybe they were not yet set up.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter character = collision.gameObject.GetComponent<PlayerCharacter>();
        if (character)
        {
            // A character entered the room
            Logger.Log(Logger.Locomotion, "{0} entered room {1}", character.Id, Id);
            character.EnterRoom(this);
            CharactersInRoom.Add(character);
            if (_deleteRoomTrigger)
            {
                _deleteRoomTrigger.HideDeleteRoomTrigger();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerCharacter character = collision.gameObject.GetComponent<PlayerCharacter>();
        if (character)
        {
            Logger.Log(Logger.Locomotion, "{0} left room {1}", character.Id, Id);
            if (character.CurrentRoom == this)
            {
                character.LeaveRoom();
            }
            foreach (PlayerCharacter c in CharactersInRoom)
            {
                if (c.Id == character.Id)
                {
                    Logger.Log(Logger.Locomotion, "Remove character {0} from room {1}", CharactersInRoom.Count, Id);
                    CharactersInRoom.Remove(c);
                    Logger.Log(Logger.Locomotion, "Removed character {0} from room {1}", CharactersInRoom.Count, Id);
                    if (CharactersInRoom.Count == 0 && _deleteRoomTrigger)
                    {
                        _deleteRoomTrigger.ShowDeleteRoomTrigger();
                    }
                    return;
                }
            }
        }
    }

    public void SetupCorners(Dictionary<Direction, Vector2> roomCorners)
    {
        if(RoomBlueprint.RightUpAxisLength % 3 != 0 || RoomBlueprint.LeftUpAxisLength % 3 != 0)
        {
            Logger.Error(Logger.Initialisation, "RightUpAxisLength ({0}) and LeftUpAxisLength ({1}) of room should always be divisible by 3", RoomBlueprint.RightUpAxisLength, RoomBlueprint.LeftUpAxisLength);
        }

        if (roomCorners.Count < 4)
        {
            Logger.Error(Logger.Initialisation, "There should be 4 roomCorners for this room");
        }
        //Logger.Log("Left corner");
        //Logger.Log(roomCorners[Direction.Left]);
        //Logger.Log("Down corner");
        //Logger.Log(roomCorners[Direction.Down]);
        RoomCorners = roomCorners;
    }

    public void SetupCollider(RoomBlueprint roomBlueprint)
    {
        if (RoomCorners.Count == 0) Logger.Error(Logger.Initialisation, "Room corders were not set up");

        Collider = gameObject.AddComponent<PolygonCollider2D>();

        Vector2 colliderPoint1 = GridHelper.CalculateLocationOnGrid(RoomBlueprint.RightUpAxisLength, 0);
        Vector2 colliderPoint2 = GridHelper.CalculateLocationOnGrid(RoomBlueprint.RightUpAxisLength, RoomBlueprint.LeftUpAxisLength);
        Vector2 colliderPoint3 = GridHelper.CalculateLocationOnGrid(0 , RoomBlueprint.LeftUpAxisLength);

        Vector2[] positions = new Vector2[] { new Vector2(0, 0), colliderPoint1, colliderPoint2, colliderPoint3, new Vector2(0, 0) };
        Collider.SetPath(0, positions);
    }

    public void SetAdjacentRooms()
    {
        Logger.Log("Set adjacent romos");
        for (int i = 0; i < _roomEdgeTiles.Count; i++)
        {
            if (_roomEdgeTiles[i].BuildingTileRooms.Count < 2) continue;
            for (int j = 0; j < _roomEdgeTiles[i].BuildingTileRooms.Count; j++)
            {
                Room otherRoom = _roomEdgeTiles[i].BuildingTileRooms[j];
                if (otherRoom != this && !this.AdjacentRooms.Contains(otherRoom))
                {
                    Logger.Log("Add {0} as adjacent room for {1}", RoomRotation, otherRoom.RoomRotation);
                    AddAdjacentRoom(otherRoom);
                    if (!otherRoom.AdjacentRooms.Contains(this))
                    {
                        Logger.Log("Add {1} as adjacent room for {0}", RoomRotation, otherRoom.RoomRotation);
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
        Logger.Warning(Logger.Building, "Enable doors for {0}, the NEW room", Id);
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
                        Logger.Log(Logger.Building, "Found a door to enable between {0} and {1}", Id, otherDoor.Room.Id);
                        Doors[i].OpenDoor();
                        otherDoor.OpenDoor(); //TODO: There should not be double wall pieces in the same location.
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
            Doors[i].DoorConnection.CloseDoor();
            Doors[i].DoorConnection.IsAccessible = false;
            Doors[i].DoorConnection.DoorConnection = null;
            Doors[i].DoorConnection = null;
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
                Vector2 location = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                tile.BuildingTileRooms.Add(this);
                if ((i == 0 && j < RoomBlueprint.LeftUpAxisLength) || (j == 0 && i < RoomBlueprint.RightUpAxisLength))
                {
                    _roomEdgeTiles.Add(tile);
                    tile.IsAvailable = Availability.Unavailable;
                }
                else if (i == RoomBlueprint.RightUpAxisLength || j == RoomBlueprint.LeftUpAxisLength)
                {
                    _roomEdgeTiles.Add(tile);
                    if(tile.IsAvailable == Availability.Available)
                        tile.IsAvailable = Availability.UpperEdge;
                }
                else
                {
                    tile.IsAvailable = Availability.Unavailable;
                }
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
            List<BuildingTile> tilesThatIncludeDeletedRoom = new List<BuildingTile>(); // all tiles that the deleted room was built on
            for (int j = RoomBlueprint.LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                if (tile.BuildingTileRooms.Count == 1)
                {
                    if (tile.BuildingTileRooms[0].Id == Id)
                    {
                        tile.IsAvailable = Availability.Available;
                        tilesThatIncludeDeletedRoom.Add(tile);
                        continue;
                    }
                }
                else if (tile.BuildingTileRooms.Count > 1)
                {
                    for (int p = 0; p < tile.BuildingTileRooms.Count; p++)
                    {
                        if (tile.BuildingTileRooms[p].Id == Id)
                        {
                            tilesThatIncludeDeletedRoom.Add(tile);

                            // complicated if statements to make sure the edges of remaining rooms get the correct availability status
                            if (tile.BuildingTileRooms.Count == 2 &&
                                (i == 0 || j == 0))
                            {
                                if (i == RoomBlueprint.RightUpAxisLength)
                                {
                                    Vector2 location1 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j - 3);
                                    BuildingTile tile1 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                                    Vector2 location2 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j + 3);
                                    BuildingTile tile2 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                                    if (tile1.IsAvailable != Availability.Unavailable && tile2.IsAvailable != Availability.Unavailable)
                                        tile.IsAvailable = Availability.UpperEdge;
                                }
                                else if (j == RoomBlueprint.LeftUpAxisLength)
                                {
                                    Vector2 location1 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i - 3, -j);
                                    BuildingTile tile1 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                                    Vector2 location2 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i + 3, -j);
                                    BuildingTile tile2 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                                    if (tile1.IsAvailable != Availability.Unavailable && tile2.IsAvailable != Availability.Unavailable)
                                        tile.IsAvailable = Availability.UpperEdge;
                                }
                                else
                                {
                                    tile.IsAvailable = Availability.UpperEdge;
                                }
                            }
                            else if (tile.BuildingTileRooms.Count == 3 &&
                            (i == 0 || j == 0))
                            {
                                if (i > 0)
                                {
                                    Vector2 location1 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j - 3);
                                    BuildingTile tile1 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                                    Vector2 location2 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i, -j + 3);
                                    BuildingTile tile2 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                                    if (tile1.IsAvailable != Availability.Unavailable && tile2.IsAvailable != Availability.Unavailable)
                                        tile.IsAvailable = Availability.UpperEdge;
                                }
                                else if (j > 0)
                                {
                                    Vector2 location1 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i - 3, -j);
                                    BuildingTile tile1 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                                    Vector2 location2 = GridHelper.CalculateLocationOnGrid(RoomCorners[Direction.Down], i + 3, -j);
                                    BuildingTile tile2 = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                                    if (tile1.IsAvailable != Availability.Unavailable && tile2.IsAvailable != Availability.Unavailable)
                                        tile.IsAvailable = Availability.UpperEdge;
                                }

                                else
                                {
                                    tile.IsAvailable = Availability.UpperEdge;
                                }
                            }
                            else if (tile.BuildingTileRooms.Count == 4)
                            {
                                if (location == new Vector2(RoomCorners[Direction.Down].x, RoomCorners[Direction.Down].y) &&
                                    RoomBlueprint.LeftUpAxisLength == 3 &&
                                    RoomBlueprint.RightUpAxisLength == 3)
                                {
                                    tile.IsAvailable = Availability.UpperEdge;
                                }
                            }
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

    public DeleteRoomTrigger GetDeleteRoomTrigger()
    {
        return _deleteRoomTrigger;
    }

    public void SetDeleteRoomTrigger(DeleteRoomTrigger deleteRoomTrigger)
    {
        _deleteRoomTrigger = deleteRoomTrigger;
    }

    public void LowerWallPieces()
    {
        // Lower walls in the down parts of the current room
        for (int i = 0; i < WallPieces.Count ; i++)
        {
            if (WallPieces[i].WallPieceType == WallPieceType.DownLeft || WallPieces[i].WallPieceType == WallPieceType.DownRight || WallPieces[i].WallPieceType == WallPieceType.CornerDown)
            {
                WallPieces[i].SetWallSprite(WallPieceDisplayMode.Transparent);
            }
        }

        // Lower the walls where needed in the upper walls for the adjacent rooms
        for (int j = 0; j < AdjacentRooms.Count; j++)
        {
            Room adjacentRoom = AdjacentRooms[j];
            if(adjacentRoom.RoomCorners[Direction.Down].x < RoomCorners[Direction.Down].x)
            {
                if (adjacentRoom.RoomCorners[Direction.Down].y < RoomCorners[Direction.Down].y) // This room is down and left of the current room
                {
                    for (int k = 0; k < adjacentRoom.WallPieces.Count; k++)
                    {
                        if (adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.UpRight || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerRight || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerUp)
                        {
                            // Don't turn off the whole wall, but only the parts that overlap with the current room.
                            if (adjacentRoom.WallPieces[k].transform.position.x < RoomCorners[Direction.Down].x && 
                                adjacentRoom.WallPieces[k].transform.position.x >= RoomCorners[Direction.Left].x - 1)   // the -1 is because the wall pieces might not be exactly positioned on the spot of the tile
                            {
                                adjacentRoom.WallPieces[k].SetWallSprite(WallPieceDisplayMode.Transparent);
                            }
                        }
                    }
                }
            }
            else if (adjacentRoom.RoomCorners[Direction.Down].x > RoomCorners[Direction.Down].x)
            {
                if(adjacentRoom.RoomCorners[Direction.Down].y < RoomCorners[Direction.Down].y) // This room is down and right of the current room
                {
                    for (int k = 0; k < adjacentRoom.WallPieces.Count; k++)
                    {
                        if (adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.UpLeft || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerLeft || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerUp)
                        {
                            // Don't turn off the whole wall, but only the parts that overlap with the current room.
                            if (adjacentRoom.WallPieces[k].transform.position.x > RoomCorners[Direction.Down].x &&
                               adjacentRoom.WallPieces[k].transform.position.x <= RoomCorners[Direction.Right].x + 1)    // the +1 is because the wall pieces might not be exactly positioned on the spot of the tile
                            {
                                adjacentRoom.WallPieces[k].SetWallSprite(WallPieceDisplayMode.Transparent);
                            }
                        }
                    }
                }
            }
        }
    }

    public void RaiseWallPieces(Room futureCurrentRoom = null)
    {
        for (int i = 0; i < WallPieces.Count; i++)
        {
            WallPieces[i].SetWallSprite(WallPieceDisplayMode.Visible);
        }

        for (int j = 0; j < AdjacentRooms.Count; j++)
        {
            Room adjacentRoom = AdjacentRooms[j];
            if (adjacentRoom.RoomCorners[Direction.Down].x < RoomCorners[Direction.Down].x)
            {
                if (adjacentRoom.RoomCorners[Direction.Down].y < RoomCorners[Direction.Down].y) // This room is down and left of the current room
                {
                    for (int k = 0; k < adjacentRoom.WallPieces.Count; k++)
                    {
                        if (adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.UpRight || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerRight || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerUp)
                        {
                            adjacentRoom.WallPieces[k].SetWallSprite(WallPieceDisplayMode.Visible);
                        }
                    }
                }
            }
            else if (adjacentRoom.RoomCorners[Direction.Down].x > RoomCorners[Direction.Down].x)
            {
                if (adjacentRoom.RoomCorners[Direction.Down].y < RoomCorners[Direction.Down].y) // This room is down and right of the current room
                {
                    for (int k = 0; k < adjacentRoom.WallPieces.Count; k++)
                    {
                        if (adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.UpLeft || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerLeft || adjacentRoom.WallPieces[k].WallPieceType == WallPieceType.CornerUp)
                        {
                            adjacentRoom.WallPieces[k].SetWallSprite(WallPieceDisplayMode.Visible);
                        }
                    }
                }
            }
        }
    }
}