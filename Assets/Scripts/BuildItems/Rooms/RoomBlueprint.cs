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
            case RoomName.Room2:
                return CreateRoom2Blueprint();
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

    // test room with multiple doors
    private static RoomBlueprint CreateRoom2Blueprint()
    {
        RoomBlueprint blueprint = new RoomBlueprint(RoomName.Room2, "Room2", "This room is just for testing");

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
            new GridLocation(0, 2),
            new GridLocation(1, 3),
            new GridLocation(3, 2),
            new GridLocation(1, 0),
        };

        return blueprint;
    }
}
