using UnityEngine;

// This is the script that drives the collection of buttons related to the build mode. Such as the different rooms.
public class BuildModeButtons : MonoBehaviour
{
    public static BuildModeButtons Instance;

    public GameObject ExitBuildModeButton;
    public GameObject RoomButton; // later replace with dynamic array
    public GameObject DeleteRoomButton; // later replace with dynamic array

    public GameObject ExitBuildModeButtonPrefab;
    public GameObject RoomButtonPrefab;
    public GameObject DeleeRoomButtonPrefab;

    public void Awake()
    {
        Guard.CheckIsNull(ExitBuildModeButtonPrefab, "ExitBuildModeButtonPrefab");
        Guard.CheckIsNull(RoomButtonPrefab, "RoomButtonPrefab");
        Guard.CheckIsNull(DeleeRoomButtonPrefab, "DeleeRoomButtonPrefab");

        Instance = this;
    }

    public void CreateAllButtons()
    {
        ExitBuildModeButton = Instantiate(ExitBuildModeButtonPrefab, transform);
        RoomButton = Instantiate(RoomButtonPrefab, transform);
        DeleteRoomButton = Instantiate(DeleeRoomButtonPrefab, transform);
    }

    public void DeleteAllButtons()
    {
        if (ExitBuildModeButton != null)
            Destroy(ExitBuildModeButton);
        if (RoomButton != null)
            Destroy(RoomButton); 
        if (DeleteRoomButton != null)
            Destroy(DeleteRoomButton);
    }
}
