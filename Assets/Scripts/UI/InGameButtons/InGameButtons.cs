using UnityEngine;

public class InGameButtons : MonoBehaviour
{
    public static InGameButtons Instance;

    // Button containers
    public GameObject RightRow;
    public GameObject BuildTabButtonsGO;

    // Buttons
    public GameObject MainMenuButton;
    public GameObject BuildTabButton;

    // Prefabs
    public GameObject BuildTabButtonPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(RightRow, "RightRow");
        Guard.CheckIsNull(BuildTabButtonsGO, "BuildTabButtonsGO");

        Guard.CheckIsNull(BuildTabButtonPrefab, "BuildTabButtonPrefab");
    }

    public void CreateButtonsForBuildTabMode()
    {
        BuildTabButtons.Instance.CreateAllButtons();
    }

    public void CreateButtonsForPlayMode()
    {
        BuildTabButton = Instantiate(BuildTabButtonPrefab, RightRow.transform);
    }

    public void DeleteButtonsForBuildTabMode()
    {
        if (BuildTabButton != null)
            Destroy(BuildTabButton);  
    }

    public void DeleteButtonsForPlayMode()
    {
        BuildTabButtons.Instance.DeleteAllButtons();
    }
}
