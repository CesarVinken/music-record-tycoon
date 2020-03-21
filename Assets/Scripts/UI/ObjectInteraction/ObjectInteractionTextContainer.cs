using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionTextContainer : MonoBehaviour
{
    public GameObject InteractionOptionsContainer;

    public GameObject InteractionOptionPrefab;

    public Text Title;

    public ObjectInteraction[] ObjectInteractionOptions = new ObjectInteraction[] { };
    public RoomObject RoomObject;

    void Awake()
    {
        Guard.CheckIsNull(InteractionOptionsContainer, "InteractionOptionsContainer");

        Guard.CheckIsNull(InteractionOptionPrefab, "InteractionOptionPrefab");

        if (Title == null)
            Logger.Error(Logger.Initialisation, "could not find Title");
    }

    public void Initialise(RoomObject roomObject)
    {
        RoomObject = roomObject;
        ObjectInteractionOptions = roomObject.RoomObjectBlueprint.ObjectInteractions;

        AddRoomObjectName(roomObject.RoomObjectBlueprint.Name);
        for (int i = 0; i < ObjectInteractionOptions.Length; i++)
        {
            AddInteractionOption(ObjectInteractionOptions[i]);
        }
    }

    public void AddInteractionOption(ObjectInteraction objectInteraction)
    {
        GameObject InteractionOptionGO = Instantiate(InteractionOptionPrefab, InteractionOptionsContainer.transform);
        ObjectInteractionOptionButton objectInteractionOptionButton = InteractionOptionGO.GetComponent<ObjectInteractionOptionButton>();
        objectInteractionOptionButton.Initialise(objectInteraction, RoomObject, RoomObject.transform.position);
    }

    public void AddRoomObjectName(string objectName)
    {
        Title.text = objectName;
    }
}
