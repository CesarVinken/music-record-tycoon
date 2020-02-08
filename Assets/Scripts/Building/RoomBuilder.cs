using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public void BuildRoom(RoomBlueprint roomBlueprint, Vector2 startingPoint, BuildingTileBuilder buildingTileBuilder)
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

        FollowUpRoomBuilding(roomBlueprint);
    }

    public void FollowUpRoomBuilding(RoomBlueprint roomBlueprint)
    {
        switch (roomBlueprint.RoomName)
        {
            case RoomName.Hallway:
                // create follow up icons for longer hallways
                break;
            default:
                break;
        }

        // Distract money, update stats etc?
    }
}
