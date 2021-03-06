﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomBuilder
{
    public void BuildRoom(RoomBlueprint roomBlueprint, BuildingTileBuilder buildingTileBuilder, BuildingPlotBuilder buildingPlotBuilder, BuildingPlot buildingPlot)
    {
        BuildRoom(roomBlueprint, buildingTileBuilder, buildingPlotBuilder, buildingPlot.StartingPoint, buildingPlot.PlotRotation);
    }

    public void BuildRoom(RoomBlueprint roomBlueprint, BuildingTileBuilder buildingTileBuilder, BuildingPlotBuilder buildingPlotBuilder, Vector2 startingPoint, ObjectRotation roomRotation)
    {
        GameObject roomGO = GameManager.Instance.InstantiatePrefab(BuilderManager.Instance.RegisteredRoomPrefabs[roomBlueprint.RoomName][roomRotation], BuilderManager.Instance.RoomsContainer.transform, startingPoint);

        Room room = roomGO.GetComponent<Room>();
        room.RoomRotation = roomRotation;
        room.RoomBlueprint = roomBlueprint;
        room.Initialise();
        room.RoomObjectsContainer.InitialiseRoomObjects();
        RoomManager.Instance.AddRoom(room);

        int rightUpAxisLength = roomRotation == ObjectRotation.Rotation0 || roomRotation == ObjectRotation.Rotation180 ?
            roomBlueprint.RightUpAxisLength : roomBlueprint.LeftUpAxisLength;
        int leftUpAxisLength = roomRotation == ObjectRotation.Rotation0 || roomRotation == ObjectRotation.Rotation180 ?
            roomBlueprint.LeftUpAxisLength : roomBlueprint.RightUpAxisLength;

        Vector2 point1 = GridHelper.GridToVectorLocation(startingPoint, rightUpAxisLength, 0);
        Vector2 point2 = GridHelper.GridToVectorLocation(point1, 0, -leftUpAxisLength);
        Vector2 point3 = GridHelper.GridToVectorLocation(point2, -rightUpAxisLength, 0);

        Dictionary<Direction, Vector2> roomCorners = new Dictionary<Direction, Vector2>()
        {
            { Direction.Down, startingPoint },
            { Direction.Right, point1 },
            { Direction.Up, point2 },
            { Direction.Left, point3 },
        };

        room.SetupCorners(roomCorners);
        room.SetupCollider();
        room.SetGraphUpdateScenePoints();

        // the sprites should already be part of the room prefab. The scripts should be on the sprites. But on start() of the room, all scripts should be initalised and added to the room's []
        //room.SetupRoomObjects();

        buildingTileBuilder.UpdateBuildingTiles(room);

        FollowUpRoomBuilding(roomBlueprint, roomCorners, buildingPlotBuilder);
        room.UpdateRoomNavhMesh();
    }

    public void FollowUpRoomBuilding(RoomBlueprint roomBlueprint, Dictionary<Direction, Vector2> roomCorners, BuildingPlotBuilder buildingPlotBuilder)
    {
        switch (roomBlueprint.RoomName)
        {
            case RoomName.Hallway:
                // create follow up icons for longer hallways
                Vector2 pointLeftUp = roomCorners[Direction.Left];
                Vector2 pointLeftDown = GridHelper.GridToVectorLocation(roomCorners[Direction.Down], -3, 0);
                Vector2 pointRightDown = GridHelper.GridToVectorLocation(roomCorners[Direction.Down], 0, 3);
                Vector2 pointRightUp = roomCorners[Direction.Right];

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointLeftUp, ObjectRotation.Rotation0))
                    CreateBuildHallwayTrigger(pointLeftUp, ObjectDirection.LeftUp);

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointLeftDown, ObjectRotation.Rotation0))
                    CreateBuildHallwayTrigger(pointLeftDown, ObjectDirection.LeftDown);

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointRightDown, ObjectRotation.Rotation0))
                    CreateBuildHallwayTrigger(pointRightDown, ObjectDirection.RightDown);

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointRightUp, ObjectRotation.Rotation0))
                    CreateBuildHallwayTrigger(pointRightUp, ObjectDirection.RightUp);
                break;
            default:
                break;
        }
    }

    public void CreateBuildHallwayTrigger(Vector2 startingPoint, ObjectDirection direction)
    {
        BuildHallwayTrigger buildHallwayTrigger = GameManager.Instance.InstantiatePrefab(
            BuilderManager.Instance.BuildHallwayTriggerPrefab,
            MainCanvas.Instance.TriggersContainer.transform,
            startingPoint).GetComponent<BuildHallwayTrigger>();
        buildHallwayTrigger.Setup(startingPoint, direction);
    }
}
