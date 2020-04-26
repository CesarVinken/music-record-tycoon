using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionCharacterOptionsMenu : ObjectInteractionOptionsMenu
{
    private ObjectInteraction _objectInteraction;
    public override void Initialise(RoomObjectGO roomObject, ObjectInteraction objectInteraction)
    {
        Logger.Log("initalise characters options menu");

        _objectInteraction = objectInteraction;
        RoomObject = roomObject;
        List<Character> possibleInteractionCharacters = CharacterManager.Instance.Characters;

        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < possibleInteractionCharacters.Count; i++)
        {
            if (!possibleInteractionCharacters[i].PossibleObjectInteractions.Contains(_objectInteraction.ObjectInteractionType))
            {
                Logger.Log("{0} can not be done by {1}", _objectInteraction.ObjectInteractionType, CharacterNameGenerator.GetName(possibleInteractionCharacters[i].Name));
                continue;
            }

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
        objectInteractionOptionButton.Initialise(_objectInteraction, RoomObject, optionType);

        return objectInteractionOptionButton;
    }
}