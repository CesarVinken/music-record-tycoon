
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingPlotBuilder
{
    private BuilderManager _builderManager;

    public BuildingPlotBuilder()
    {
        _builderManager = BuilderManager.Instance;
    }

    public bool GetPlotIsAvailable(RoomBlueprint roomBlueprint, Vector2 existingRoomStartingPoint, int rightUpAxisLocationFromCurrentRoom, int leftUpAxisLocationFromCurrentRoom)
    {
        bool isAvailable = true;
        Vector2 plotLocationStartingPoint = GridHelper.CalculateLocationOnGrid(existingRoomStartingPoint, rightUpAxisLocationFromCurrentRoom, leftUpAxisLocationFromCurrentRoom);
        Vector2 point1 = GridHelper.CalculateLocationOnGrid(plotLocationStartingPoint, roomBlueprint.RightUpAxisLength, 0);
        Vector2 point2 = GridHelper.CalculateLocationOnGrid(point1, 0, -roomBlueprint.LeftUpAxisLength);
        Vector2 point3 = GridHelper.CalculateLocationOnGrid(point2, -roomBlueprint.RightUpAxisLength, 0);

        List<BuildingTile> roomSquareTiles = _builderManager.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= point3.x &&
            tile.StartingPoint.x <= point1.x &&
            tile.StartingPoint.y <= point2.y &&
            tile.StartingPoint.y >= plotLocationStartingPoint.y
        );

        for (int i = 3; i <= roomBlueprint.RightUpAxisLength; i += 3)
        {
            for (int j = roomBlueprint.LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = GridHelper.CalculateLocationOnGrid(plotLocationStartingPoint, i, -j);

                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                if (i == 0 || i == roomBlueprint.RightUpAxisLength || j == 0 || j == roomBlueprint.LeftUpAxisLength)
                {
                    // skip tiles that are at the edge because they may overlap with the walls of adjacent rooms
                    continue;
                }

                if (tile == null)
                {
                    Logger.Error(Logger.Building, "Could not find tile at {0}", location);
                }

                if (tile.IsAvailable != Availability.Available)
                {
                    isAvailable = false;
                    break;
                }
            }
        }

        return isAvailable;
    }

    // check all tiles where the plot would be drawn if the building tile is available. Only draw a plot when all tiles are available
    public bool GetPlotIsAvailable(RoomBlueprint roomBlueprint, Vector2 roomStartingPoint, RoomRotation roomRotation)
    {
        int rightUpAxisLength = roomRotation == RoomRotation.Rotation0 || roomRotation == RoomRotation.Rotation180 ?
            roomBlueprint.RightUpAxisLength : roomBlueprint.LeftUpAxisLength;
        int leftUpAxisLength = roomRotation == RoomRotation.Rotation0 || roomRotation == RoomRotation.Rotation180 ?
    roomBlueprint.LeftUpAxisLength : roomBlueprint.RightUpAxisLength;
        Vector2 plotLocationStartingPoint = GridHelper.CalculateLocationOnGrid(roomStartingPoint, 0, 0);
        Vector2 point1 = GridHelper.CalculateLocationOnGrid(plotLocationStartingPoint, rightUpAxisLength, 0);
        Vector2 point2 = GridHelper.CalculateLocationOnGrid(point1, 0, -leftUpAxisLength);
        Vector2 point3 = GridHelper.CalculateLocationOnGrid(point2, -rightUpAxisLength, 0);

        List<BuildingTile> roomSquareTiles = _builderManager.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= point3.x &&
            tile.StartingPoint.x <= point1.x &&
            tile.StartingPoint.y <= point2.y &&
            tile.StartingPoint.y >= plotLocationStartingPoint.y
        );

        BuildingTile startingTile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == plotLocationStartingPoint);
        if (startingTile.IsAvailable == Availability.Unavailable) return false;

        bool isAvailable = true;

        for (int i = 3; i <= rightUpAxisLength; i += 3)
        {
            for (int j = leftUpAxisLength; j >= 0; j -= 3)
            {
                BuildingTile tile = GetBuildingTileForAvailability(i, j, plotLocationStartingPoint, roomSquareTiles);

                if (i == 0 || i == rightUpAxisLength || j == 0 || j == leftUpAxisLength)
                {
                    // skip tiles that are at the edge because they may overlap with the walls of adjacent rooms
                    continue;
                }

                if (tile == null)
                {
                    Logger.Error(Logger.Building, "Could not find tile at {0}", GridHelper.CalculateLocationOnGrid(plotLocationStartingPoint, i, -j));
                }

                if (tile.IsAvailable != Availability.Available)
                {
                    isAvailable = false;
                    break;
                }
            }
        }

        return isAvailable;
    }

    public void DrawAvailablePlots()
    {
        if (BuilderManager.InDeleteObjectMode)
            _builderManager.DeactivateDeleteRoomMode();

        RoomBlueprint selectedRoomBlueprint = _builderManager.SelectedRoom;
        BuildMenuWorldSpaceContainer.Instance.DestroyBuildingPlots();

        for (int i = 0; i < RoomManager.Rooms.Count; i++)
        {
            Room existingRoom = RoomManager.Rooms[i];
            Logger.Log("*********************");
            Logger.Log("");
            Logger.Log("Start looking at next room at {0}, {1}", existingRoom.RoomCorners[Direction.Down].x, existingRoom.RoomCorners[Direction.Down].y);
            Logger.Log("");
            Logger.Log("*********************");
            // for each door, check if it aligns with the doors of the selected room
            for (int j = 0; j < existingRoom.Doors.Count; j++)
            {
                Door existingDoor = existingRoom.Doors[j];
                if (existingDoor.DoorConnection != null) continue; // if the door is already activated, no need to check for new connections

                bool createdPlotForDoor = false;

                // the doors that are already on the map
                GridLocation doorPositionOnExistingRoom = GridHelper.CalculateGridLocationFromVector2(existingDoor.transform.position);

                for (int k = 0; k < selectedRoomBlueprint.DoorLocations.Length; k++)
                {
                    // for each door location, check if a building plot is available starting at the roomDoorPosition - bluePrintDoorPosition
                    GridLocation doorPositionOnBlueprint = selectedRoomBlueprint.DoorLocations[k];

                    if ((doorPositionOnExistingRoom.UpRight == doorPositionOnBlueprint.UpRight) && (doorPositionOnExistingRoom.UpLeft == doorPositionOnBlueprint.UpLeft)) continue;
                    Logger.Log("doorPositionOnExistingRoom {0}, {1} - doorPositionOnBlueprint 0 {2}, {3}", doorPositionOnExistingRoom.UpRight, doorPositionOnExistingRoom.UpLeft, doorPositionOnBlueprint.UpRight, doorPositionOnBlueprint.UpLeft);

                    GridLocation blueprintRoomStartPosition = new GridLocation(doorPositionOnExistingRoom.UpRight - doorPositionOnBlueprint.UpRight, (doorPositionOnExistingRoom.UpLeft - doorPositionOnBlueprint.UpLeft));

                    if (blueprintRoomStartPosition.UpLeft % 3 != 0 || blueprintRoomStartPosition.UpRight % 3 != 0)
                    {
                        Logger.Log(Logger.Building, "This room cannot be built because it would not start on a (large) tile location. Its start position would be {0}, {1}. Skip. ", blueprintRoomStartPosition.UpRight, blueprintRoomStartPosition.UpLeft);
                        continue;
                    }

                    RoomRotation roomRotation = RoomRotation.Rotation0;
                    Vector2 blueprintRoomStartPositionVector = GridHelper.CalculateLocationOnGrid((int)blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
                    if (GetPlotIsAvailable(selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation))
                    {
                        BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation);
                        createdPlotForDoor = true;
                    }
                }

                // 90 degrees turned
                if (!createdPlotForDoor)
                {
                    Logger.Log("Try in second rotation, swap x axis 90 degrees");
                    for (int k = 0; k < selectedRoomBlueprint.DoorLocations.Length; k++)
                    {
                        // for each door location, check if a building plot is available starting at the roomDoorPosition - bluePrintDoorPosition
                        GridLocation rawDoorPositionOnBlueprint = selectedRoomBlueprint.DoorLocations[k];
                        GridLocation doorPositionOnBlueprint = new GridLocation(
                            rawDoorPositionOnBlueprint.UpLeft,  //swapped axis on purpose
                            selectedRoomBlueprint.RightUpAxisLength - rawDoorPositionOnBlueprint.UpRight);
                        Logger.Log("doorPositionOnExistingRoom {0}, {1} - doorPositionOnBlueprint 90 {2}, {3}", doorPositionOnExistingRoom.UpRight, doorPositionOnExistingRoom.UpLeft, doorPositionOnBlueprint.UpRight, doorPositionOnBlueprint.UpLeft);
                        if ((doorPositionOnExistingRoom.UpRight == doorPositionOnBlueprint.UpRight) && (doorPositionOnExistingRoom.UpLeft == doorPositionOnBlueprint.UpLeft)) continue;

                        GridLocation blueprintRoomStartPosition = new GridLocation(doorPositionOnExistingRoom.UpRight - doorPositionOnBlueprint.UpRight, (doorPositionOnExistingRoom.UpLeft - doorPositionOnBlueprint.UpLeft));

                        if (blueprintRoomStartPosition.UpLeft % 3 != 0 || blueprintRoomStartPosition.UpRight % 3 != 0)
                        {
                            Logger.Log(Logger.Building, "This room cannot be built because it would not start on a (large) tile location. Its start position would be {0}, {1}. Skip. ", blueprintRoomStartPosition.UpRight, blueprintRoomStartPosition.UpLeft);
                            continue;
                        }

                        RoomRotation roomRotation = RoomRotation.Rotation90;
                        Vector2 blueprintRoomStartPositionVector = GridHelper.CalculateLocationOnGrid((int)blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
                        if (GetPlotIsAvailable(selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation))
                        {
                            BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation);
                            Logger.Warning("created 90 degrees plot for door!");
                            createdPlotForDoor = true;
                        }
                    }
                }

                // 180 degrees turned
                if (!createdPlotForDoor)
                {
                    Logger.Log("Try in third rotation, swap x axis 180 degrees");
                    for (int k = 0; k < selectedRoomBlueprint.DoorLocations.Length; k++)
                    {
                        // for each door location, check if a building plot is available starting at the roomDoorPosition - bluePrintDoorPosition
                        GridLocation rawDoorPositionOnBlueprint = selectedRoomBlueprint.DoorLocations[k];
                        GridLocation doorPositionOnBlueprint = new GridLocation(
                            selectedRoomBlueprint.RightUpAxisLength - rawDoorPositionOnBlueprint.UpRight - 1,
                            selectedRoomBlueprint.LeftUpAxisLength - rawDoorPositionOnBlueprint.UpLeft);
                        Logger.Log("doorPositionOnExistingRoom {0}, {1} - doorPositionOnBlueprint 180 {2}, {3}", doorPositionOnExistingRoom.UpRight, doorPositionOnExistingRoom.UpLeft, doorPositionOnBlueprint.UpRight, doorPositionOnBlueprint.UpLeft);
                        if ((doorPositionOnExistingRoom.UpRight == doorPositionOnBlueprint.UpRight) && (doorPositionOnExistingRoom.UpLeft == doorPositionOnBlueprint.UpLeft)) continue;

                        GridLocation blueprintRoomStartPosition = new GridLocation(doorPositionOnExistingRoom.UpRight - doorPositionOnBlueprint.UpRight, (doorPositionOnExistingRoom.UpLeft - doorPositionOnBlueprint.UpLeft));

                        if (blueprintRoomStartPosition.UpLeft % 3 != 0 || blueprintRoomStartPosition.UpRight % 3 != 0)
                        {
                            Logger.Log(Logger.Building, "This room cannot be built because it would not start on a (large) tile location. Its start position would be {0}, {1}. Skip. ", blueprintRoomStartPosition.UpRight, blueprintRoomStartPosition.UpLeft);
                            continue;
                        }

                        RoomRotation roomRotation = RoomRotation.Rotation180;
                        Vector2 blueprintRoomStartPositionVector = GridHelper.CalculateLocationOnGrid((int)blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
                        if (GetPlotIsAvailable(selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation))
                        {
                            BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation);
                            createdPlotForDoor = true;
                        }
                    }
                }

                // 270 degrees turned
                if (!createdPlotForDoor)
                {
                    Logger.Log("Try in fourth rotation, swap x axis 270 degrees");
                    for (int k = 0; k < selectedRoomBlueprint.DoorLocations.Length; k++)
                    {
                        // for each door location, check if a building plot is available starting at the roomDoorPosition - bluePrintDoorPosition
                        GridLocation rawDoorPositionOnBlueprint = selectedRoomBlueprint.DoorLocations[k];
                        GridLocation doorPositionOnBlueprint = new GridLocation(
                            selectedRoomBlueprint.LeftUpAxisLength - rawDoorPositionOnBlueprint.UpLeft,  //swapped axis on purpose
                            selectedRoomBlueprint.RightUpAxisLength - rawDoorPositionOnBlueprint.UpRight);
                        Logger.Log("doorPositionOnExistingRoom {0}, {1} - doorPositionOnBlueprint 270 {2}, {3}", doorPositionOnExistingRoom.UpRight, doorPositionOnExistingRoom.UpLeft, doorPositionOnBlueprint.UpRight, doorPositionOnBlueprint.UpLeft);
                        if ((doorPositionOnExistingRoom.UpRight == doorPositionOnBlueprint.UpRight) && (doorPositionOnExistingRoom.UpLeft == doorPositionOnBlueprint.UpLeft)) continue;

                        GridLocation blueprintRoomStartPosition = new GridLocation(doorPositionOnExistingRoom.UpRight - doorPositionOnBlueprint.UpRight, (doorPositionOnExistingRoom.UpLeft - doorPositionOnBlueprint.UpLeft));
                        Logger.Log(Logger.Building, "Its start position would be {0}, {1}. ", blueprintRoomStartPosition.UpRight, blueprintRoomStartPosition.UpLeft);

                        if (blueprintRoomStartPosition.UpLeft % 3 != 0 || blueprintRoomStartPosition.UpRight % 3 != 0)
                        {
                            Logger.Log(Logger.Building, "This room cannot be built because it would not start on a (large) tile location. Its start position would be {0}, {1}. Skip. ", blueprintRoomStartPosition.UpRight, blueprintRoomStartPosition.UpLeft);
                            continue;
                        }

                        RoomRotation roomRotation = RoomRotation.Rotation270;
                        Vector2 blueprintRoomStartPositionVector = GridHelper.CalculateLocationOnGrid((int)blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
                        if (GetPlotIsAvailable(selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation))
                        {
                            BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation);
                            createdPlotForDoor = true;
                        }
                    }
                }
            }


        }

        //Initial room space to avoid an empty map.
        if (RoomManager.Rooms.Count == 0)
        {
            BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, new Vector2(0, 0), RoomRotation.Rotation0);
        }
    }

    private BuildingTile GetBuildingTileForAvailability(int rightUpAxisFromStartingPoint, int leftUpAxisFromStartingPoint, Vector2 plotLocationStartingPoint, List<BuildingTile> roomSquareTiles)
    {
        Vector2 location = GridHelper.CalculateLocationOnGrid(plotLocationStartingPoint, rightUpAxisFromStartingPoint, -leftUpAxisFromStartingPoint);

        BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

        return tile;
    }
}
