using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectInteractionOptionsMenu : MonoBehaviour
{
    public GameObject InteractionOptionsContainer;

    public ObjectInteraction[] ObjectInteractionOptions = new ObjectInteraction[] { };
    public ObjectInteractionOptionsContainerGO ObjectInteractionOptionsContainerGO;
    public RoomObject RoomObject;
    public Character InteractingCharacter = null;

    void Awake()
    {
        ObjectInteractionOptionsContainerGO = transform.GetComponentInChildren<ObjectInteractionOptionsContainerGO>();

        Guard.CheckIsNull(ObjectInteractionOptionsContainerGO, "ObjectInteractionOptionsContainerGO");
        
        InteractionOptionsContainer = ObjectInteractionOptionsContainerGO.gameObject;

        Guard.CheckIsNull(InteractionOptionsContainer, "InteractionOptionsContainer");
    }

    public void Update()
    {
        Vector2 textPosition = Camera.main.WorldToScreenPoint(RoomObject.transform.position);
        transform.position = textPosition;
    }

    public abstract void Initialise(RoomObject roomObject);
    public abstract ObjectInteractionOptionButton CreateInteractionOptionButton(GameObject parent, int index, int optionsLength);

    public void Destroy()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    //public void CreateInteractionOptionButton(ObjectInteraction objectInteraction, int index, int optionsLength)
    //{
    //    GameObject InteractionOptionGO = Instantiate(InteractionOptionPrefab, InteractionOptionsContainer.transform);
    //    RectTransform rect = InteractionOptionGO.GetComponent<RectTransform>();

    //    Vector2 interactionOptionPosition = GetInteractionOptionPosition(optionsLength, index);
    //    rect.anchoredPosition = interactionOptionPosition;

    //    ObjectInteractionOptionType optionType = ObjectInteractionOptionType.InteractionStarter;

    //    if (objectInteraction.CharacterRole != ObjectInteractionCharacterRole.NoCharacter && ObjectInteractionRunner.InteractingCharacter == null)
    //        optionType = ObjectInteractionOptionType.CharacterMenuTrigger;  //This means we first need to select a character before we can run the action, so the button should lead to a character menu

    //    ObjectInteractionOptionButton objectInteractionOptionButton = InteractionOptionGO.GetComponent<ObjectInteractionOptionButton>();
    //    objectInteractionOptionButton.Initialise(objectInteraction, RoomObject, optionType);
    //}

    public void AddRoomObjectName(string objectName)
    {
        ObjectInteractionOptionsContainerGO.Title.text = objectName;
    }

    public Vector2 GetInteractionOptionPosition(int optionsLength, int index)
    {
        switch (optionsLength)
        {
            case 1:
                switch (index)
                {
                    case 0:
                        return new Vector2(0, -60);
                    default:
                        Logger.Error("No interaction option position set up for ObjectInteractionOptions with length {0}, index {1}", ObjectInteractionOptions.Length, index);
                        return new Vector2(0, 0);
                }
            case 2:
                switch (index)
                {
                    case 0:
                        return new Vector2(-50, 40);
                    case 1:
                        return new Vector2(50, -40);
                    default:
                        Logger.Error("No interaction option position set up for ObjectInteractionOptions with length {0}, index {1}", ObjectInteractionOptions.Length, index);
                        return new Vector2(0, 0);
                }
            case 3:
                switch (index)
                {
                    case 0:
                        return new Vector2(-25, 60);
                    case 1:
                        return new Vector2(50, -55);
                    case 2:
                        return new Vector2(100, -15);
                    default:
                        Logger.Error("No interaction option position set up for ObjectInteractionOptions with length {0}, index {1}", ObjectInteractionOptions.Length, index);
                        return new Vector2(0, 0);
                }
            case 4:
                switch (index)
                {
                    case 0:
                        return new Vector2(-50, 65);
                    case 1:
                        return new Vector2(100, 35);
                    case 2:
                        return new Vector2(50, -65);
                    case 3:
                        return new Vector2(-100, -30);
                    default:
                        Logger.Error("No interaction option position set up for ObjectInteractionOptions with length {0}, index {1}", ObjectInteractionOptions.Length, index);
                        return new Vector2(0, 0);
                }
            default:
                Logger.Error("No interaction option position set up for ObjectInteractionOptions with length {0}, index {1}", ObjectInteractionOptions.Length, index);
                return new Vector2(0, 0);
        }
    }
}
