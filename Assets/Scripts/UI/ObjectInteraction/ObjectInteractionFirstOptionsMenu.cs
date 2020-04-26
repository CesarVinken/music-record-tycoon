using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionFirstOptionsMenu : ObjectInteractionOptionsMenu
{
    public override void Initialise(RoomObjectGO roomObject, ObjectInteraction objectInteraction)
    {
        Logger.Log("initalise first options menu");
        RoomObject = roomObject;
        ObjectInteractionOptions = roomObject.RoomObjectBlueprint.ObjectInteractions;
        List<ObjectInteractionOptionButton> objectInteractionOptionButtons = new List<ObjectInteractionOptionButton>();
        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < ObjectInteractionOptions.Length; i++)
        {
            GameObject InteractionOptionGO = Instantiate(ObjectInteractionOptionsContainerGO.InteractionOptionPrefab, InteractionOptionsContainer.transform);
            InteractionOptionGO.name = ObjectInteractionOptions[i].Name;

            Logger.Log("ObjectInteractionOption {0}", ObjectInteractionOptions[i].Name);

            ObjectInteractionOptionType optionType = ObjectInteractionOptionType.InteractionStarter;

            //if (ObjectInteractionOptions[i].CharacterRole != ObjectInteractionCharacterRole.NoCharacter && ObjectInteractionRunner.InteractingCharacter == null)
            if (ObjectInteractionOptions[i].CharacterRole != ObjectInteractionCharacterRole.NoCharacter)
                    optionType = ObjectInteractionOptionType.CharacterMenuTrigger;  //This means we first need to select a character before we can run the action, so the button should lead to a character menu

            if(optionType == ObjectInteractionOptionType.CharacterMenuTrigger)
            {
                // only show this interaction if there is at least 1 character that can execute it.
                if (!CharacterManager.Instance.Characters.Any(character => character.PossibleObjectInteractions.Contains(ObjectInteractionOptions[i].ObjectInteractionType)))
                    continue;
            }

            ObjectInteractionOptionButton objectInteractionOptionButton = CreateInteractionOptionButton(InteractionOptionGO, i, ObjectInteractionOptions.Length);
            objectInteractionOptionButtons.Add(objectInteractionOptionButton);
            objectInteractionOptionButton.Initialise(ObjectInteractionOptions[i], RoomObject, optionType);
            objectInteractionOptionButton.SetInteractionOptionText(ObjectInteractionOptions[i].Name);
        }

        if(objectInteractionOptionButtons.Count == 0)
        {
            Logger.Warning("not a single character in the game can currently so {0}. We need to set up something for this case", roomObject.RoomObject.RoomObjectName);
        }
    }

    public override ObjectInteractionOptionButton CreateInteractionOptionButton(GameObject parentGO, int index, int optionsLength)
    {
        RectTransform rect = parentGO.GetComponent<RectTransform>();

        Vector2 interactionOptionPosition = GetInteractionOptionPosition(optionsLength, index);
        rect.anchoredPosition = interactionOptionPosition;

        ObjectInteractionOptionButton objectInteractionOptionButton = parentGO.GetComponent<ObjectInteractionOptionButton>();
        return objectInteractionOptionButton;
    }
}
