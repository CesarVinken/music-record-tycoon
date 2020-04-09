using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : BuildItem
{
    public List<BuildingTile> RoomEdgeTilesPerCluster = new List<BuildingTile>();    // every 3 tiles

    public RoomObjectsContainer RoomObjectsContainer;

    public Dictionary<Direction, Vector2> RoomCorners;
    public ObjectRotation RoomRotation;
    public PolygonCollider2D Collider;
    public RoomBlueprint RoomBlueprint;

    public List<Door> Doors = new List<Door>();
    public List<Room> AdjacentRooms = new List<Room>();
    public List<Character> CharactersInRoom = new List<Character>();
    public List<WallPiece> WallPieces;  // possible optimisation: already divide pieces into correct wall side: upleft, downleft etc.

    private DeleteRoomTrigger _deleteRoomTrigger;

    new public void Initialise()
    {
        base.Initialise();
        AdjacentRooms.Clear();
        CharactersInRoom.Clear();

        if (RoomObjectsContainer == null)
            Logger.Error(Logger.Initialisation, "Could not find RoomObjectsContainer script");

        if (WallPieces.Count == 0) Logger.Warning("There are no wall pieces found for this room. Maybe they were not yet set up.");

        for (int i = 0; i < WallPieces.Count; i++)
        {
            WallPieces[i].Room = this;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();
        if (character)
        {
            Logger.Log(Logger.Locomotion, "{0} entered room {1}", character.Id, Id);
            CharactersInRoom.Add(character);
            character.EnterRoom(this);
            if (_deleteRoomTrigger)
            {
                _deleteRoomTrigger.HideDeleteRoomTrigger();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();
        if (character)
        {
            Logger.Log(Logger.Locomotion, "{0} left room {1}", character.Id, Id);
            if (character.CurrentRoom == this)
            {
                character.LeaveRoom();
            }
            foreach (Character c in CharactersInRoom)
            {
                if (c.Id == character.Id)
                {
                    Logger.Log(Logger.Locomotion, "Remove character {0} from room {1}", CharactersInRoom.Count, Id);
                    CharactersInRoom.Remove(c);
                    if (CharactersInRoom.Count == 0 && _deleteRoomTrigger)
                    {
                        _deleteRoomTrigger.ShowDeleteRoomTrigger();
                    }
                    break;
                }
            }
            if (CharactersInRoom.Count == 0)
            {
                Logger.Log("Raise wall pieces of room {0}", Id);
                RaiseWallPieces();
            }
        }
    }

    public void SetupCorners(Dictionary<Direction, Vector2> roomCorners)
    {
        if (RoomBlueprint.RightUpAxisLength % 3 != 0 || RoomBlueprint.LeftUpAxisLength % 3 != 0)
        {
            Logger.Error(Logger.Initialisation, "RightUpAxisLength ({0}) and LeftUpAxisLength ({1}) of room should always be divisible by 3", RoomBlueprint.RightUpAxisLength, RoomBlueprint.LeftUpAxisLength);
        }

        if (roomCorners.Count < 4)
        {
            Logger.Error(Logger.Initialisation, "There should be 4 roomCorners for this room");
        }
        RoomCorners = roomCorners;
    }

    public void SetupCollider()
    {
        if (RoomCorners.Count == 0) Logger.Error(Logger.Initialisation, "Room corders were not set up");

        int rightUpAxisLength = GetRightUpAxisLengthForRoomRotation();
        int leftUpAxisLength = GetLeftUpAxisLengthForRoomRotation();

        Collider = gameObject.AddComponent<PolygonCollider2D>();

        Vector2 colliderPoint1 = GridHelper.GridToVectorLocation(rightUpAxisLength, 0);
        Vector2 colliderPoint2 = GridHelper.GridToVectorLocation(rightUpAxisLength, leftUpAxisLength);
        Vector2 colliderPoint3 = GridHelper.GridToVectorLocation(0, leftUpAxisLength);

        Vector2[] positions = new Vector2[] { new Vector2(0, 0), colliderPoint1, colliderPoint2, colliderPoint3, new Vector2(0, 0) };
        Collider.SetPath(0, positions);
    }

    public void SetupRoomObjects()
    {
        for (int i = 0; i < RoomBlueprint.RoomObjects.Length; i++)
        {
            BuilderManager.Instance.BuildRoomObject(RoomBlueprint.RoomObjects[i].RoomObjectBlueprint, RoomBlueprint.RoomObjects[i].RoomObjectLocation, this);
        }
    }

    public void SetAdjacentRooms()
    {
        for (int i = 0; i < RoomEdgeTilesPerCluster.Count; i++)
        {
            if (RoomEdgeTilesPerCluster[i].BuildingTileRooms.Count < 2) continue;
            for (int j = 0; j < RoomEdgeTilesPerCluster[i].BuildingTileRooms.Count; j++)
            {
                Room otherRoom = RoomEdgeTilesPerCluster[i].BuildingTileRooms[j];
                if (otherRoom != this && !this.AdjacentRooms.Contains(otherRoom))
                {
                    Logger.Log(Logger.Building, "Add {0}.{1} as adjacent room for {2}.{3}", RoomBlueprint.Name, RoomRotation, otherRoom.RoomBlueprint.Name, otherRoom.RoomRotation);
                    AddAdjacentRoom(otherRoom);
                    if (!otherRoom.AdjacentRooms.Contains(this))
                    {
                        Logger.Log(Logger.Building, "Add {0}.{1} as adjacent room for {2}.{3}", RoomBlueprint.Name, RoomRotation, otherRoom.RoomBlueprint.Name, otherRoom.RoomRotation);
                        otherRoom.AddAdjacentRoom(this);
                    }
                }
            }
        }
    }

    public List<WallPiece> GetWallpiecesByActiveBuildingTile(Vector2 overlapPosition, Room room)
    {
        List<WallPiece> overlappingWallPiece = room.WallPieces.Where(wallPiece =>
        {
            Vector2 wallPiecePosition = new Vector2(wallPiece.transform.position.x, wallPiece.transform.position.y);
            return wallPiece.gameObject.activeSelf && wallPiecePosition == overlapPosition;
        }).ToList();

        return overlappingWallPiece;
    }

    public List<WallPiece> GetWallpiecesByAnyBuildingTile(Vector2 overlapPosition, Room room)
    {
        List<WallPiece> overlappingWallPiece = room.WallPieces.Where(wallPiece =>
        {
            Vector2 wallPiecePosition = new Vector2(wallPiece.transform.position.x, wallPiece.transform.position.y);
            return wallPiecePosition == overlapPosition;
        }).ToList();

        return overlappingWallPiece;
    }

    private void DisableWallpiece(Vector2 overlapPosition, Room thisRoom, Room otherRoom)   // thisRoom = new room --|-- otherRoom = room adjacent to the new room
    {
        List<WallPiece> overlappingWallPiecesThisRoom = GetWallpiecesByActiveBuildingTile(overlapPosition, thisRoom);
        List<WallPiece> overlappingWallPiecesOtherRoom = GetWallpiecesByActiveBuildingTile(overlapPosition, otherRoom);

        for (int i = 0; i < overlappingWallPiecesThisRoom.Count; i++)
        {
            WallPiece overlappingWallPieceThisRoom = overlappingWallPiecesThisRoom[i];
            for (int j = 0; j < overlappingWallPiecesOtherRoom.Count; j++)
            {
                WallPiece overlappingWallPieceOtherRoom = overlappingWallPiecesOtherRoom[j];
                if (overlappingWallPieceOtherRoom && overlappingWallPieceThisRoom)
                {
                    if ((overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.DownRight || overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceThisRoom.WallPieceType == WallPieceType.UpLeft)
                    {
                        overlappingWallPieceOtherRoom.gameObject.SetActive(false);
                    }

                    if ((overlappingWallPieceThisRoom.WallPieceType == WallPieceType.DownRight || overlappingWallPieceThisRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.UpLeft)
                    {
                        overlappingWallPieceThisRoom.gameObject.SetActive(false);
                    }

                    if ((overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.DownLeft || overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceThisRoom.WallPieceType == WallPieceType.UpRight)
                    {
                        overlappingWallPieceOtherRoom.gameObject.SetActive(false);
                    }

                    if ((overlappingWallPieceThisRoom.WallPieceType == WallPieceType.DownLeft || overlappingWallPieceThisRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.UpRight)
                    {
                        overlappingWallPieceThisRoom.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void DisableOverlappingWallPieces()
    {
        //get wall pieces with overlap. Then wall pieces against edge tiles of all other rooms
        List<BuildingTile> edgeTileClustersWithOverlap = RoomEdgeTilesPerCluster.Where(tile => tile.BuildingTileRooms.Count > 1).ToList();

        for (int i = 0; i < edgeTileClustersWithOverlap.Count; i++)
        {
            BuildingTile tileWithOverlap = edgeTileClustersWithOverlap[i];
            for (int j = 0; j < tileWithOverlap.BuildingTileRooms.Count; j++)
            {
                Room otherRoom = tileWithOverlap.BuildingTileRooms[j];
                if (otherRoom == this) continue;

                for (int k = 0; k < otherRoom.RoomEdgeTilesPerCluster.Count; k++)
                {
                    BuildingTile otherRoomTile = otherRoom.RoomEdgeTilesPerCluster[k];
                    // the tile in this room and the same tile in the other room. Now find the common wall pieces
                    if (otherRoomTile.StartingPoint.x == tileWithOverlap.StartingPoint.x)
                    {
                        //Logger.Warning("we found the common tile!!  {0},{1}", tileWithOverlap.StartingPoint.x, tileWithOverlap.StartingPoint.y);
                        DisableWallpiece(tileWithOverlap.StartingPoint, this, otherRoom);
                        DisableWallpiece(new Vector2(tileWithOverlap.StartingPoint.x + 5, tileWithOverlap.StartingPoint.y + 2.5f), this, otherRoom);
                        DisableWallpiece(new Vector2(tileWithOverlap.StartingPoint.x + 10, tileWithOverlap.StartingPoint.y + 5f), this, otherRoom);
                        DisableWallpiece(new Vector2(tileWithOverlap.StartingPoint.x + 5, tileWithOverlap.StartingPoint.y - 2.5f), this, otherRoom);
                        DisableWallpiece(new Vector2(tileWithOverlap.StartingPoint.x + 10, tileWithOverlap.StartingPoint.y - 5f), this, otherRoom);
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

        int rightUpAxisLength = GetRightUpAxisLengthForRoomRotation();
        int leftUpAxisLength = GetLeftUpAxisLengthForRoomRotation();

        for (int i = 0; i <= rightUpAxisLength; i += 3)
        {
            for (int j = leftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                tile.BuildingTileRooms.Add(this);
                if ((i == 0 && j < leftUpAxisLength) || (j == 0 && i < rightUpAxisLength))
                {
                    RoomEdgeTilesPerCluster.Add(tile);
                    tile.IsAvailable = Availability.Unavailable;
                }
                else if (i == rightUpAxisLength || j == leftUpAxisLength)
                {
                    RoomEdgeTilesPerCluster.Add(tile);
                    if (tile.IsAvailable == Availability.Available)
                        tile.IsAvailable = Availability.UpperEdge;
                }
                else
                {
                    tile.IsAvailable = Availability.Unavailable;
                }
            }
        }

        return RoomEdgeTilesPerCluster;
    }

    // Make tiles available again for building
    public void CleanUpDeletedRoomTiles()
    {
        List<BuildingTile> roomLargerSquareTiles = BuilderManager.Instance.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= RoomCorners[Direction.Left].x - 15 &&
            tile.StartingPoint.x <= RoomCorners[Direction.Right].x + 15 &&
            tile.StartingPoint.y <= RoomCorners[Direction.Up].y + 10 &&
            tile.StartingPoint.y >= RoomCorners[Direction.Down].y - 10
            );

        List<BuildingTile> roomSquareTiles = roomLargerSquareTiles.FindAll(tile =>
            tile.StartingPoint.x >= RoomCorners[Direction.Left].x &&
            tile.StartingPoint.x <= RoomCorners[Direction.Right].x &&
            tile.StartingPoint.y <= RoomCorners[Direction.Up].y &&
            tile.StartingPoint.y >= RoomCorners[Direction.Down].y
        );

        int rightUpAxisLength = GetRightUpAxisLengthForRoomRotation();
        int leftUpAxisLength = GetLeftUpAxisLengthForRoomRotation();

        for (int i = 0; i <= rightUpAxisLength; i += 3)
        {
            List<BuildingTile> tilesThatIncludeDeletedRoom = new List<BuildingTile>(); // all tiles that the deleted room was built on
            for (int j = leftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                if (tile.BuildingTileRooms.Count == 1)
                {
                    if (tile.BuildingTileRooms[0].Id == Id) // check if this room is part of the found tile
                    {
                        tile.IsAvailable = Availability.Available;
                        tilesThatIncludeDeletedRoom.Add(tile);
                    }
                }
                else if (tile.BuildingTileRooms.Count > 1)
                {
                    for (int p = 0; p < tile.BuildingTileRooms.Count; p++)
                    {
                        if (tile.BuildingTileRooms[p].Id == Id)  // check if this room is part of the found tile
                        {
                            tilesThatIncludeDeletedRoom.Add(tile);

                            // complicated if statements to make sure the edges of remaining rooms get the correct availability status
                            if (tile.BuildingTileRooms.Count == 2 &&
                                (i == 0 || j == 0))
                            {
                                if (i == rightUpAxisLength)
                                {
                                    Vector2 location1 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i, -j - 3);
                                    BuildingTile tile1 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                                    Vector2 location2 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i, -j + 3);
                                    BuildingTile tile2 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                                    if (tile1.IsAvailable != Availability.Unavailable && tile2.IsAvailable != Availability.Unavailable)
                                        tile.IsAvailable = Availability.UpperEdge;
                                }
                                else if (j == leftUpAxisLength)
                                {
                                    Vector2 location1 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i - 3, -j);
                                    BuildingTile tile1 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                                    Vector2 location2 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i + 3, -j);
                                    BuildingTile tile2 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

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
                                    Vector2 location1 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i, -j + 3);
                                    BuildingTile tile1 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location1);
                                    Vector2 location2 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i, -j - 3);
                                    BuildingTile tile2 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location2);

                                    if (tile1 == null) Logger.Error("Error during tile clean up. Tried to find tile at location {0},{1}", location1.x, location1.y);
                                    if (tile2 == null) Logger.Error("Error during tile clean up. Tried to find tile at location {0},{1}", location2.x, location2.y);

                                    if (tile1.IsAvailable != Availability.Available && tile2.BuildingTileRooms.Count == 1)
                                        tile.IsAvailable = Availability.UpperEdge;
                                }
                                else if (j > 0)
                                {
                                    Vector2 location1 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i - 3, -j);
                                    BuildingTile tile1 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location1);
                                    Vector2 location2 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i + 3, -j);
                                    BuildingTile tile2 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location2);

                                    if (tile1 == null) Logger.Error("Error during tile clean up. Tried to find tile at location {0},{1}", location1.x, location1.y);
                                    if (tile2 == null) Logger.Error("Error during tile clean up. Tried to find tile at location {0},{1}", location2.x, location2.y);

                                    if (tile1.IsAvailable != Availability.Available && tile2.BuildingTileRooms.Count == 1)
                                        tile.IsAvailable = Availability.UpperEdge;

                                }
                                else
                                {
                                    tile.IsAvailable = Availability.UpperEdge;
                                }
                            }
                            else if (tile.BuildingTileRooms.Count == 4)
                            {
                                Vector2 location1 = GridHelper.GridToVectorLocation(RoomCorners[Direction.Down], i + 3, -j - 3);
                                BuildingTile tile1 = roomLargerSquareTiles.FirstOrDefault(t => t.StartingPoint == location1);
                                if (leftUpAxisLength == 3 &&
                                    rightUpAxisLength == 3 &&
                                    location == new Vector2(RoomCorners[Direction.Down].x, RoomCorners[Direction.Down].y)
                                    )
                                {
                                    tile.IsAvailable = Availability.UpperEdge;
                                }
                                else if (location == new Vector2(RoomCorners[Direction.Down].x, RoomCorners[Direction.Down].y)
                                    && tile1.BuildingTileRooms.Count == 1 && tile1.BuildingTileRooms[0].Id == Id)
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

    public void LowerWallPiece(Vector2 overlapPosition, Room thisRoom, Room otherRoom)
    {
        Logger.Warning("we found the common tile!!  {0},{1}. Shall we make it transparent? This room is {2}", overlapPosition.x, overlapPosition.y, thisRoom.Id);

        List<WallPiece> overlappingWallPiecesThisRoom = GetWallpiecesByAnyBuildingTile(overlapPosition, thisRoom);
        List<WallPiece> overlappingWallPiecesOtherRoom = GetWallpiecesByAnyBuildingTile(overlapPosition, otherRoom);

        for (int i = 0; i < overlappingWallPiecesThisRoom.Count; i++)
        {
            WallPiece overlappingWallPieceThisRoom = overlappingWallPiecesThisRoom[i];
            for (int j = 0; j < overlappingWallPiecesOtherRoom.Count; j++)
            {
                WallPiece overlappingWallPieceOtherRoom = overlappingWallPiecesOtherRoom[j];
                if (overlappingWallPieceOtherRoom && overlappingWallPieceThisRoom)
                {
                    if ((overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.DownRight || overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceThisRoom.WallPieceType == WallPieceType.UpLeft)
                    {
                        if (otherRoom.CharactersInRoom.Count < 1) continue;
                        overlappingWallPieceThisRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }

                    if ((overlappingWallPieceThisRoom.WallPieceType == WallPieceType.DownRight || overlappingWallPieceThisRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.UpLeft)
                    {
                        if (thisRoom.CharactersInRoom.Count < 1) continue;
                        overlappingWallPieceOtherRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }

                    if ((overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.DownLeft || overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceThisRoom.WallPieceType == WallPieceType.UpRight)
                    {
                        if (otherRoom.CharactersInRoom.Count < 1) continue;
                        overlappingWallPieceThisRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }

                    if ((overlappingWallPieceThisRoom.WallPieceType == WallPieceType.DownLeft || overlappingWallPieceThisRoom.WallPieceType == WallPieceType.CornerDown) &&
                        overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.UpRight)
                    {
                        if (thisRoom.CharactersInRoom.Count < 1) continue;
                        overlappingWallPieceOtherRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }
                }
            }
        }
    }

    public void LowerWallPieces()
    {
        //Logger.Log("lower wall pieces. CharactersInRoom? {0}", CharactersInRoom.Count);
        // Make wall pieces transparent in this room
        if (CharactersInRoom.Count > 0)
        {
            for (int i = 0; i < WallPieces.Count; i++)
            {
                if (WallPieces[i].WallPieceType == WallPieceType.DownLeft || WallPieces[i].WallPieceType == WallPieceType.DownRight || WallPieces[i].WallPieceType == WallPieceType.CornerDown)
                {
                    WallPieces[i].SetWallSprite(WallPieceDisplayMode.Transparent);
                }
            }
        }

        //get all wall tile clusters with overlap. Then compare the direction of the wall pieces with those of other rooms and lower correct wall pieces
        List<BuildingTile> edgeTileClustersWithOverlap = RoomEdgeTilesPerCluster.Where(tile => tile.BuildingTileRooms.Count > 1).ToList();

        for (int j = 0; j < edgeTileClustersWithOverlap.Count; j++)
        {
            BuildingTile tileWithOverlap = edgeTileClustersWithOverlap[j];
            for (int k = 0; k < tileWithOverlap.BuildingTileRooms.Count; k++)
            {
                Room otherRoom = tileWithOverlap.BuildingTileRooms[k];
                if (otherRoom == this) continue;

                for (int m = 0; m < otherRoom.RoomEdgeTilesPerCluster.Count; m++)
                {
                    BuildingTile otherRoomTile = otherRoom.RoomEdgeTilesPerCluster[m];
                    // the tile in this room and the same tile in the other room. Now find the common wall pieces
                    if (otherRoomTile.StartingPoint.x == tileWithOverlap.StartingPoint.x)
                    {
                        //Logger.Warning("we found the common tile!!  {0},{1}. Shall we make it transparent?", tileWithOverlap.StartingPoint.x, tileWithOverlap.StartingPoint.y);
                        LowerWallPiece(tileWithOverlap.StartingPoint, this, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 5, tileWithOverlap.StartingPoint.y + 2.5f), this, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 10, tileWithOverlap.StartingPoint.y + 5f), this, otherRoom);
                        //LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 15, tileWithOverlap.StartingPoint.y + 7.5f), this, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 5, tileWithOverlap.StartingPoint.y - 2.5f), this, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 10, tileWithOverlap.StartingPoint.y - 5f), this, otherRoom);
                    }
                }
            }
        }
    }

    public void RaiseWallPiece(Vector2 position, Room room)
    {
        List<WallPiece> wallPieces = room.WallPieces.Where(wallPiece => wallPiece.transform.position.x == position.x && wallPiece.transform.position.y == position.y).ToList();
        if(wallPieces.Count > 0)
        {
            for (int i = 0; i < wallPieces.Count; i++)
            {
                if (wallPieces[i].gameObject.activeSelf)
                {
                    wallPieces[i].SetWallSprite(WallPieceDisplayMode.Visible);
                    Logger.Log("Raise {0} at {1},{2}", wallPieces[i].gameObject.name, wallPieces[i].transform.position.x, wallPieces[i].transform.position.y);
                }

            }
        }
    }

    // this room is the room that is left
    public void RaiseWallPieces()
    {
        List<BuildingTile> edgeTileClustersWithOverlap = RoomEdgeTilesPerCluster.Where(tile => tile.BuildingTileRooms.Count > 1).ToList();
        for (int i = 0; i < WallPieces.Count; i++)
        {
            WallPiece wallPiece = WallPieces[i];
            if (wallPiece.WallPieceType == WallPieceType.DownLeft || wallPiece.WallPieceType == WallPieceType.DownRight || wallPiece.WallPieceType == WallPieceType.CornerDown)
            {
                if(wallPiece.gameObject.activeSelf)
                    wallPiece.SetWallSprite(WallPieceDisplayMode.Visible);
                else
                {
                    //find the corresponding wall piece in the other room and activate that
                    //Logger.Log("We need to find the corresponding wallpiece to wallpiece at {0},{1}", wallPiece.transform.position.x, wallPiece.transform.position.y);
                    List<WallPiece> overlappingOtherRoomWallPieces = new List<WallPiece>();
                    for (int j = 0; j < AdjacentRooms.Count; j++)
                    {
                        overlappingOtherRoomWallPieces.AddRange(AdjacentRooms[j].WallPieces.Where(otherWallPiece => otherWallPiece.transform.position == wallPiece.transform.position).ToList());
                    }
                    for (int k = 0; k < overlappingOtherRoomWallPieces.Count; k++)
                    {
                        //Logger.Log("try piece with name {0} at {1},{2}", overlappingOtherRoomWallPieces[k].gameObject.name, overlappingOtherRoomWallPieces[k].transform.position.x, overlappingOtherRoomWallPieces[k].transform.position.y);

                        if (overlappingOtherRoomWallPieces[k].gameObject.activeSelf)
                        {
                            overlappingOtherRoomWallPieces[k].SetWallSprite(WallPieceDisplayMode.Visible);
                        }
                    }
                }
            }

            List<BuildingTile> overlappingTileClusters = edgeTileClustersWithOverlap.Where(edgeTile => (edgeTile.StartingPoint == new Vector2(wallPiece.transform.position.x, wallPiece.transform.position.y))).ToList();

            if (overlappingTileClusters.Count == 0) continue;

            BuildingTile overlappingTile = overlappingTileClusters[0]; // take any tile, because the starting positions should be the same
            Logger.Warning("go over tile with overlap  {0},{1}", overlappingTile.StartingPoint.x, overlappingTile.StartingPoint.y);
            List<WallPiece> overlappingWallPieces = new List<WallPiece>();
            for (int k = 0; k < overlappingTile.BuildingTileRooms.Count; k++)
            {
                Room otherRoom = overlappingTile.BuildingTileRooms[k];
                if (otherRoom == this) continue;
                overlappingWallPieces.AddRange(GetWallpiecesByAnyBuildingTile(overlappingTile.StartingPoint, otherRoom));
            }

            Logger.Log("overlappingWallpieces Count: {0}", overlappingWallPieces.Count);

            for (int j = 0; j < overlappingWallPieces.Count; j++)
            {
                Logger.Log("overlappingWallpieces {0} type {1} against wallPiece type {2} ", j, overlappingWallPieces[j].WallPieceType, wallPiece.WallPieceType);

                if (overlappingWallPieces[j] == wallPiece) continue;

                // it is a corner, intersecting downleft in a corner to the right
                if (wallPiece.WallPieceType == WallPieceType.DownLeft)
                {
                    //Logger.Warning("we found the common tile!!  {0},{1}. Make it visible DownLeft", wallPiece.transform.position.x, wallPiece.transform.position.y);
                    if ((overlappingWallPieces[j].WallPieceType == WallPieceType.DownRight || overlappingWallPieces[j].WallPieceType == WallPieceType.CornerDown)
                        && overlappingWallPieces[j].Room.CharactersInRoom.Count == 0
                        )
                    {
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y + 2.5f), this);
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y + 5f), this);
                    }
                }
                if (wallPiece.WallPieceType == WallPieceType.UpLeft)
                {
                    Logger.Log("wallPiece.WallPieceType == WallPieceType.UpLeft.. at {0},{1}", wallPiece.transform.position.x, wallPiece.transform.position.y);
                    Logger.Log("overlappingWallpieces[j].WallPieceType? {0}. Name {1}", overlappingWallPieces[j].WallPieceType, overlappingWallPieces[j].transform.gameObject.name);
                    if ((overlappingWallPieces[j].WallPieceType == WallPieceType.DownRight || overlappingWallPieces[j].WallPieceType == WallPieceType.CornerDown)
                        && overlappingWallPieces[j].Room.CharactersInRoom.Count == 0
                        )
                    {
                        wallPiece.SetWallSprite(WallPieceDisplayMode.Visible);
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y + 2.5f), this);
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y + 5f), this);
                        //if there is no cornering piece of type UpRight at same location
                        if(!WallPieces.Any(piece => piece.transform.position == wallPiece.transform.position && piece.WallPieceType == WallPieceType.UpRight))
                        {
                            RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y - 2.5f), this);
                            RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y - 5f), this);
                        }
                    }
                }
                if (wallPiece.WallPieceType == WallPieceType.DownRight)
                {
                    Logger.Warning("we found the common tile!!  {0},{1}. Make it visible DownRight", wallPiece.transform.position.x, wallPiece.transform.position.y);
                    if ((overlappingWallPieces[j].WallPieceType == WallPieceType.DownLeft || overlappingWallPieces[j].WallPieceType == WallPieceType.CornerDown)
                        && overlappingWallPieces[j].Room.CharactersInRoom.Count == 0
                        )
                    {
                        //RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y + 2.5f), this);
                        //RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y + 5f), this);
                    }
                }
                if (wallPiece.WallPieceType == WallPieceType.UpRight)
                {
                    Logger.Warning("we found the common tile!!  {0},{1}. Make it visible UpRight!", wallPiece.transform.position.x, wallPiece.transform.position.y);
                    if ((overlappingWallPieces[j].WallPieceType == WallPieceType.DownLeft || overlappingWallPieces[j].WallPieceType == WallPieceType.CornerDown)
                        && overlappingWallPieces[j].Room.CharactersInRoom.Count == 0
                        )
                    {
                        //RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y + 2.5f), this);
                        //RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y + 5f), this);
                    }
                }
            }
        }
    }

    public int GetRightUpAxisLengthForRoomRotation() {
        return RoomRotation == ObjectRotation.Rotation0 || RoomRotation == ObjectRotation.Rotation180 ?
        RoomBlueprint.RightUpAxisLength : RoomBlueprint.LeftUpAxisLength;
    }
    public int GetLeftUpAxisLengthForRoomRotation()
    {
        return RoomRotation == ObjectRotation.Rotation0 || RoomRotation == ObjectRotation.Rotation180 ?
        RoomBlueprint.LeftUpAxisLength : RoomBlueprint.RightUpAxisLength;
    }
}