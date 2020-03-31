using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionCharacterOptionsMenu : ObjectInteractionOptionsMenu
{
    public override void Initialise(RoomObject roomObject)
    {
        RoomObject = roomObject;
        List<Character> possibleInteractionCharacters = CharacterManager.Instance.Characters;

        Logger.Log("possibleInteractionCharacters.Count {0}", possibleInteractionCharacters.Count);
        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < possibleInteractionCharacters.Count; i++)
        {
            if (!possibleInteractionCharacters[i].PossibleObjectInteractions.Contains(ObjectInteractionRunner.ObjectInteraction.ObjectInteractionType))
                return;

            GameObject InteractionOptionGO = Instantiate(ObjectInteractionOptionsContainerGO.InteractionOptionPrefab, InteractionOptionsContainer.transform);
            InteractionOptionGO.name = CharacterNameGenerator.GetName(possibleInteractionCharacters[i].Name);

            ObjectInteractionOptionButton objectInteractionOptionButton = CreateInteractionOptionButton(InteractionOptionGO, i, possibleInteractionCharacters.Count);
            objectInteractionOptionButton.SetInteractingCharacter(possibleInteractionCharacters[i]);
            objectInteractionOptionButton.SetInteractionOptionText(CharacterNameGenerator.GetName(possibleInteractionCharacters[i].Name));
        }
    }

    public override ObjectInteractionOptionButton CreateInteractionOptionButton(GameObject parentGO, int index, int optionsLength)
    {
        RectTransform rect = parentGO.GetComponent<RectTransform>();

        Vector2 interactionOptionPosition = GetInteractionOptionPosition(optionsLength, index);
        rect.anchoredPosition = interactionOptionPosition;

        ObjectInteractionOptionType optionType = ObjectInteractionOptionType.InteractionStarter;

        ObjectInteractionOptionButton objectInteractionOptionButton = parentGO.GetComponent<ObjectInteractionOptionButton>();
        objectInteractionOptionButton.Initialise(ObjectInteractionRunner.ObjectInteraction, RoomObject, optionType);

        return objectInteractionOptionButton;
    }
}