using UnityEngine;

public class RoomObjectGO : MonoBehaviour
{
    public RoomObject RoomObject;
    public RoomObjectBlueprint RoomObjectBlueprint;
    public ObjectRotation RoomObjectRotation;
    public Room ParentRoom;
    public Vector2 RoomObjectLocation;

    public void Awake()
    {
        if (transform.childCount == 0) RoomObjectLocation = transform.position;
        else RoomObjectLocation = transform.GetChild(0).transform.position;
    }

    public void OnMouseDown()
    {
        Logger.Log("mouse down");
        ShowInteractionMenu();
    }

    public void Initialise(RoomObjectBlueprint roomObjectBlueprint, ObjectRotation roomObjectRotation, Room parentRoom)
    {
        RoomObjectBlueprint = roomObjectBlueprint;
        RoomObjectRotation = roomObjectRotation;
        ParentRoom = parentRoom;
    }

    public void ShowInteractionMenu()
    {
        if (RoomObjectBlueprint.ObjectInteractions.Length == 0) return;

        OnScreenTextContainer.Instance.CreateObjectInteractionTextContainer(this, ObjectInteractionOptionsMenuType.FirstOptionsMenu);
    }

}
