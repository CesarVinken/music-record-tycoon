using UnityEngine;

public class OnScreenTextContainer : MonoBehaviour
{
    public static OnScreenTextContainer Instance;

    public GameObject ObjectInteractionTextContainer;

    public GameObject ObjectInteractionTextContainerPrefab;
    void Awake()
    {
        Guard.CheckIsNull(ObjectInteractionTextContainerPrefab, "ObjectInteractionTextContainerPrefab");

        Instance = this;
    }

    public void CreateObjectInteractionTextContainer(RoomObject roomObject)
    {
        GameObject objectInteractionTextContainerGO = Instantiate(ObjectInteractionTextContainerPrefab, transform);
        ObjectInteractionTextContainer objectInteractionTextContainer = objectInteractionTextContainerGO.GetComponent<ObjectInteractionTextContainer>();
        objectInteractionTextContainer.Initialise(roomObject);
    }

    public void DeleteObjectInteractionTextContainer()
    {
        Logger.Log("Not yet implemented");
    }
}
