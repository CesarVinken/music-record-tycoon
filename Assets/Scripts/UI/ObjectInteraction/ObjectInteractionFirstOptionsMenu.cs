using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectInteractionFirstOptionsMenu : ObjectInteractionOptionsMenu
{
    private List<ObjectInteractionOptionButton> _objectInteractionOptionButtons = new List<ObjectInteractionOptionButton>();
    
    public override void Initialise(RoomObjectGO roomObject, ObjectInteraction objectInteraction)
    {
        Logger.Log(Logger.Interaction, "initalise first options menu");
        RoomObject = roomObject;
        ObjectInteractionOptions = roomObject.RoomObjectBlueprint.ObjectInteractions;
        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < ObjectInteractionOptions.Length; i++)
        {
            Logger.Log(Logger.Interaction, "ObjectInteractionOption {0}", ObjectInteractionOptions[i].Name);

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

            GameObject InteractionOptionGO = Instantiate(ObjectInteractionOptionsContainerGO.InteractionOptionPrefab, InteractionOptionsContainer.transform);
            InteractionOptionGO.name = ObjectInteractionOptions[i].Name;

            ObjectInteractionOptionButton objectInteractionOptionButton = CreateInteractionOptionButton(InteractionOptionGO, i, ObjectInteractionOptions.Length);
            _objectInteractionOptionButtons.Add(objectInteractionOptionButton);
            objectInteractionOptionButton.Initialise(ObjectInteractionOptions[i], RoomObject, optionType);
            objectInteractionOptionButton.SetInteractionOptionText(ObjectInteractionOptions[i].Name);
        }

        if(_objectInteractionOptionButtons.Count == 0)
        {
            ShowEmptyOptionsMenu();
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

    private void ShowEmptyOptionsMenu()
    {
        Logger.Warning(Logger.Interaction, "not a single character in the game can currently interact with {0}. We need to set up something for this case", RoomObject.RoomObject.RoomObjectName);

        ObjectInteraction emptyInteraction = ObjectInteraction.Create(ObjectInteractionType.Empty,
            "There is no one in the studio who can interact with the " + RoomObject.RoomObjectBlueprint.Name,
            ObjectInteractionCharacterRole.NoCharacter);

        GameObject InteractionOptionGO = Instantiate(ObjectInteractionOptionsContainerGO.InteractionOptionPrefab, InteractionOptionsContainer.transform);
        InteractionOptionGO.name = emptyInteraction.Name;

        Logger.Log(Logger.Interaction, "Display empty interaction meny");
        ObjectInteractionOptionType optionType = ObjectInteractionOptionType.InteractionStarter;

        ObjectInteractionOptionButton objectInteractionOptionButton = CreateInteractionOptionButton(InteractionOptionGO, 0, ObjectInteractionOptions.Length);
        _objectInteractionOptionButtons.Add(objectInteractionOptionButton);
        objectInteractionOptionButton.Initialise(emptyInteraction, RoomObject, optionType);
        objectInteractionOptionButton.SetInteractionOptionText(emptyInteraction.Name);
    }
}
