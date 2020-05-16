using UnityEngine;

public class RoomObjectGO : MonoBehaviour
{
    public RoomObject RoomObject;
    public RoomObjectBlueprint RoomObjectBlueprint;
    public ObjectRotation RoomObjectRotation;
    public Room ParentRoom;
    public Character InteractingCharacter; // for either character interactions or routines
    public GameObject RoomObjectInteractionLocation;

    public void Awake()
    {
        Guard.CheckIsNull(RoomObjectInteractionLocation, "RoomObjectInteractionLocation");
    }

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

    public void Register()
    {
        RoomManager.Instance.RegisterRoomObjectGO(this);
    }

    public void Deregister()
    {
        RoomManager.Instance.DeregisterRoomObjectGO(this);
    }

    public void SetInteractingCharacter(Character character)
    {
        Logger.Log(Logger.Interaction, "Register {0} at {1}", character.FullName(), RoomObjectBlueprint.RoomObjectName);
        InteractingCharacter = character;
    }

    public  void UnsetInteractingCharacter(Character character = null)
    {
        if(InteractingCharacter == character || character == null)
        {
            Logger.Log(Logger.Interaction, "Deregister {0} from {1}", character.FullName(), RoomObjectBlueprint.RoomObjectName);
            InteractingCharacter = null;
        }
    }
}
