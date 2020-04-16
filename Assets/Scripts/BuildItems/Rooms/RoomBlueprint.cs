using UnityEngine;

public class RoomBlueprint : BuildItemBlueprint
{
    public RoomName RoomName;
    
    public int RightUpAxisLength;
    public int LeftUpAxisLength;

    public GridLocation[] DoorLocations;
    public RoomObjectBlueprintForRoom[] RoomObjects = new RoomObjectBlueprintForRoom[] { };

    protected RoomBlueprint(RoomName roomName, string name, string description) : base(name, description)
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
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Room1, "Room1", "This room has only 1 door");

        blueprint.RightUpAxisLength = 9;
        blueprint.LeftUpAxisLength = 6;
        blueprint.DoorLocations = new GridLocation[]
        {
            new GridLocation(4, 0)
        };
        blueprint.RoomObjects = new RoomObjectBlueprintForRoom[]
        {
            new RoomObjectBlueprintForRoom(RoomObjectBlueprint.CreateBlueprint(RoomObjectName.Piano), new GridLocation(8, 4))       
        };

        return blueprint;
    }

    // recording room 1, for testing
    private static RoomBlueprint CreateRecordingStudio1Blueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.RecordingStudio1, "Recording Studio 1", "Your first recording studio! But this is only the control room part.");

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
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Hallway, "Hallway", "A nice hallway");

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
