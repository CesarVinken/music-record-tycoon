using System.Threading.Tasks;
using UnityEngine;

public static class ObjectInteractionRunner
{
    public static ObjectInteraction ObjectInteraction;
    public static Vector2 RoomObjectLocation;
    public static RoomObject RoomObject;
    public static Character InteractingCharacter = null;
    public static ObjectInteractionOptionType OptionType;

    public static async void Run()
    {
        if (ObjectInteraction == null)
        {
            Logger.Error("Object interaction should not be null at this point!");
            return;
        }

        if (RoomObject == null)
        {
            Logger.Error("RoomObject should not be null at this point!");
            return;
        }

        if (RoomObjectLocation == null)
        {
            Logger.Error("RoomObjectLocation should not be null at this point!");
            return;
        }

        // run
        if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.NoCharacter)
        {
            CharacterManager.Instance.DeselectCharacter();

            InteractingCharacter = null;
            RoomObject = null;
            ObjectInteraction objectInteraction = ObjectInteraction;
            ObjectInteraction = null;

            GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(objectInteraction, RoomObjectLocation);
            await Task.Delay(3000);

            if (interactionSequenceLine != null)
                OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);
        }
        else
        {
            CharacterManager.Instance.DeselectCharacter();

            Character interactingCharacter = InteractingCharacter;
            InteractingCharacter = null;
            RoomObject roomObject = RoomObject;
            RoomObject = null;
            ObjectInteraction objectInteraction = ObjectInteraction;
            ObjectInteraction = null;

            Vector2 roomObjectLocation = RoomObjectLocation;
            interactingCharacter.PlayerLocomotion.SetLocomotionTarget(roomObjectLocation);
            Vector2 characterTarget = interactingCharacter.NavActor.Target;

            await MoveToInteractionLocation(interactingCharacter, roomObjectLocation, characterTarget, objectInteraction, roomObject);

            interactingCharacter.CharacterAnimationHandler.SetLocomotion(false);
            interactingCharacter.SetCharacterActionState(CharacterActionState.Action);
            interactingCharacter.PlayerLocomotion.SetLocomotionTarget(interactingCharacter.transform.position);

            GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(objectInteraction, interactingCharacter);
            await Task.Delay(3000);

            if (interactionSequenceLine != null)
                OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);

            if (characterTarget != interactingCharacter.NavActor.Target)
            {
                interactingCharacter.CharacterAnimationHandler.SetLocomotion(true, interactingCharacter);
            }
            else
            {
                interactingCharacter.SetCharacterActionState(CharacterActionState.Idle);
            }
        }

        return;
    }

    public static async Task MoveToInteractionLocation(Character character, Vector2 roomObjectLocation, Vector2 characterTarget, ObjectInteraction objectInteraction, RoomObject roomObject)
    {
        if (objectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterAtRoomObject)
        {
            while (Vector2.Distance(character.transform.position, roomObjectLocation) > 12)
            {
                await Task.Yield();
                if (characterTarget != character.NavActor.Target)
                    return;
                if (roomObject == null)
                    return;
            }
        }
        else if (objectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterInRoom)
        {
            while (character.CurrentRoom != roomObject.ParentRoom)
            {
                await Task.Yield();
                if (characterTarget != character.NavActor.Target)
                    return;
                if (roomObject == null)
                    return;
                if (roomObject.ParentRoom == null)
                    return;
            }
        }

        return;
    }
}
