using System.Threading.Tasks;
using UnityEngine;

public class RoomObjectBuilder
{
    public void BuildRoomObject(RoomObjectBlueprint roomObjectBlueprint, GridLocation roomObjectLocation, Room parentRoom)
    {
        Logger.Log("Try to put a {0} in the room", roomObjectBlueprint.Name);
        GameObject roomObjectGO = GameManager.Instance.InstantiatePrefab(BuilderManager.Instance.RoomObjectPrefabs[roomObjectBlueprint.RoomObjectName][ObjectRotation.Rotation0], parentRoom.RoomObjectsContainer.transform);

        GridLocation rotationTranslatedObjectLocation = TranslateObjectLocationForRoomRotation(roomObjectLocation, parentRoom);
        
        Vector2 roomObjectLocalLocation = GridHelper.GridToVectorLocation(rotationTranslatedObjectLocation);
        roomObjectGO.transform.position = new Vector2(parentRoom.RoomCorners[Direction.Down].x + roomObjectLocalLocation.x, parentRoom.RoomCorners[Direction.Down].y + roomObjectLocalLocation.y);


        RoomObject roomObject = roomObjectGO.GetComponent<RoomObject>();
        parentRoom.RoomObjectsContainer.AddRoomObject(roomObject);
    }

    public GridLocation TranslateObjectLocationForRoomRotation(GridLocation roomObjectLocation, Room parentRoom)
    {
        switch (parentRoom.RoomRotation)
        {
            case ObjectRotation.Rotation0:
                return roomObjectLocation;
            case ObjectRotation.Rotation90:
                return new GridLocation(roomObjectLocation.UpLeft, parentRoom.RoomBlueprint.RightUpAxisLength - roomObjectLocation.UpRight);
            case ObjectRotation.Rotation180:
                return new GridLocation(parentRoom.RoomBlueprint.RightUpAxisLength - roomObjectLocation.UpRight, parentRoom.RoomBlueprint.LeftUpAxisLength - roomObjectLocation.UpLeft);
            case ObjectRotation.Rotation270:
                return new GridLocation(parentRoom.RoomBlueprint.LeftUpAxisLength - roomObjectLocation.UpLeft, roomObjectLocation.UpRight);
            default:
                Logger.Error("Cannot find implementation for rotation {0}", parentRoom.RoomRotation);
                return roomObjectLocation;
        }
    }
}
