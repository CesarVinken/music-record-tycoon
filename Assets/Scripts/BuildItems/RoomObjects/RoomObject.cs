using UnityEngine;
using UnityEngine.EventSystems;

public class RoomObject : MonoBehaviour
{
    public RoomObjectBlueprint RoomObjectBlueprint;
    public PolygonCollider2D Collider;
    public ObjectRotation RoomObjectRotation;
    public Vector2 LocationInRoom; // needed to calculate location in rotated room
    public Room Room;


    public void OnMouseDown()
    {
        if (GameManager.Instance.CurrentPlatform == Platform.PC)
        {
        }
    }

    public void ShowInteractionMenu()
    {
        if (RoomObjectBlueprint.ObjectInteractions.Length == 0) return;
        Logger.Log("Show interaction options for objects");
    }

}
