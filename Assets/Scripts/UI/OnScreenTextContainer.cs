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

    public void CreateObjectInteractionTextContainer(RoomObject roomObject)
    {
        GameObject objectInteractionTextContainerGO = Instantiate(ObjectInteractionTextContainerPrefab, transform);
        ObjectInteractionTextContainer = objectInteractionTextContainerGO;
        ObjectInteractionTextContainer objectInteractionTextContainer = objectInteractionTextContainerGO.GetComponent<ObjectInteractionTextContainer>();
        objectInteractionTextContainer.Initialise(roomObject);
    }

    public void DeleteObjectInteractionTextContainer()
    {
        Destroy(ObjectInteractionTextContainer);
        ObjectInteractionTextContainer = null;
    }

    public GameObject CreateInteractionSequenceLine(ObjectInteraction objectInteraction)
    {
        GameObject interactionSequenceLineGO = Instantiate(InteractionSequenceLinePrefab, transform);
        InteractionSequenceLine interactionSequenceLine = interactionSequenceLineGO.GetComponent<InteractionSequenceLine>();
        interactionSequenceLine.Initialise(objectInteraction.Reaction);

        return interactionSequenceLineGO;
    }

    public void DeleteInteractionSequenceLine(GameObject InteractionSequenceLineGO)
    {
        Destroy(InteractionSequenceLineGO);
    }
}
