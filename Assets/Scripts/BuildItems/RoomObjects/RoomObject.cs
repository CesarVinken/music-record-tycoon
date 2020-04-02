using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public RoomObjectBlueprint RoomObjectBlueprint;
    public ObjectRotation RoomObjectRotation;
    public Room ParentRoom;

    public void OnMouseDown()
    {
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
