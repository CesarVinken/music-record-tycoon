using UnityEngine;

// This is the script that drives the collection of buttons related to the build Tab. Such as the different rooms.
public class BuildTabButtons : MonoBehaviour
{
    public static BuildTabButtons Instance;

    public GameObject ExitBuildTabButton;
    public GameObject RoomButton; // later replace with dynamic array
    public GameObject DeleteRoomButton; // later replace with dynamic array

    public GameObject ExitBuildTabButtonPrefab;
    public GameObject RoomButtonPrefab;
    public GameObject DeleteRoomButtonPrefab;

    public void Awake()
    {
        Guard.CheckIsNull(ExitBuildTabButtonPrefab, "ExitBuildTabButtonPrefab");
        Guard.CheckIsNull(RoomButtonPrefab, "RoomButtonPrefab");
        Guard.CheckIsNull(DeleteRoomButtonPrefab, "DeleeRoomButtonPrefab");

        Instance = this;
    }

    public void CreateAllButtons()
    {
        ExitBuildTabButton = Instantiate(ExitBuildTabButtonPrefab, transform);
        RoomButton = Instantiate(RoomButtonPrefab, transform);
        DeleteRoomButton = Instantiate(DeleteRoomButtonPrefab, transform);
    }

    public void DeleteAllButtons()
    {
        if (ExitBuildTabButton != null)
            Destroy(ExitBuildTabButton);
        if (RoomButton != null)
            Destroy(RoomButton); 
        if (DeleteRoomButton != null)
            Destroy(DeleteRoomButton);
    }
}
