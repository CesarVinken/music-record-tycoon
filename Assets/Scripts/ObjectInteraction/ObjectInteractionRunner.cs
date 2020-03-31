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
        if(ObjectInteraction == null)
        {
            Logger.Error("Object interaction should not be null at this point!");
            return;
        }

        // run
        if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.NoCharacter)
        {
            CharacterManager.Instance.DeselectCharacter();
            GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction, RoomObjectLocation);
            await Task.Delay(3000);

            if (interactionSequenceLine != null)
                OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);
        }
        else
        {
            CharacterManager.Instance.DeselectCharacter();

            Character character = InteractingCharacter;

            Vector2 roomObjectLocation = RoomObjectLocation;
            character.PlayerLocomotion.SetLocomotionTarget(RoomObjectLocation);
            Vector2 characterTarget = character.NavActor.Target;

            await MoveToInteractionLocation(character, roomObjectLocation, characterTarget);

            character.CharacterAnimationHandler.SetLocomotion(false);
            character.SetCharacterActionState(CharacterActionState.Action);
            character.PlayerLocomotion.SetLocomotionTarget(character.transform.position);

            GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction, InteractingCharacter);
            await Task.Delay(3000);

            if (interactionSequenceLine != null)
                OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);

            if (characterTarget != character.NavActor.Target)
            {
                character.CharacterAnimationHandler.SetLocomotion(true, character);
            }
            else
            {
                character.SetCharacterActionState(CharacterActionState.Idle);
            }
        }

        ObjectInteraction = null;
        InteractingCharacter = null;
        return;
    }

    public static async Task MoveToInteractionLocation(Character character, Vector2 roomObjectLocation, Vector2 characterTarget)
    {
        if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterAtRoomObject)
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
        else if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterInRoom)
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
