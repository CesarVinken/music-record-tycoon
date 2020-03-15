// The Blueprints and their info are used in the store. All room objects that can be created through the UI should have a blueprint version
public class RoomObjectBlueprint : BuildItemBlueprint
{
    public RoomObjectName RoomObjectName;

    protected RoomObjectBlueprint(RoomObjectName roomObjectName, string name, string description) : base(name, description)
    {
        RoomObjectName = roomObjectName;
    }

    public static RoomObjectBlueprint CreateBlueprint(RoomObjectName roomObjectName)
    {
        switch (roomObjectName)
        {
            case RoomObjectName.Guitar:
                return CreateGuitarBlueprint();
            case RoomObjectName.Piano:
                return CreatePianoBlueprint();
            default:
                Logger.Error("Cannot find a creation function for blueprint {0}", roomObjectName);
                return null;
        }
    }

    // test room with one door
    private static RoomObjectBlueprint CreateGuitarBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Guitar, "Guitar", "A mean guitar");

        // add interaction specifics

        return blueprint;
    }

    private static RoomObjectBlueprint CreatePianoBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Piano, "Piano", "Be more like mozart");

        // add interaction specifics

        return blueprint;
    }

}
