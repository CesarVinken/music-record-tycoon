using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionTextContainer : MonoBehaviour
{
    public GameObject InteractionOptionsContainer;

    public GameObject InteractionOptionPrefab;

    public Text Title;

    public ObjectInteraction[] ObjectInteractionOptions = new ObjectInteraction[] { };
    public RoomObject RoomObject;
    public Character InteractingCharacter;

    void Awake()
    {
        Guard.CheckIsNull(InteractionOptionsContainer, "InteractionOptionsContainer");

        Guard.CheckIsNull(InteractionOptionPrefab, "InteractionOptionPrefab");

        if (Title == null)
            Logger.Error(Logger.Initialisation, "could not find Title");
    }

    public void Update()
    {
        Vector2 textPosition = Camera.main.WorldToScreenPoint(RoomObject.transform.position);
        transform.position = textPosition;
    }

    public void Initialise(RoomObject roomObject)
    {
        RoomObject = roomObject;
        ObjectInteractionOptions = roomObject.RoomObjectBlueprint.ObjectInteractions;
        InteractingCharacter = CharacterManager.Instance.SelectedCharacter;

        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < ObjectInteractionOptions.Length; i++)
        {
            AddInteractionOption(ObjectInteractionOptions[i], i);
        }
    }

    public void AddInteractionOption(ObjectInteraction objectInteraction, int index)
    {
        GameObject InteractionOptionGO = Instantiate(InteractionOptionPrefab, InteractionOptionsContainer.transform);
        RectTransform rect = InteractionOptionGO.GetComponent<RectTransform>();

        Vector2 interactionOptionPosition = GetInteractionOptionPosition(index);
        rect.anchoredPosition = interactionOptionPosition;

        ObjectInteractionOptionButton objectInteractionOptionButton = InteractionOptionGO.GetComponent<ObjectInteractionOptionButton>();
        objectInteractionOptionButton.Initialise(objectInteraction, RoomObject, RoomObject.transform.position, InteractingCharacter);
    }

    public void AddRoomObjectName(string objectName)
    {
        Title.text = objectName;
    }

    public Vector2 GetInteractionOptionPosition(int index)
    {
        switch (ObjectInteractionOptions.Length)
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
