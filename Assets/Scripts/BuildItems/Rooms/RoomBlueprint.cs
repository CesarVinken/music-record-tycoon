public class RoomBlueprint : BuildItemBlueprint
{
    public int RightUpAxisLength;
    public int LeftUpAxisLength;
    public GridLocation[] DoorLocations;
    protected RoomBlueprint(RoomName roomName, string name, string description) : base(roomName, name, description)
    {
    }

    public static RoomBlueprint CreateBlueprint(RoomName roomName)
    {
        switch (roomName)
        {
            case RoomName.Hallway:
                return CreateHallwayBlueprint();
            case RoomName.Room1:
                return CreateRoom1Blueprint();
            default:
                Logger.Error("Cannot find a creation function for blueprint {0}", roomName);
                return null;
        }
    }

    private static RoomBlueprint CreateRoom1Blueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Room1, "Room1", "This room is just for testing");

        blueprint.RightUpAxisLength = 9;
        blueprint.LeftUpAxisLength = 6;
        blueprint.DoorLocations = new GridLocation[]
        {
            new GridLocation(3, 0),
            new GridLocation(3, 6),
            new GridLocation(0, 3),
            new GridLocation(9, 5),
            new GridLocation(0, 2),
        };

        return blueprint;
    }

    private static RoomBlueprint CreateHallwayBlueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Hallway, "Hallway", "A nice hallway");

        blueprint.RightUpAxisLength = 3;
        blueprint.LeftUpAxisLength = 3;
        blueprint.DoorLocations = new GridLocation[]
        {
            new GridLocation(1, 0),
            new GridLocation(0, 2),
            new GridLocation(3, 2),
            new GridLocation(1, 3),
        };

        return blueprint;
    }
}

public struct GridLocation
{
    public float UpRight;
    public float UpLeft;

    public GridLocation(float upRight, float upLeft)
    {
        UpRight = upRight;    
        UpLeft = upLeft;
    }
}