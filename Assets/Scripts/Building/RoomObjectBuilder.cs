using UnityEngine;

public class RoomObjectBuilder
{
    // Used when player builds a new object. Not used for the default objects that already are part of the room, as they do not need a separate GO instantiation.
    public void BuildRoomObject(RoomObjectBlueprint roomObjectBlueprint, GridLocation roomObjectLocation, Room parentRoom)
    {
        Logger.Log("Trying to put a {0} in the room", roomObjectBlueprint.Name);
        GridLocation rotationTranslatedObjectLocation = TranslateObjectLocationForRoomRotation(roomObjectLocation, parentRoom);

        Vector2 roomObjectLocalLocation = GridHelper.GridToVectorLocation(rotationTranslatedObjectLocation);
        Vector2 roomObjectWorldPosition = new Vector2(parentRoom.RoomCorners[Direction.Down].x + roomObjectLocalLocation.x, parentRoom.RoomCorners[Direction.Down].y + roomObjectLocalLocation.y);
        
        GameObject roomObjectGO = GameManager.Instance.InstantiatePrefab(
            BuilderManager.Instance.PlaceableRoomObjectPrefabs[roomObjectBlueprint.RoomObjectName][ObjectRotation.Rotation0],
            parentRoom.RoomObjectsContainer.transform,
            roomObjectWorldPosition);

        RoomObjectGO roomObject = roomObjectGO.GetComponent<RoomObjectGO>();
        roomObject.Initialise(roomObjectBlueprint, parentRoom.RoomRotation, parentRoom);
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
