// The Blueprints and their info are used in the store. All room objects that can be created through the UI should have a blueprint version
public class RoomObjectBlueprint : BuildItemBlueprint
{
    public RoomObjectName RoomObjectName;
    public ObjectInteraction[] ObjectInteractions = new ObjectInteraction[] { };

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
        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Perform, "Guitar plays itself", "It is a self-playing guitar"),
            new ObjectInteraction(ObjectInteractionType.Perform, "Play guitar", "A mean guitar", ObjectInteractionCharacterRole.CharacterAtRoomObject),
            new ObjectInteraction(ObjectInteractionType.Practice, "Learn playing the guitar", "Our hero hopes he will be as good Jimmy Hendrix one day.", ObjectInteractionCharacterRole.CharacterInRoom)
        };
        return blueprint;
    }

    private static RoomObjectBlueprint CreatePianoBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Piano, "Piano", "Be more like Mozart");

        // add interaction specifics
        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Perform, "Self-play", "It is a self-playing piano"),
            new ObjectInteraction(ObjectInteractionType.Perform, "Perform", "Our hero is playing the guitar", ObjectInteractionCharacterRole.CharacterAtRoomObject),
            new ObjectInteraction(ObjectInteractionType.Practice, "Practice", "Our hero hopes he will play as Rachmaninov one day.", ObjectInteractionCharacterRole.CharacterInRoom),
            new ObjectInteraction(ObjectInteractionType.Repair, "Repair", "The piano is no longer broken!", ObjectInteractionCharacterRole.CharacterAtRoomObject)
        };
        blueprint.ObjectInteractions = objectInteractions;

        return blueprint;
    }

}
