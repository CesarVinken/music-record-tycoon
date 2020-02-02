using UnityEngine;

public class InGameButtons : MonoBehaviour
{
    public static InGameButtons Instance;

    // Button containers
    public GameObject LeftRow;
    public GameObject BuildMenuGO;

    // Buttons
    public GameObject MainMenuButton;
    public GameObject BuildMenuButton;

    // Prefabs
    public GameObject MainMenuButtonPrefab;
    public GameObject BuildMenuButtonPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(LeftRow, "LeftRow");
        Guard.CheckIsNull(BuildMenuGO, "BuildMenuBGO");

        Guard.CheckIsNull(MainMenuButtonPrefab, "MainMenuButtonPrefab");
        Guard.CheckIsNull(BuildMenuButtonPrefab, "BuildMenuButtonPrefab");
    }

    public void ShowMainMenuButton(bool show)
    {
        MainMenuButton.SetActive(show);
    }

    public void ShowBuildMenuButton(bool show)
    {
        BuildMenuButton.SetActive(show);
    }
}
