// The Blueprints and their info are used in the store. All room objects that can be created through the UI should have a blueprint version
public class RoomObjectBlueprint : BuildItemBlueprint<RoomObjectBlueprint>
{
    public RoomObjectName RoomObjectName;
    public ObjectInteraction[] ObjectInteractions = new ObjectInteraction[] { };
    public CharacterRoutineType[] CharacterRoutines = new CharacterRoutineType[] { };

    private RoomObjectBlueprint(RoomObjectName roomObjectName)
    {
        RoomObjectName = roomObjectName;
        Price = -1;
    }

    public static RoomObjectBlueprint Create(RoomObjectName roomObjectName)
    {
        switch (roomObjectName)
        {
            case RoomObjectName.Guitar:
                return GuitarBlueprint();
            case RoomObjectName.Piano:
                return PianoBlueprint();
            case RoomObjectName.ControlRoomMicrophone:
                return ControlRoomMicrophoneBlueprint();
            case RoomObjectName.MixPanel:
                return MixPanelBlueprint();
            case RoomObjectName.Telephone:
                return TelephoneBlueprint();
            default:
                Logger.Error("Cannot find a creation function for blueprint {0}", roomObjectName);
                return null;
        }
    }

    protected override RoomObjectBlueprint WithName(string name)
    {
        Name = name;
        return this;
    }

    protected override RoomObjectBlueprint WithMenuDescription(string description)
    {
        Description = description;
        return this;
    }

    protected override RoomObjectBlueprint WithPrice(int price)
    {
        Price = price;
        return this;
    }

    private RoomObjectBlueprint WithObjectInteractions(ObjectInteraction[] objectInteractions)
    {
        ObjectInteractions = objectInteractions;
        return this;
    }

    private RoomObjectBlueprint WithCharacterRoutines(CharacterRoutineType[] characterRoutines)
    {
        CharacterRoutines = characterRoutines;
        return this;
    }

    private static RoomObjectBlueprint GuitarBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Guitar)
            .WithName("Guitar")
            .WithMenuDescription("A mean guitar")
            .WithPrice(5)
            .WithObjectInteractions(new ObjectInteraction[]
            {
                ObjectInteraction.Create(ObjectInteractionType.Perform, "Guitar plays itself")
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("It is a self-playing guitar")),
                ObjectInteraction.Create(ObjectInteractionType.Perform, "Play guitar", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("A mean guitar")),
                ObjectInteraction.Create(ObjectInteractionType.Practice, "Learn playing the guitar", ObjectInteractionCharacterRole.CharacterInRoom)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("Our hero hopes he will be as good Jimmy Hendrix one day."))
            });

        return blueprint;
    }

    private static RoomObjectBlueprint PianoBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Piano)
            .WithName("Piano")
            .WithMenuDescription("Be more like Mozart")
            .WithPrice(15)
            .WithObjectInteractions(new ObjectInteraction[]
            {
                ObjectInteraction.Create(ObjectInteractionType.Perform, "Self-play")
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("It is a self-playing piano")),
                ObjectInteraction.Create(ObjectInteractionType.Perform, "Perform", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("Our hero is playing the guitar")),
                ObjectInteraction.Create(ObjectInteractionType.Practice, "Practice", ObjectInteractionCharacterRole.CharacterInRoom)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("Our hero hopes he will play as Rachmaninov one day.")),
                ObjectInteraction.Create(ObjectInteractionType.Repair, "Repair", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("The piano is no longer broken!"))
            });

        return blueprint;
    }

    private static RoomObjectBlueprint ControlRoomMicrophoneBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.ControlRoomMicrophone)
            .WithName("Microphone")
            .WithMenuDescription("Test one, two")
            .WithObjectInteractions(new ObjectInteraction[]
            {
                ObjectInteraction.Create(ObjectInteractionType.Perform, "Speak", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("Everyone is listening to the instructions"))
            })
            .WithCharacterRoutines(new CharacterRoutineType[]
            {
                CharacterRoutineType.Create(CharacterRoutineTypeName.Sing)
            });

        return blueprint;
    }

    private static RoomObjectBlueprint MixPanelBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.MixPanel)
            .WithName("Mix panel")
            .WithMenuDescription("Turn everything up to 11")
            .WithObjectInteractions(new ObjectInteraction[]
            {
                ObjectInteraction.Create(ObjectInteractionType.Record, "Remix", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("The song now sounds even better")),
                ObjectInteraction.Create(ObjectInteractionType.Record, "Record song", ObjectInteractionCharacterRole.CharacterAtRoomObject)
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("A new song was recorded"))
            })
            .WithCharacterRoutines(new CharacterRoutineType[]
            {

            });

        return blueprint;
    }

    private static RoomObjectBlueprint TelephoneBlueprint()
    {
        RoomObjectBlueprint blueprint = new RoomObjectBlueprint(RoomObjectName.Telephone)
            .WithName("Telephone")
            .WithMenuDescription("A classic black telephone")
            .WithObjectInteractions(new ObjectInteraction[] 
            {
                ObjectInteraction.Create(ObjectInteractionType.Contact, "Call the police")
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("You asked the police to removed the loud, beared hippies from your studio")),
                ObjectInteraction.Create(ObjectInteractionType.Contact, "Order a pizza")
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("You ordered a pizza for the whole band")),
                ObjectInteraction.Create(ObjectInteractionType.Contact, "Organise a tour")
                    .AddInteractionStep(InteractionStep.Create().WithSequenceLine("The band is going on tour again!"))
            })
            .WithCharacterRoutines(new CharacterRoutineType[]
            {
                CharacterRoutineType.Create(CharacterRoutineTypeName.MakePhoneCall)
            });

        return blueprint;
    }
}
