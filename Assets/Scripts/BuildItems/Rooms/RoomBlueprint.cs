public class RoomBlueprint : BuildItemBlueprint<RoomBlueprint>
{
    public RoomName RoomName;
    
    public int RightUpAxisLength;
    public int LeftUpAxisLength;

    public GridLocation[] DoorLocations;

    protected RoomBlueprint(RoomName roomName)
    {
        RoomName = roomName;
    }

    public static RoomBlueprint CreateBlueprint(RoomName roomName)
    {
        switch (roomName)
        {
            case RoomName.Hallway:
                return CreateHallwayBlueprint();
            case RoomName.Room1:
                return CreateRoom1Blueprint();
            case RoomName.RecordingStudio1:
                return CreateRecordingStudio1Blueprint();
            default:
                Logger.Error("Cannot find a creation function for blueprint {0}", roomName);
                return null;
        }
    }

    public override RoomBlueprint WithName(string name)
    {
        Name = name;
        return this;
    }

    public override RoomBlueprint WithMenuDescription(string description)
    {
        Description = description;
        return this;
    }

    public struct RoomObjectBlueprintForRoom
    {
        public RoomObjectBlueprint RoomObjectBlueprint;
        public GridLocation RoomObjectLocation;

        public RoomObjectBlueprintForRoom(RoomObjectBlueprint roomObjectBlueprint, GridLocation roomObjectLocation)
        {
            RoomObjectBlueprint = roomObjectBlueprint;
            RoomObjectLocation = roomObjectLocation;
        }
    }

    // test room with one door
    private static RoomBlueprint CreateRoom1Blueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Room1)
        .WithName("Room1")
        .WithMenuDescription("This room has only 1 door");

        blueprint.RightUpAxisLength = 9;
        blueprint.LeftUpAxisLength = 6;
        blueprint.DoorLocations = new GridLocation[]
        {
            new GridLocation(4, 0)
        };

        return blueprint;
    }

    // recording room 1, for testing
    private static RoomBlueprint CreateRecordingStudio1Blueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.RecordingStudio1)
            .WithName("Recording Studio 1")
            .WithMenuDescription("Your first recording studio! But this is only the control room part.");

        blueprint.RightUpAxisLength = 3;
        blueprint.LeftUpAxisLength = 6;
        blueprint.DoorLocations = new GridLocation[]
        {
            new GridLocation(1, 6),
            new GridLocation(1, 0),
        };

        return blueprint;
    }

    private static RoomBlueprint CreateHallwayBlueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Hallway)
            .WithName("Hallway")
            .WithMenuDescription("A nice hallway");

        blueprint.RightUpAxisLength = 3;
        blueprint.LeftUpAxisLength = 3;
        blueprint.DoorLocations = new GridLocation[]
        {
            new GridLocation(0, 2),
            new GridLocation(1, 3),
            new GridLocation(3, 2),
            new GridLocation(1, 0),
        };

        return blueprint;
    }
}
