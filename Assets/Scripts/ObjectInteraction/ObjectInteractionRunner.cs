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

        List<string> interactionSteps = ObjectInteraction.InteractionSteps;
        interactionSteps = new List<string>();
        interactionSteps.Add("temporary");
        for (int i = 0; i < interactionSteps.Count; i++)
        {
            await MakeInteractionTransaction(interactionSteps[i]);
        }

        
    }

    public async Task MakeInteractionTransaction(string interactionStep)
    {
        Vector2 roomObjectLocation = RoomObject.RoomObjectLocation;

        Logger.Log("Make interaction Transaction for {0}", ObjectInteraction.Name);
        if (ObjectInteraction.CharacterRole == ObjectInteractionCharacterRole.NoCharacter)
        {
            GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction, roomObjectLocation);
            await Task.Delay(3000);

            if (interactionSequenceLine != null)
                OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);
        }
        else
        {
            InteractingCharacter.PlayerLocomotion.SetLocomotionTarget(roomObjectLocation);
            Vector2 characterTarget = InteractingCharacter.NavActor.Target;
            Logger.Log("characterTarget for {0} is {1},{2}", InteractingCharacter.Name, characterTarget.x, characterTarget.y);

            await MoveToInteractionLocation(InteractingCharacter, roomObjectLocation, characterTarget, ObjectInteraction, RoomObject);

            InteractingCharacter.CharacterAnimationHandler.SetLocomotion(false);
            InteractingCharacter.SetCharacterActionState(CharacterActionState.Action);
            InteractingCharacter.PlayerLocomotion.SetLocomotionTarget(InteractingCharacter.transform.position);

            GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction, roomObjectLocation);
            await Task.Delay(3000);

            if (interactionSequenceLine != null)
                OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);

            //TEMPORARY
            if(ObjectInteraction.ObjectInteractionType == ObjectInteractionType.Record )
            {
                // TODO externalise album record action into own class
                RecordSong();
            }
            /////

            if (characterTarget != InteractingCharacter.NavActor.Target)
            {
                InteractingCharacter.CharacterAnimationHandler.SetLocomotion(true, InteractingCharacter);
            }
            else
            {
                InteractingCharacter.SetCharacterActionState(CharacterActionState.Idle);
            }
        }

        return;
    }

    public static async Task MoveToInteractionLocation(Character character, Vector2 roomObjectLocation, Vector2 characterTarget, ObjectInteraction objectInteraction, RoomObjectGO roomObject)
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

    public void RecordSong()
    {
        Song song = new Song("Dancing in the Streets", InteractingCharacter);
        for (int i = 0; i < MusicManager.Instance.Songs.Count; i++)
        {
            Logger.Log("{0}. a song by {1}", MusicManager.Instance.Songs[i].Name, MusicManager.Instance.Songs[i].Composer);
        }
    }
}
