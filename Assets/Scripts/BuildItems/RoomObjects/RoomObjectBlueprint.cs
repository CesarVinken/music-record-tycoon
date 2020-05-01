﻿// The Blueprints and their info are used in the store. All room objects that can be created through the UI should have a blueprint version
public class RoomObjectBlueprint : BuildItemBlueprint
{
    public RoomObjectName RoomObjectName;
    public ObjectInteraction[] ObjectInteractions = new ObjectInteraction[] { };

    protected RoomObjectBlueprint(RoomObjectName roomObjectName, string name = "tba", string description = "tba") : base(name, description)
    {
        RoomObjectName = roomObjectName;
    }

    public RoomObjectBlueprint WithName(string name)
    {
        Name = name;
        return this;
    }

    public RoomObjectBlueprint WithMenuDescription(string description)
    {
        Description = description;
        return this;
    }

    public static RoomObjectBlueprint CreateBlueprint(RoomObjectName roomObjectName)
    {
        switch (roomObjectName)
        {
            case RoomObjectName.Guitar:
                return CreateGuitarBlueprint();
            case RoomObjectName.Piano:
                return CreatePianoBlueprint();
            case RoomObjectName.ControlRoomMicrophone:
                return CreateControlRoomMicrophoneBlueprint();
            case RoomObjectName.MixPanel:
                return CreateMixPanelBlueprint();
            case RoomObjectName.Telephone:
                return CreateTelephoneBlueprint();
            default:
                Logger.Error("Cannot find a creation function for blueprint {0}", roomObjectName);
                return null;
        }
    }

    private static RoomObjectBlueprint CreateGuitarBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Guitar)
            .WithName("Guitar")
            .WithMenuDescription("A mean guitar");

        // add interaction specifics
        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Perform, "Guitar plays itself")
                .AddInteractionStep(new InteractionStep().WithSequenceLine("It is a self-playing guitar")),
            new ObjectInteraction(ObjectInteractionType.Perform, "Play guitar", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("A mean guitar")),
            new ObjectInteraction(ObjectInteractionType.Practice, "Learn playing the guitar", ObjectInteractionCharacterRole.CharacterInRoom)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("Our hero hopes he will be as good Jimmy Hendrix one day."))
        };
        return blueprint;
    }

    private static RoomObjectBlueprint CreatePianoBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Piano, "Piano", "Be more like Mozart")
            .WithName("Piano")
            .WithMenuDescription("Be more like Mozart");

        // add interaction specifics
        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Perform, "Self-play")
                .AddInteractionStep(new InteractionStep().WithSequenceLine("It is a self-playing piano")),
            new ObjectInteraction(ObjectInteractionType.Perform, "Perform", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("Our hero is playing the guitar")),
            new ObjectInteraction(ObjectInteractionType.Practice, "Practice", ObjectInteractionCharacterRole.CharacterInRoom)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("Our hero hopes he will play as Rachmaninov one day.")),
            new ObjectInteraction(ObjectInteractionType.Repair, "Repair", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("The piano is no longer broken!"))
        };
        blueprint.ObjectInteractions = objectInteractions;

        return blueprint;
    }

    private static RoomObjectBlueprint CreateControlRoomMicrophoneBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.ControlRoomMicrophone)
            .WithName("Microphone")
            .WithMenuDescription("Test one, two");

        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Perform, "Speak", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("Everyone is listening to the instructions")),
        };
        blueprint.ObjectInteractions = objectInteractions;

        return blueprint;
    }

    private static RoomObjectBlueprint CreateMixPanelBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.MixPanel)
            .WithName("Mix panel")
            .WithMenuDescription("Turn everything up to 11");

        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Record, "Remix", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("The song now sounds even better")),
            new ObjectInteraction(ObjectInteractionType.Record, "Record song", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                .AddInteractionStep(new InteractionStep().WithSequenceLine("A new song was recorded"))
        };
        blueprint.ObjectInteractions = objectInteractions;

        return blueprint;
    }

    private static RoomObjectBlueprint CreateTelephoneBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.ControlRoomMicrophone)
            .WithName("Microphone")
            .WithMenuDescription("Test one, two");

        ObjectInteraction[] objectInteractions = new ObjectInteraction[] {
            new ObjectInteraction(ObjectInteractionType.Contact, "Call the police")
                .AddInteractionStep(new InteractionStep().WithSequenceLine("You asked the police to removed the loud, beared hippies from your studio")),
            new ObjectInteraction(ObjectInteractionType.Contact, "Order a pizza")
                .AddInteractionStep(new InteractionStep().WithSequenceLine("You ordered a pizza for the whole band")),
            new ObjectInteraction(ObjectInteractionType.Contact, "Organise a tour")
                .AddInteractionStep(new InteractionStep().WithSequenceLine("The band is going on tour again!"))
        };
        blueprint.ObjectInteractions = objectInteractions;

        return blueprint;
    }
}
