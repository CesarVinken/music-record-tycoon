using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionOptionButton : MonoBehaviour
{
    public Text InteractionOptionText;

    public ObjectInteraction ObjectInteraction;
    public Vector2 RoomObjectLocation;
    public RoomObject RoomObject;
    public Character InteractingCharacter;
    public ObjectInteractionOptionType OptionType;

    public void Awake()
    {
        if (InteractionOptionText == null)
            Logger.Log(Logger.Initialisation, "Could not find InteractionOptionText component on ObjectInteractionOption");
    }

    public void Initialise(ObjectInteraction objectInteraction, RoomObject roomObject, ObjectInteractionOptionType optionType)
    {
        ObjectInteraction = objectInteraction;
        RoomObjectLocation = roomObject.transform.position;
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
        ObjectInteractionRunner.ObjectInteraction = ObjectInteraction;
        ObjectInteractionRunner.RoomObject = RoomObject;
        ObjectInteractionRunner.RoomObjectLocation = RoomObjectLocation;
        ObjectInteractionRunner.InteractingCharacter = InteractingCharacter;

        if (OptionType == ObjectInteractionOptionType.CharacterMenuTrigger)
        {
            // display new options menu to choose character for picked interaction
            OnScreenTextContainer.Instance.CreateObjectInteractionTextContainer(RoomObject, ObjectInteractionOptionsMenuType.CharacterOptionsMenu);
            return;
        }

        Logger.Log("Run interaction");

        ObjectInteractionRunner.Run();
    }
}
