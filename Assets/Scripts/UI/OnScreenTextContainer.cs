using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenTextContainer : MonoBehaviour
{
    public static OnScreenTextContainer Instance;

    public GameObject ObjectInteractionTextContainer;

    public GameObject ObjectInteractionTextContainerPrefab;
    public GameObject InteractionSequenceLinePrefab;
    void Awake()
    {
        Guard.CheckIsNull(ObjectInteractionTextContainerPrefab, "ObjectInteractionTextContainerPrefab");
        Guard.CheckIsNull(InteractionSequenceLinePrefab, "InteractionSequenceLinePrefab");

        Instance = this;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ObjectInteractionTextContainer)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                        return;

                    ObjectInteractionOptionButton objectInteractionOptionButton = EventSystem.current.currentSelectedGameObject.GetComponent<ObjectInteractionOptionButton>();
                    if (objectInteractionOptionButton == null)
                    {
                        DeleteObjectInteractionTextContainer();
                    }
                } 
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                DeleteObjectInteractionTextContainer();
            }
        }
    }

    public void CreateObjectInteractionTextContainer(RoomObjectGO roomObject, ObjectInteractionOptionsMenuType optionsMenuType, ObjectInteraction objectInteraction = null)
    {
        Vector2 textPosition = Camera.main.WorldToScreenPoint(roomObject.transform.position);
        GameObject objectInteractionTextContainerGO = GameManager.Instance.InstantiatePrefab(ObjectInteractionTextContainerPrefab, transform, textPosition);
        ObjectInteractionTextContainer = objectInteractionTextContainerGO;

        ObjectInteractionOptionsMenu objectInteractionTextContainer = AddOptionsMenuComponent(optionsMenuType, objectInteractionTextContainerGO);
        objectInteractionTextContainer.Initialise(roomObject, objectInteraction);
    }

    private ObjectInteractionOptionsMenu AddOptionsMenuComponent(ObjectInteractionOptionsMenuType optionsMenuType, GameObject optionsGO) { 
        switch (optionsMenuType)
        {
            case ObjectInteractionOptionsMenuType.FirstOptionsMenu:
                return optionsGO.AddComponent<ObjectInteractionFirstOptionsMenu>();
            case ObjectInteractionOptionsMenuType.CharacterOptionsMenu:
                return optionsGO.AddComponent<ObjectInteractionCharacterOptionsMenu>();
            default:
                Logger.Error("Options Menu Type {0} was not yet defined", optionsMenuType);
                return null;
        }
    }

    //// 
    //public void CreateObjectInteractionTextContainer(RoomObject roomObject, ObjectInteraction objectInteraction)
    //{
    //    Vector2 textPosition = Camera.main.WorldToScreenPoint(roomObject.transform.position);
    //    GameObject objectInteractionTextContainerGO = GameManager.Instance.InstantiatePrefab(ObjectInteractionTextContainerPrefab, transform, textPosition);
    //    ObjectInteractionTextContainer = objectInteractionTextContainerGO;
    //    ObjectInteractionFirstOptionsMenu objectInteractionTextContainer = objectInteractionTextContainerGO.AddComponent<ObjectInteractionFirstOptionsMenu>();
    //    objectInteractionTextContainer.Initialise(roomObject);
    //}

    public void DeleteObjectInteractionTextContainer()
    {
        Destroy(ObjectInteractionTextContainer);
        ObjectInteractionTextContainer = null;
    }

    //public GameObject CreateInteractionSequenceLine(ObjectInteraction objectInteraction, Character interactingCharacter)
    //{
    //    Vector2 sequenceLinePosition = Camera.main.WorldToScreenPoint(interactingCharacter.transform.position);
    //    GameObject interactionSequenceLineGO = GameManager.Instance.InstantiatePrefab(InteractionSequenceLinePrefab, transform, sequenceLinePosition);
    //    InteractionSequenceLine interactionSequenceLine = interactionSequenceLineGO.GetComponent<InteractionSequenceLine>();
    //    interactionSequenceLine.Initialise(objectInteraction.Reaction, interactingCharacter.transform.position, interactingCharacter);

    //    return interactionSequenceLineGO;
    //}

    public GameObject CreateInteractionSequenceLine(InteractionSequenceLine interactionSequenceLine, Vector2 roomObjectLocation)
    {
        Logger.Log("roomObjectLocation {0},{1}", roomObjectLocation.x, roomObjectLocation.y);

        Vector2 sequenceLinePosition = Camera.main.WorldToScreenPoint(roomObjectLocation);
        GameObject lineGO = GameManager.Instance.InstantiatePrefab(InteractionSequenceLinePrefab, transform, sequenceLinePosition);
        InteractionSequenceLineGO interactionSequenceLineGO = lineGO.GetComponent<InteractionSequenceLineGO>();
        interactionSequenceLineGO.Initialise(interactionSequenceLine, roomObjectLocation, null);

        return lineGO;
    }

    public void DeleteInteractionSequenceLine(GameObject InteractionSequenceLineGO)
    {
        Destroy(InteractionSequenceLineGO);
    }
}
