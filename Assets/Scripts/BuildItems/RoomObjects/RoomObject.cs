using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public RoomObjectBlueprint RoomObjectBlueprint;
    //public PolygonCollider2D Collider;
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

        Logger.Log("Show interaction options for objects");
        //ObjectInteractionCollection objectInteractionCollection = new ObjectInteractionCollection(this)
        OnScreenTextContainer.Instance.CreateObjectInteractionTextContainer(this);
    }

}
