using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectInteractionRunner
{
    public ObjectInteraction ObjectInteraction;
    public RoomObjectGO RoomObject;
    public Character InteractingCharacter = null;
    public ObjectInteractionOptionType OptionType;

    public ObjectInteractionRunner(ObjectInteraction objectInteraction, RoomObjectGO roomObject, Character interactingCharacter)
    {
        if (objectInteraction == null)
        {
            Logger.Error("Object interaction should not be null at this point!");
            return;
        }

        if (roomObject == null)
        {
            Logger.Error("RoomObject should not be null at this point!");
            return;
        }

        ObjectInteraction = objectInteraction;
        RoomObject = roomObject;
        InteractingCharacter = interactingCharacter;
    }

    public async void Run()
    {
        Logger.Log("Run interaction");
        Logger.Log("role {0}", ObjectInteraction.CharacterRole);
        CharacterManager.Instance.DeselectCharacter();

        List<InteractionStep> interactionSteps = ObjectInteraction.InteractionSteps;
        Vector2 roomObjectLocation = RoomObject.RoomObjectInteractionLocation.transform.position;

        for (int i = 0; i < interactionSteps.Count; i++)
        {
            await RunInteractionStep(interactionSteps[i], roomObjectLocation);
        }

        RoomObject.UnsetInteractingCharacter(InteractingCharacter);

        if (roomObjectLocation != InteractingCharacter.PlayerLocomotion.Target && Vector2.Distance(roomObjectLocation, InteractingCharacter.PlayerLocomotion.Target) > 1f)
        {
            InteractingCharacter.CharacterAnimationHandler.SetLocomotion(true);
            InteractingCharacter.SetCharacterActionState(CharacterActionState.Moving);
        }
        else
        {
            InteractingCharacter.SetCharacterActionState(CharacterActionState.Idle);
        }
    }

    public async Task RunInteractionStep(InteractionStep interactionStep, Vector2 roomObjectLocation)
    {
        Logger.Log("Make interaction Transaction for {0}", ObjectInteraction.Name);
        if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.NoCharacter)
        {
            if (interactionStep.HasSequenceLine())
            {
                GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(interactionStep.InteractionSequenceLine, roomObjectLocation);
                interactionStep.InteractionSequenceLineGO = interactionSequenceLine;

            }
            await Task.Delay(interactionStep.Duration);
            interactionStep.CleanUp();
        }
        else
        {
            Vector2 characterTarget = roomObjectLocation;
            InteractingCharacter.PlayerLocomotion.SetLocomotionTarget(characterTarget);
            Logger.Log("characterTarget for {0} is {1},{2}", InteractingCharacter.CharacterName, characterTarget.x, characterTarget.y);

            await MoveToInteractionLocation(InteractingCharacter, characterTarget, ObjectInteraction, RoomObject);
            if (characterTarget != InteractingCharacter.PlayerLocomotion.Target)    // character target was changed on the way
            {
                return;
            }
            RoomObject.SetInteractingCharacter(InteractingCharacter);

            InteractingCharacter.CharacterAnimationHandler.SetLocomotion(false);
            InteractingCharacter.SetCharacterActionState(CharacterActionState.PlayerAction);
            InteractingCharacter.PlayerLocomotion.SetLocomotionTarget(InteractingCharacter.transform.position);

            if (interactionStep.HasSequenceLine())
            {
                GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(interactionStep.InteractionSequenceLine, roomObjectLocation);
                interactionStep.InteractionSequenceLineGO = interactionSequenceLine;

            }
            await Task.Delay(interactionStep.Duration);
            interactionStep.CleanUp();

            //TEMPORARY
            if (ObjectInteraction.ObjectInteractionType == ObjectInteractionType.Record)
            {
                // TODO externalise album record action into own class
                RecordSong();
            }
            /////
        }

        return;
    }

    public static async Task MoveToInteractionLocation(Character character, Vector2 characterTarget, ObjectInteraction objectInteraction, RoomObjectGO roomObject)
    {
        if (objectInteraction.CharacterRole == ObjectInteractionCharacterRole.CharacterAtRoomObject)
        {
            while (Vector2.Distance(character.transform.position, characterTarget) > CharacterManager.MinDistanceForInteraction)
            {
                await Task.Yield();
                if (characterTarget != character.PlayerLocomotion.Target)
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
                if (characterTarget != character.PlayerLocomotion.Target)
                    return;
                if (roomObject == null)
                    return;
                if (roomObject.ParentRoom == null)
                    return;
            }
        }

        return;
    }

    public void RecordSong()
    {
        Song song = new Song("Dancing in the Streets", InteractingCharacter);
        for (int i = 0; i < MusicManager.Instance.Songs.Count; i++)
        {
            Logger.Log("{0}. a song by {1}", MusicManager.Instance.Songs[i].Name, MusicManager.Instance.Songs[i].Composer);
        }
    }
}
