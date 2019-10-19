using UnityEngine;

public class InGameButtons : MonoBehaviour
{
    public static InGameButtons Instance;

    // Button containers
    public GameObject RightRow;
    public GameObject BuildModeButtonsGO;

    // Buttons
    public GameObject MainMenuButton;
    public GameObject BuildModeButton;

    // Prefabs
    public GameObject BuildModeButtonPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(RightRow, "RightRow");
        Guard.CheckIsNull(BuildModeButtonsGO, "BuildModeButtonsGO");

        Guard.CheckIsNull(BuildModeButtonPrefab, "BuildModeButtonPrefab");
    }

    public void CreateButtonsForBuildMode()
    {
        BuildModeButtons.Instance.CreateAllButtons();
    }

    public void CreateButtonsForPlayMode()
    {
        BuildModeButton = Instantiate(BuildModeButtonPrefab, RightRow.transform);
    }

    public void DeleteButtonsForBuildMode()
    {
        if (BuildModeButton != null)
            Destroy(BuildModeButton);  
    }

    public void DeleteButtonsForPlayMode()
    {
        BuildModeButtons.Instance.DeleteAllButtons();
    }
}
