using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionFirstOptionsMenu : ObjectInteractionOptionsMenu
{
    public override void Initialise(RoomObject roomObject)
    {
        RoomObject = roomObject;
        ObjectInteractionOptions = roomObject.RoomObjectBlueprint.ObjectInteractions;

        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < ObjectInteractionOptions.Length; i++)
        {
            GameObject InteractionOptionGO = Instantiate(ObjectInteractionOptionsContainerGO.InteractionOptionPrefab, InteractionOptionsContainer.transform);
            InteractionOptionGO.name = ObjectInteractionOptions[i].Name;


            ObjectInteractionOptionType optionType = ObjectInteractionOptionType.InteractionStarter;
            
            if (ObjectInteractionOptions[i].CharacterRole != ObjectInteractionCharacterRole.NoCharacter && ObjectInteractionRunner.InteractingCharacter == null)
                optionType = ObjectInteractionOptionType.CharacterMenuTrigger;  //This means we first need to select a character before we can run the action, so the button should lead to a character menu

            if(optionType == ObjectInteractionOptionType.CharacterMenuTrigger)
            {
                // only show this interaction if there is at least 1 character that can execute it.
                if (!CharacterManager.Instance.Characters.Any(character => character.PossibleObjectInteractions.Contains(ObjectInteractionOptions[i].ObjectInteractionType)))
                    continue;
            }

            ObjectInteractionOptionButton objectInteractionOptionButton = CreateInteractionOptionButton(InteractionOptionGO, i, ObjectInteractionOptions.Length);
            objectInteractionOptionButton.Initialise(ObjectInteractionOptions[i], RoomObject, optionType);
            objectInteractionOptionButton.SetInteractionOptionText(ObjectInteractionOptions[i].Name);
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
