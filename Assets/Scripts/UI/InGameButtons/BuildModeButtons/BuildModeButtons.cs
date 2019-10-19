using UnityEngine;

// This is the script that drives the collection of buttons related to the build mode. Such as the different rooms.
public class BuildModeButtons : MonoBehaviour
{
    public static BuildModeButtons Instance;

    public GameObject ExitBuildModeButton;
    public GameObject RoomButton; // later replace with dynamic array

    public GameObject ExitBuildModeButtonPrefab;
    public GameObject RoomButtonPrefab;

    public void Awake()
    {
        Guard.CheckIsNull(ExitBuildModeButtonPrefab, "ExitBuildModeButtonPrefab");
        Guard.CheckIsNull(RoomButtonPrefab, "RoomButtonPrefab");

        Instance = this;
    }

    public void CreateAllButtons()
    {
        ExitBuildModeButton = Instantiate(ExitBuildModeButtonPrefab, transform);
        RoomButton = Instantiate(RoomButtonPrefab, transform);
    }

    public void DeleteAllButtons()
    {
        if (ExitBuildModeButton != null)
            Destroy(ExitBuildModeButton);
        if (RoomButton != null)
            Destroy(RoomButton);
    }
}
