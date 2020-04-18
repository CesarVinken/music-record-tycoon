using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallPieceTransparencyHandler
{
    public static void ReenableWallpiecesFromAdjacentRooms(Room room)
    {
        // after a neighbour room is deleted, go over all wallpieces in this room to check if any wallpieces now do not overlap but are disabled. Reenable these
        List<BuildingTile> edgeTileClustersWithOverlap = room.RoomEdgeTilesPerCluster.Where(tile => tile.BuildingTileRooms.Count > 1).ToList();

        List<Room> roomsStartingOnEdgeTile = new List<Room>();
        for (int j = 0; j < edgeTileClustersWithOverlap.Count; j++)
        {
            BuildingTile edgeTileClusterWithOverlap = edgeTileClustersWithOverlap[j];
            for (int p = 0; p < edgeTileClusterWithOverlap.BuildingTileRooms.Count; p++)
            {
                Room overlappingRoom = edgeTileClusterWithOverlap.BuildingTileRooms[p];
                if (overlappingRoom.RoomCorners[Direction.Down].x == edgeTileClusterWithOverlap.StartingPoint.x && overlappingRoom.RoomCorners[Direction.Down].y == edgeTileClusterWithOverlap.StartingPoint.y)
                {
                    roomsStartingOnEdgeTile.Add(overlappingRoom);
                }
            }
        }

        // rooms that have the down corner on along the edge of this room
        for (int i = 0; i < edgeTileClustersWithOverlap.Count; i++)
        {
            BuildingTile tileWithOverlap = edgeTileClustersWithOverlap[i];

            List<WallPiece> wallPiecesAllRooms = new List<WallPiece>();
            for (int k = 0; k < tileWithOverlap.BuildingTileRooms.Count; k++)
            {
                Room otherRoom = tileWithOverlap.BuildingTileRooms[k];
                wallPiecesAllRooms.AddRange(Room.GetWallpiecesByAnyBuildingTile(tileWithOverlap.StartingPoint, otherRoom));
            }
            List<WallPiece> activeWallPiecesFromThisRoom = wallPiecesAllRooms.Where(wallPiece => wallPiece.Room.Id == room.Id).ToList();
            if (activeWallPiecesFromThisRoom.Count == 0) continue;

            for (int j = 0; j < activeWallPiecesFromThisRoom.Count; j++)
            {
                WallPiece activeWallPiece = activeWallPiecesFromThisRoom[j];

                if (activeWallPiece.WallPieceType == WallPieceType.UpLeft || activeWallPiece.WallPieceType == WallPieceType.DownLeft) // UpLeft for continuing wall parts, DownLeft at the bottom corner
                {
                    if (activeWallPiece.transform.position.x == 0)
                    {
                        Logger.Log("For this tile at {0},{1} the type is {2}", activeWallPiece.transform.position.x, activeWallPiece.transform.position.y, activeWallPiece.WallPieceType);
                        Logger.Log("This room downside is {0},{1}, with the id {2}", room.RoomCorners[Direction.Down].x, room.RoomCorners[Direction.Down].y, room.Id);
                    }

                    List<WallPiece> oppositeWallPieces = wallPiecesAllRooms.Where(wallPiece => wallPiece.WallPieceType == WallPieceType.DownRight).ToList();

                    for (int k = 0; k < oppositeWallPieces.Count; k++)
                    {
                        WallPiece oppositeWallPiece = oppositeWallPieces[k];
                        if (activeWallPiece.transform.position.x == 0)
                        {
                            Logger.Log("oppositeWallPiece at {0},{1} the type is {2}", oppositeWallPiece.transform.position.x, oppositeWallPiece.transform.position.y, oppositeWallPiece.WallPieceType);
                        }
                        // only if not corner piece
                        if (!wallPiecesAllRooms.Any(wallPiece => wallPiece.WallPieceType == WallPieceType.UpRight && wallPiece.Room.Id != room.Id))
                        {
                            oppositeWallPiece.gameObject.SetActive(true);
                            if (oppositeWallPiece.Room.CharactersInRoom.Count > 0)
                                oppositeWallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);
                        }

                        oppositeWallPiece.Room.WallPieces.Where(
                            wallPiece => wallPiece.WallPieceType == WallPieceType.DownRight
                            && wallPiece.transform.position.x == tileWithOverlap.StartingPoint.x + 5
                            && wallPiece.transform.position.y == tileWithOverlap.StartingPoint.y + 2.5f
                            && room.WallPieces.Any(wallPieceThisRoom =>
                                wallPieceThisRoom.transform.position.x == tileWithOverlap.StartingPoint.x + 5
                                && wallPieceThisRoom.transform.position.y == tileWithOverlap.StartingPoint.y + 2.5f))
                            .ToList()
                            .ForEach(wallPiece => {
                                wallPiece.gameObject.SetActive(true);
                                if (wallPiece.Room.CharactersInRoom.Count > 0)
                                    wallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);
                            });
                        oppositeWallPiece.Room.WallPieces.Where(
                        wallPiece => wallPiece.WallPieceType == WallPieceType.DownRight
                        && wallPiece.transform.position.x == tileWithOverlap.StartingPoint.x + 10
                        && wallPiece.transform.position.y == tileWithOverlap.StartingPoint.y + 5f
                        && room.WallPieces.Any(wallPieceThisRoom =>
                            wallPieceThisRoom.transform.position.x == tileWithOverlap.StartingPoint.x + 10
                            && wallPieceThisRoom.transform.position.y == tileWithOverlap.StartingPoint.y + 5f))
                        .ToList()
                        .ForEach(wallPiece => {
                            wallPiece.gameObject.SetActive(true);
                            if (wallPiece.Room.CharactersInRoom.Count > 0)
                                wallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);
                        });
                    }

                    //for dealing with corners
                    for (int l = 0; l < roomsStartingOnEdgeTile.Count; l++)
                    {
                        Room roomStartingOnEdgeTile = roomsStartingOnEdgeTile[l];
                        Vector2 roomStartingOnEdgeTileLocation = roomStartingOnEdgeTile.RoomCorners[Direction.Down];
                        // check if there is a wallpiece intersecting the corner
                        if (activeWallPiece.transform.position.x == roomStartingOnEdgeTileLocation.x
                        && activeWallPiece.transform.position.y == roomStartingOnEdgeTileLocation.y)
                        {
                            for (int n = 0; n < roomStartingOnEdgeTile.WallPieces.Count; n++)
                            {
                                WallPiece wallPiece = roomStartingOnEdgeTile.WallPieces[n];
                                if((wallPiece.transform.position.x == roomStartingOnEdgeTileLocation.x + 5
                                    && wallPiece.transform.position.y == roomStartingOnEdgeTileLocation.y + 2.5f) ||
                                    (wallPiece.transform.position.x == roomStartingOnEdgeTileLocation.x + 10
                                    && wallPiece.transform.position.y == roomStartingOnEdgeTileLocation.y + 5f))
                                {
                                    wallPiece.gameObject.SetActive(true);
                                    if (wallPiece.Room.CharactersInRoom.Count > 0)
                                        wallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);
                                }

                            }
                            Logger.Log("There is something on the corner");
                        }
                    }
                }
                if (activeWallPiece.WallPieceType == WallPieceType.UpRight)
                {
                    List<WallPiece> oppositeWallPieces = wallPiecesAllRooms.Where(wallPiece => wallPiece.WallPieceType == WallPieceType.DownLeft).ToList();

                    for (int k = 0; k < oppositeWallPieces.Count; k++)
                    {
                        WallPiece oppositeWallPiece = oppositeWallPieces[k];

                        oppositeWallPiece.gameObject.SetActive(true);

                        if (oppositeWallPiece.Room.CharactersInRoom.Count > 0)
                            oppositeWallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);

                        oppositeWallPiece.Room.WallPieces.Where(
                            wallPiece => wallPiece.WallPieceType == WallPieceType.DownLeft
                            && wallPiece.transform.position.x == tileWithOverlap.StartingPoint.x + 5
                            && wallPiece.transform.position.y == tileWithOverlap.StartingPoint.y - 2.5f
                            )
                            .ToList()
                            .ForEach(wallPiece => {
                                wallPiece.gameObject.SetActive(true);
                                if (wallPiece.Room.CharactersInRoom.Count > 0)
                                    wallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);
                            });
                        oppositeWallPiece.Room.WallPieces.Where(
                            wallPiece => wallPiece.WallPieceType == WallPieceType.DownLeft
                            && wallPiece.transform.position.x == tileWithOverlap.StartingPoint.x + 10
                            && wallPiece.transform.position.y == tileWithOverlap.StartingPoint.y - 5f
                            )
                            .ToList()
                            .ForEach(wallPiece => {
                                wallPiece.gameObject.SetActive(true);
                                if (wallPiece.Room.CharactersInRoom.Count > 0)
                                    wallPiece.SetWallSprite(WallPieceDisplayMode.Transparent);
                            });
                    }
                }
            }
        }
    }

    public static void LowerWallPiece(Vector2 overlapPosition, Room thisRoom, Room otherRoom)
    {
        List<WallPiece> overlappingWallPiecesThisRoom = Room.GetWallpiecesByAnyBuildingTile(overlapPosition, thisRoom);
        List<WallPiece> overlappingWallPiecesOtherRoom = Room.GetWallpiecesByAnyBuildingTile(overlapPosition, otherRoom);

        for (int i = 0; i < overlappingWallPiecesThisRoom.Count; i++)
        {

            WallPiece overlappingWallPieceThisRoom = overlappingWallPiecesThisRoom[i];
            for (int j = 0; j < overlappingWallPiecesOtherRoom.Count; j++)
            {
                WallPiece overlappingWallPieceOtherRoom = overlappingWallPiecesOtherRoom[j];

                if (overlappingWallPieceOtherRoom && overlappingWallPieceThisRoom)
                {
                    if ((overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.DownRight) &&
                        overlappingWallPieceThisRoom.WallPieceType == WallPieceType.UpLeft)
                    {
                        if (otherRoom.CharactersInRoom.Count < 1) continue;
                        if (!overlappingWallPieceThisRoom.gameObject.activeSelf) continue;
                        overlappingWallPieceThisRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }

                    if ((overlappingWallPieceThisRoom.WallPieceType == WallPieceType.DownRight) &&
                        overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.UpLeft)
                    {
                        if (thisRoom.CharactersInRoom.Count < 1) continue;
                        if (!overlappingWallPieceOtherRoom.gameObject.activeSelf) continue;
                        overlappingWallPieceOtherRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }

                    if ((overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.DownLeft) &&
                        overlappingWallPieceThisRoom.WallPieceType == WallPieceType.UpRight)
                    {
                        if (otherRoom.CharactersInRoom.Count < 1) continue;
                        if (!overlappingWallPieceThisRoom.gameObject.activeSelf) continue;
                        overlappingWallPieceThisRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }

                    if ((overlappingWallPieceThisRoom.WallPieceType == WallPieceType.DownLeft) &&
                        overlappingWallPieceOtherRoom.WallPieceType == WallPieceType.UpRight)
                    {
                        if (thisRoom.CharactersInRoom.Count < 1) continue;
                        if (!overlappingWallPieceOtherRoom.gameObject.activeSelf) continue;
                        overlappingWallPieceOtherRoom.SetWallSprite(WallPieceDisplayMode.Transparent);
                    }
                }
            }
        }
    }

    public static void LowerWallPieces(Room room)
    {
        // Make wall pieces transparent in this room
        if (room.CharactersInRoom.Count > 0)
        {
            for (int i = 0; i < room.WallPieces.Count; i++)
            {
                if (room.WallPieces[i].WallPieceType == WallPieceType.DownLeft || room.WallPieces[i].WallPieceType == WallPieceType.DownRight)
                {
                    if (!room.WallPieces[i].gameObject.activeSelf) continue;
                    room.WallPieces[i].SetWallSprite(WallPieceDisplayMode.Transparent);
                }
            }
        }

        //get all wall tile clusters with overlap. Then compare the direction of the wall pieces with those of other rooms and lower correct wall pieces
        List<BuildingTile> edgeTileClustersWithOverlap = room.RoomEdgeTilesPerCluster.Where(tile => tile.BuildingTileRooms.Count > 1).ToList();

        for (int j = 0; j < edgeTileClustersWithOverlap.Count; j++)
        {
            BuildingTile tileWithOverlap = edgeTileClustersWithOverlap[j];
            for (int k = 0; k < tileWithOverlap.BuildingTileRooms.Count; k++)
            {
                Room otherRoom = tileWithOverlap.BuildingTileRooms[k];
                if (otherRoom == room) continue;

                for (int m = 0; m < otherRoom.RoomEdgeTilesPerCluster.Count; m++)
                {
                    BuildingTile otherRoomTile = otherRoom.RoomEdgeTilesPerCluster[m];
                    // the tile in this room and the same tile in the other room. Now find the common wall pieces
                    if (otherRoomTile.StartingPoint.x == tileWithOverlap.StartingPoint.x)
                    {
                        LowerWallPiece(tileWithOverlap.StartingPoint, room, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 5, tileWithOverlap.StartingPoint.y + 2.5f), room, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 10, tileWithOverlap.StartingPoint.y + 5f), room, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 5, tileWithOverlap.StartingPoint.y - 2.5f), room, otherRoom);
                        LowerWallPiece(new Vector2(tileWithOverlap.StartingPoint.x + 10, tileWithOverlap.StartingPoint.y - 5f), room, otherRoom);
                    }
                }
            }
        }
    }

    // this room is the room that is left
    public static void RaiseWallPieces(Room room)
    {
        List<BuildingTile> edgeTileClustersWithOverlap = room.RoomEdgeTilesPerCluster.Where(tile => tile.BuildingTileRooms.Count > 1).ToList();
        for (int i = 0; i < room.WallPieces.Count; i++)
        {
            WallPiece wallPiece = room.WallPieces[i];
            if (wallPiece.WallPieceType == WallPieceType.DownLeft || wallPiece.WallPieceType == WallPieceType.DownRight)
            {

                if (wallPiece.gameObject.activeSelf)
                    wallPiece.SetWallSprite(WallPieceDisplayMode.Visible);
                else
                {
                    //find the corresponding wall piece in the other room and activate that
                    List<WallPiece> overlappingOtherRoomWallPieces = new List<WallPiece>();
                    for (int j = 0; j < room.AdjacentRooms.Count; j++)
                    {
                        overlappingOtherRoomWallPieces.AddRange(room.AdjacentRooms[j].WallPieces.Where(otherWallPiece => otherWallPiece.transform.position == wallPiece.transform.position).ToList());
                    }
                    for (int k = 0; k < overlappingOtherRoomWallPieces.Count; k++)
                    {
                        WallPiece overlappingOtherRoomWallPiece = overlappingOtherRoomWallPieces[k];
                        if (wallPiece.transform.position.x == 30 && wallPiece.transform.position.y == 0)
                        {
                            Logger.Log("RAISE wall pieces for {0}, {1}", wallPiece.transform.position.x, wallPiece.transform.position.y);
                            Logger.Log("wallPiece type {0}", wallPiece.WallPieceType);
                            Logger.Log("overlappingOtherRoomWallPiece type {0}", overlappingOtherRoomWallPiece.WallPieceType);
                        }
                        if (overlappingOtherRoomWallPiece.gameObject.activeSelf)
                        {
                            if (wallPiece.WallPieceType == WallPieceType.DownLeft)
                            {
                                if (overlappingOtherRoomWallPiece.WallPieceType == WallPieceType.UpLeft)
                                {
                                    if (overlappingOtherRoomWallPiece.Room.CharactersInRoom.Count > 0) continue;
                                    WallPiece downRightWallPiece = overlappingOtherRoomWallPieces.FirstOrDefault(overlappingWallPiece => overlappingWallPiece.WallPieceType == WallPieceType.DownRight);
                                    if (downRightWallPiece && downRightWallPiece.Room.CharactersInRoom.Count > 0) continue;
                                }
                            }
                            if (wallPiece.WallPieceType == WallPieceType.DownRight)
                            {
                                if (overlappingOtherRoomWallPiece.WallPieceType == WallPieceType.UpRight)
                                {
                                    if (overlappingOtherRoomWallPiece.Room.CharactersInRoom.Count > 0) continue;
                                    WallPiece downLeftWallPiece = overlappingOtherRoomWallPieces.FirstOrDefault(overlappingWallPiece => overlappingWallPiece.WallPieceType == WallPieceType.DownLeft);
                                    if (downLeftWallPiece && downLeftWallPiece.Room.CharactersInRoom.Count > 0) continue;
                                }
                            }
                            overlappingOtherRoomWallPiece.SetWallSprite(WallPieceDisplayMode.Visible);
                        }
                    }
                }
            }

            List<BuildingTile> overlappingTileClusters = edgeTileClustersWithOverlap.Where(edgeTile => (edgeTile.StartingPoint == new Vector2(wallPiece.transform.position.x, wallPiece.transform.position.y))).ToList();

            if (overlappingTileClusters.Count == 0) continue;

            BuildingTile overlappingTile = overlappingTileClusters[0]; // take any tile, because the starting positions should be the same

            List<WallPiece> overlappingWallPieces = new List<WallPiece>();
            for (int k = 0; k < overlappingTile.BuildingTileRooms.Count; k++)
            {
                Room otherRoom = overlappingTile.BuildingTileRooms[k];
                if (otherRoom == room) continue;
                overlappingWallPieces.AddRange(Room.GetWallpiecesByAnyBuildingTile(overlappingTile.StartingPoint, otherRoom));
            }

            for (int j = 0; j < overlappingWallPieces.Count; j++)
            {
                WallPiece overlappingWallPiece = overlappingWallPieces[j];
                if (overlappingWallPiece == wallPiece) continue;

                // it is a corner, intersecting downleft in a corner to the right
                if (wallPiece.WallPieceType == WallPieceType.DownLeft)
                {
                    //Logger.Warning("we found the common tile!!  {0},{1}. Make it visible DownLeft", wallPiece.transform.position.x, wallPiece.transform.position.y);
                    if ((overlappingWallPiece.WallPieceType == WallPieceType.DownRight)
                        && overlappingWallPiece.Room.CharactersInRoom.Count == 0
                        )
                    {
                        //get wall pieces from 1 tile up right. Check if there is a DownRight wallpiece which more than 1 person in the room. If so, continuel
                        Room corneringRoom = RoomManager.Rooms.FirstOrDefault(r => r.RoomCorners[Direction.Down].x == wallPiece.transform.position.x && r.RoomCorners[Direction.Down].y == wallPiece.transform.position.y);
                        if (corneringRoom && corneringRoom.CharactersInRoom.Count > 0) continue;                           

                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y + 2.5f), room);
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y + 5f), room);
                    }
                }
                if (wallPiece.WallPieceType == WallPieceType.UpLeft)
                {
                    if ((overlappingWallPiece.WallPieceType == WallPieceType.DownRight)
                        && overlappingWallPiece.Room.CharactersInRoom.Count == 0
                        )
                    {
                        WallPiece downRightWallPiece = overlappingWallPieces.FirstOrDefault(
                            w => w.WallPieceType == WallPieceType.DownRight
                            && w.Room.Id != overlappingWallPiece.Room.Id);
                        if (downRightWallPiece && downRightWallPiece.Room.CharactersInRoom.Count > 0) continue;

                        Room corneringRoom = RoomManager.Rooms.FirstOrDefault(r => r.RoomCorners[Direction.Down].x == wallPiece.transform.position.x && r.RoomCorners[Direction.Down].y == wallPiece.transform.position.y);
                        if (corneringRoom && corneringRoom.CharactersInRoom.Count > 0) continue;

                        wallPiece.SetWallSprite(WallPieceDisplayMode.Visible);
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y + 2.5f), room);
                        RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y + 5f), room);

                        //if there is no cornering piece of type UpRight at same location
                        if (!room.WallPieces.Any(piece => piece.transform.position == wallPiece.transform.position && piece.WallPieceType == WallPieceType.UpRight))
                        {
                            RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 5, wallPiece.transform.position.y - 2.5f), room);
                            RaiseWallPiece(new Vector2(wallPiece.transform.position.x + 10, wallPiece.transform.position.y - 5f), room);
                        }
                    }
                }
            }
        }
    }

    public static void RaiseWallPiece(Vector2 position, Room room)
    {
        List<WallPiece> wallPieces = room.WallPieces.Where(wallPiece => wallPiece.transform.position.x == position.x && wallPiece.transform.position.y == position.y).ToList();
        if (wallPieces.Count > 0)
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

}
