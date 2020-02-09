using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public void BuildRoom(RoomBlueprint roomBlueprint, Vector2 startingPoint, BuildingTileBuilder buildingTileBuilder, BuildingPlotBuilder buildingPlotBuilder)
    {
        GameObject roomGO = Instantiate(BuilderManager.Instance.RoomPrefabs[roomBlueprint.RoomName], BuilderManager.Instance.RoomsContainer.transform);
        roomGO.transform.position = startingPoint;

        Room room = roomGO.GetComponent<Room>();
        RoomManager.Instance.AddRoom(room);

        Vector2 point1 = GridHelper.CalculateLocationOnGrid(startingPoint, roomBlueprint.RightUpAxisLength, 0);
        Vector2 point2 = GridHelper.CalculateLocationOnGrid(point1, 0, -roomBlueprint.LeftUpAxisLength);
        Vector2 point3 = GridHelper.CalculateLocationOnGrid(point2, -roomBlueprint.RightUpAxisLength, 0);

        Dictionary<Direction, Vector2> roomCorners = new Dictionary<Direction, Vector2>()
        {
            { Direction.Down, startingPoint },
            { Direction.Right, point1 },
            { Direction.Up, point2 },
            { Direction.Left, point3 },
        };
        room.RoomBlueprint = roomBlueprint;
        room.SetupCorners(roomCorners);
        room.SetupCollider(roomBlueprint);

        buildingTileBuilder.UpdateBuildingTiles(room);

        // When building a room that is next to the room where the currently selected character is, then the bordering wall graphicals should be switched to the 'low' wall versions
        for (int i = 0; i < room.AdjacentRooms.Count; i++)
        {
            if (room.AdjacentRooms[i].Id == PlayerCharacter.Instance.CurrentRoom.Id)
            {
                PlayerCharacter.Instance.CurrentRoom.LowerWallPieces();
            }
        }

        FollowUpRoomBuilding(roomBlueprint, roomCorners, buildingPlotBuilder);
    }

    public void FollowUpRoomBuilding(RoomBlueprint roomBlueprint, Dictionary<Direction, Vector2> roomCorners, BuildingPlotBuilder buildingPlotBuilder)
    {
        switch (roomBlueprint.RoomName)
        {
            case RoomName.Hallway:
                // create follow up icons for longer hallways
                Vector2 pointLeftUp = roomCorners[Direction.Left];
                Vector2 pointLeftDown = GridHelper.CalculateLocationOnGrid(roomCorners[Direction.Down], -3, 0);
                Vector2 pointRightDown = GridHelper.CalculateLocationOnGrid(roomCorners[Direction.Down], 0, 3);
                Vector2 pointRightUp = roomCorners[Direction.Right];

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointLeftUp))
                    CreateBuildHallwayTrigger(pointLeftUp, ObjectDirection.LeftUp);

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointLeftDown))
                    CreateBuildHallwayTrigger(pointLeftDown, ObjectDirection.LeftDown);

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointRightDown))
                    CreateBuildHallwayTrigger(pointRightDown, ObjectDirection.RightDown);

                if (buildingPlotBuilder.GetPlotIsAvailable(roomBlueprint, pointRightUp))
                    CreateBuildHallwayTrigger(pointRightUp, ObjectDirection.RightUp);
                break;
            default:
                break;
        }

        // Distract money, update stats etc?
    }

    public void CreateBuildHallwayTrigger(Vector2 startingPoint, ObjectDirection direction)
    {

        BuildHallwayTrigger buildHallwayTrigger = Instantiate(BuilderManager.Instance.BuildHallwayTriggerPrefab, MainCanvas.Instance.TriggersContainer.transform).GetComponent<BuildHallwayTrigger>();
        buildHallwayTrigger.Setup(startingPoint, direction);
    }
}
