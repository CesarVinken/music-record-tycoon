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
        ObjectInteractionRunner.InteractingCharacter = InteractingCharacter;

        Logger.Log("OptionType: {0}", OptionType);
        if (OptionType == ObjectInteractionOptionType.CharacterMenuTrigger)
        {
            // display new options menu to choose character for picked interaction
            OnScreenTextContainer.Instance.CreateObjectInteractionTextContainer(RoomObject, ObjectInteractionOptionsMenuType.CharacterOptionsMenu);
            return;
        }

        //if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.NoCharacter)
        //{
        //    GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction, RoomObjectLocation);
        //    await Task.Delay(3000);

        //    if (interactionSequenceLine != null)
        //        OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);

        //    return;
        //}

        ObjectInteractionRunner.Run();
    }

    //public async Task MoveToInteractionLocation(Character character, Vector2 roomObjectLocation, Vector2 characterTarget)
    //{
    //    if(ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterAtRoomObject)
    //    {
    //        while (Vector2.Distance(character.transform.position, RoomObjectLocation) > 12)
    //        {
    //            await Task.Yield();
    //            if (roomObjectLocation != RoomObjectLocation)
    //                return;
    //            if (characterTarget != character.NavActor.Target)
    //                return;
    //            if (RoomObject == null)
    //                return;
    //        }
    //    }
    //    else if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterInRoom)
    //    {
    //        while (character.CurrentRoom != RoomObject.ParentRoom)
    //        {
    //            await Task.Yield();
    //            if (roomObjectLocation != RoomObjectLocation)
    //                return;
    //            if (characterTarget != character.NavActor.Target)
    //                return;
    //            if (RoomObject == null)
    //                return;
    //            if (RoomObject.ParentRoom == null)
    //                return;
    //        }
    //    }

    //    return;
    //}
}
