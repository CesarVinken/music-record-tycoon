using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionOptionButton : MonoBehaviour
{
    public Text InteractionOptionText;

    public ObjectInteraction ObjectInteraction;
    public RoomObjectGO RoomObject;
    public Character InteractingCharacter;
    public ObjectInteractionOptionType OptionType;

    public void Awake()
    {
        if (InteractionOptionText == null)
            Logger.Log(Logger.Initialisation, "Could not find InteractionOptionText component on ObjectInteractionOption");
    }

    public void Initialise(ObjectInteraction objectInteraction, RoomObjectGO roomObject, ObjectInteractionOptionType optionType)
    {
        ObjectInteraction = objectInteraction;
        RoomObject = roomObject;
        OptionType = optionType;
    }

    public void SetInteractingCharacter(Character character)
    {
        InteractingCharacter = character;
    }

    public void SetInteractionOptionText(string text)
    {
        InteractionOptionText.text = text;
    }

    public void Run()
    {
        OnScreenTextContainer.Instance.DeleteObjectInteractionTextContainer();

        //objectInteractionRunner.ObjectInteraction = ObjectInteraction;
        //objectInteractionRunner.RoomObject = RoomObject;
        //objectInteractionRunner.RoomObjectLocation = RoomObjectLocation;
        //objectInteractionRunner.InteractingCharacter = InteractingCharacter;
        ObjectInteractionRunner objectInteractionRunner = new ObjectInteractionRunner(ObjectInteraction, RoomObject, InteractingCharacter);

        if (OptionType == ObjectInteractionOptionType.CharacterMenuTrigger)
        {
            // display new options menu to choose character for picked interaction
            OnScreenTextContainer.Instance.CreateObjectInteractionTextContainer(RoomObject, ObjectInteractionOptionsMenuType.CharacterOptionsMenu, ObjectInteraction);
            return;
        }

        objectInteractionRunner.Run();

        //ObjectInteractionRunner objectInteractionRunner = new ObjectInteractionRunner();
        //objectInteractionRunner.Initialise(ObjectInteraction, RoomObject, RoomObjectLocation, InteractingCharacter);
    }
}
