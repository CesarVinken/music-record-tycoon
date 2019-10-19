using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public static bool InBuildMode;
    public static bool HasRoomSelected;

    public GameObject Room1Prefab;  // Should become generic later on.
    public GameObject SelectedRoom;

    void Awake()
    {
        InBuildMode = false;
        HasRoomSelected = false;

        SelectedRoom = null;

        Guard.CheckIsNull(Room1Prefab, "Room1Prefab");
    }

    void Update()
    {
        if (HasRoomSelected && !GameManager.MainMenuOpen)
        {
            if (Input.GetMouseButtonDown(1)) {
                UnsetSelectedRoom();
            }
        }
    }

    public void EnterBuildMode()
    {
        InBuildMode = true;
        InGameButtons.Instance.CreateButtonsForBuildMode();
        InGameButtons.Instance.DeleteButtonsForBuildMode();
    }

    public void ExitBuildMode()
    {
        InBuildMode = false;
        HasRoomSelected = false;
        SelectedRoom = null;

        InGameButtons.Instance.CreateButtonsForPlayMode();
        InGameButtons.Instance.DeleteButtonsForPlayMode();
    }

    public void SetSelectedRoom(GameObject room)
    {
        // Should maybe become Room type? For example: SelectedRoom = room.Prefab
        SelectedRoom = room;

        HasRoomSelected = true;
    }

    public void UnsetSelectedRoom()
    {
        SelectedRoom = null;
        HasRoomSelected = false;
        Debug.Log("no room selected.");
    }
}
