using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionOptionButton : MonoBehaviour
{
    public Text InteractionOptionText;

    public ObjectInteraction ObjectInteraction;
    public Vector2 RoomObjectLocation;
    RoomObject RoomObject;

    public void Awake()
    {
        if (InteractionOptionText == null)
            Logger.Log(Logger.Initialisation, "Could not find InteractionOptionText component on ObjectInteractionOption");
    }

    public void Initialise(ObjectInteraction objectInteraction, RoomObject roomObject, Vector2 roomObjectLocation)
    {
        ObjectInteraction = objectInteraction;
        RoomObjectLocation = roomObjectLocation;
        RoomObject = roomObject;
        InteractionOptionText.text = objectInteraction.Name;
    }

    public async void Run()
    {
        Character character = CharacterManager.Instance.SelectedCharacter;
        OnScreenTextContainer.Instance.DeleteObjectInteractionTextContainer();
        Logger.Log("Spawn text {0}", ObjectInteraction.Reaction);

        Vector2 roomObjectLocation = RoomObjectLocation;
        character.PlayerLocomotion.SetLocomotionTarget(RoomObjectLocation);
        Vector2 characterTarget = character.NavActor.Target;

        await MoveToInteractionLocation(character, roomObjectLocation, characterTarget);

        character.PlayerLocomotion.StopLocomotion(CharacterActionState.Action);
        character.PlayerLocomotion.SetLocomotionTarget(character.transform.position);


        GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction);
        await Task.Delay(3000);

        if(interactionSequenceLine != null)
            OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);

        if(characterTarget != character.NavActor.Target)
        {
            character.CharacterAnimationHandler.SetLocomotion(true, character);
        } else
        {
            character.SetCharacterActionState(CharacterActionState.Idle);
        }

        return;
    }

    public async Task MoveToInteractionLocation(Character character, Vector2 roomObjectLocation, Vector2 characterTarget)
    {
        Logger.Log("HAALALA::::" + ObjectInteraction.ObjectInteractionLocationType);
        if(ObjectInteraction.ObjectInteractionLocationType == ObjectInteractionLocationType.AtRoomObject)
        {
            while (Vector2.Distance(character.transform.position, RoomObjectLocation) > 12)
            {
                await Task.Yield();
                if (roomObjectLocation != RoomObjectLocation)
                    return;
                if (characterTarget != character.NavActor.Target)
                    return;
                if (RoomObject == null)
                    return;
            }
        }
        else
        {
            while (character.CurrentRoom != RoomObject.ParentRoom)
            {
                await Task.Yield();
                if (roomObjectLocation != RoomObjectLocation)
                    return;
                if (characterTarget != character.NavActor.Target)
                    return;
                if (RoomObject == null)
                    return;
                if (RoomObject.ParentRoom == null)
                    return;
            }
        }

        return;
    }
}
