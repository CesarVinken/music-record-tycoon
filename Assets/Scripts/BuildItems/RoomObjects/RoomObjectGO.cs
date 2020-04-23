using UnityEngine;

public class RoomObjectGO : MonoBehaviour
{
    public RoomObject RoomObject;
    public RoomObjectBlueprint RoomObjectBlueprint;
    public ObjectRotation RoomObjectRotation;
    public Room ParentRoom;

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
