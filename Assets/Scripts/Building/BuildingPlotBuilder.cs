﻿
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

    // check all tiles where the plot would be drawn if the building tile is available. Only draw a plot when all tiles are available
    public bool GetPlotIsAvailable(RoomBlueprint roomBlueprint, Vector2 roomStartingPoint, ObjectRotation roomRotation)
    {
        int rightUpAxisLength = roomRotation == ObjectRotation.Rotation0 || roomRotation == ObjectRotation.Rotation180 ?
            roomBlueprint.RightUpAxisLength : roomBlueprint.LeftUpAxisLength;
        int leftUpAxisLength = roomRotation == ObjectRotation.Rotation0 || roomRotation == ObjectRotation.Rotation180 ?
            roomBlueprint.LeftUpAxisLength : roomBlueprint.RightUpAxisLength;
        Vector2 plotLocationStartingPoint = GridHelper.GridToVectorLocation(roomStartingPoint, 0, 0);
        Vector2 point1 = GridHelper.GridToVectorLocation(plotLocationStartingPoint, rightUpAxisLength, 0);
        Vector2 point2 = GridHelper.GridToVectorLocation(point1, 0, -leftUpAxisLength);
        Vector2 point3 = GridHelper.GridToVectorLocation(point2, -rightUpAxisLength, 0);

        List<BuildingTile> roomSquareTiles = _builderManager.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= point3.x &&
            tile.StartingPoint.x <= point1.x &&
            tile.StartingPoint.y <= point2.y &&
            tile.StartingPoint.y >= plotLocationStartingPoint.y
        );

        BuildingTile startingTile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == plotLocationStartingPoint);
        if (startingTile.IsAvailable == Availability.Unavailable) return false;

        bool isAvailable = true;

        for (int i = 0; i <= rightUpAxisLength - 3; i += 3)
        {
            for (int j = 0; j <= leftUpAxisLength - 3; j += 3)
            {
                BuildingTile tile = GetBuildingTileForAvailability(i, j, plotLocationStartingPoint, roomSquareTiles);

                //if (i == 0 || i == rightUpAxisLength || j == 0 || j == leftUpAxisLength)
                //{
                //    // skip tiles that are at the edge because they may overlap with the walls of adjacent rooms
                //    continue;
                //}

                if (tile == null)
                {
                    Logger.Error(Logger.Building, "Could not find tile at {0}", GridHelper.GridToVectorLocation(plotLocationStartingPoint, i, -j));
                }

                if (tile.IsAvailable == Availability.Unavailable)
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
            //Logger.Log("*********************");
            //Logger.Log("");
            //Logger.Log("Start looking at next room at {0}, {1}", existingRoom.RoomCorners[Direction.Down].x, existingRoom.RoomCorners[Direction.Down].y);
            //Logger.Log("");
            //Logger.Log("*********************");
            // for each door, check if it aligns with the doors of the selected room
            for (int j = 0; j < existingRoom.Doors.Count; j++)
            {
                Door existingDoor = existingRoom.Doors[j];
                if (existingDoor.DoorConnection != null) continue; // if the door is already activated, no need to check for new connections

                bool createdPlotForDoor = false;

                // the doors that are already on the map
                GridLocation doorPositionOnExistingRoom = GridHelper.VectorToGridLocation(existingDoor.transform.position);

                createdPlotForDoor = TryBuildPlot(ObjectRotation.Rotation0, selectedRoomBlueprint, doorPositionOnExistingRoom);
                if (!createdPlotForDoor)
                {
                    createdPlotForDoor = TryBuildPlot(ObjectRotation.Rotation90, selectedRoomBlueprint, doorPositionOnExistingRoom);
                }
                if (!createdPlotForDoor)
                {
                    createdPlotForDoor = TryBuildPlot(ObjectRotation.Rotation180, selectedRoomBlueprint, doorPositionOnExistingRoom);
                }
                if (!createdPlotForDoor)
                {
                    TryBuildPlot(ObjectRotation.Rotation270, selectedRoomBlueprint, doorPositionOnExistingRoom);
                }      
            }
        }

        //Initial room space to avoid an empty map.
        if (RoomManager.Rooms.Count == 0)
        {
            BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, new Vector2(0, 0), ObjectRotation.Rotation0);
        }
    }

    public bool TryBuildPlot(ObjectRotation roomRotation, RoomBlueprint selectedRoomBlueprint, GridLocation doorPositionOnExistingRoom)
    {
        for (int k = 0; k < selectedRoomBlueprint.DoorLocations.Length; k++)
        {
            GridLocation doorPositionOnBlueprint = getDoorPositionOnBlueprint(roomRotation, selectedRoomBlueprint, selectedRoomBlueprint.DoorLocations[k]);

            //Logger.Log("doorPositionOnExistingRoom {0}, {1} - doorPositionOnBlueprint {2} with rotation {3} is {4}, {5}",
            //    doorPositionOnExistingRoom.UpRight, doorPositionOnExistingRoom.UpLeft, selectedRoomBlueprint.RoomName, roomRotation, doorPositionOnBlueprint.UpRight, doorPositionOnBlueprint.UpLeft);
            if ((doorPositionOnExistingRoom.UpRight == doorPositionOnBlueprint.UpRight) && (doorPositionOnExistingRoom.UpLeft == doorPositionOnBlueprint.UpLeft)) continue;

            GridLocation blueprintRoomStartPosition = new GridLocation(doorPositionOnExistingRoom.UpRight - doorPositionOnBlueprint.UpRight, (doorPositionOnExistingRoom.UpLeft - doorPositionOnBlueprint.UpLeft));

            if (blueprintRoomStartPosition.UpLeft % 3 != 0 || blueprintRoomStartPosition.UpRight % 3 != 0)
            {
                Logger.Log(Logger.Building, "This room cannot be built because it would not start on a (large) tile location. Its start position would be {0}, {1}. Skip. ", blueprintRoomStartPosition.UpRight, blueprintRoomStartPosition.UpLeft);
                continue;
            }

            Vector2 blueprintRoomStartPositionVector = GridHelper.GridToVectorLocation((int)blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
            //Logger.Log("the blueprintRoomStartPosition would be {0}, {1}", blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
            //Logger.Log("the blueprintRoomStartPositionVector would be {0}, {1}", blueprintRoomStartPositionVector.x, blueprintRoomStartPositionVector.y);

            if (GetPlotIsAvailable(selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation))
            {
                BuildMenuWorldSpaceContainer.Instance.CreateBuildingPlot(_builderManager.BuildPlotPrefab, selectedRoomBlueprint, blueprintRoomStartPositionVector, roomRotation);
                Logger.Log(Logger.Building, "Created plot for building at {0}, {1}", blueprintRoomStartPositionVector.x, blueprintRoomStartPositionVector.y);
                return true;
            }
        }
        return false;
    }

    public GridLocation getDoorPositionOnBlueprint(ObjectRotation roomRotation, RoomBlueprint selectedRoomBlueprint, GridLocation door)
    {
        GridLocation rawDoorPositionOnBlueprint = door;

        switch (roomRotation)
        {
            case ObjectRotation.Rotation0:
                return door;
            case ObjectRotation.Rotation90:
                return new GridLocation(
                    rawDoorPositionOnBlueprint.UpLeft,  //swapped axis on purpose
                    selectedRoomBlueprint.RightUpAxisLength - rawDoorPositionOnBlueprint.UpRight);
            case ObjectRotation.Rotation180:
                return new GridLocation(
                    selectedRoomBlueprint.RightUpAxisLength - rawDoorPositionOnBlueprint.UpRight - 1,
                    selectedRoomBlueprint.LeftUpAxisLength - rawDoorPositionOnBlueprint.UpLeft);
            case ObjectRotation.Rotation270:
                return new GridLocation(
                    selectedRoomBlueprint.LeftUpAxisLength - rawDoorPositionOnBlueprint.UpLeft,  //swapped axis on purpose
                    selectedRoomBlueprint.RightUpAxisLength - rawDoorPositionOnBlueprint.UpRight);
            default:
                Logger.Error("Unknown RoomRotation given {0}", roomRotation);
                return new GridLocation();
        }
    }

    private BuildingTile GetBuildingTileForAvailability(int rightUpAxisFromStartingPoint, int leftUpAxisFromStartingPoint, Vector2 plotLocationStartingPoint, List<BuildingTile> roomSquareTiles)
    {
        Vector2 location = GridHelper.GridToVectorLocation(plotLocationStartingPoint, rightUpAxisFromStartingPoint, -leftUpAxisFromStartingPoint);

        BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

        return tile;
    }
}
