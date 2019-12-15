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

    //public void CreateButtonsForBuildMenuMode()
    //{
    //    BuildMenuContainer.Instance.CreateAllButtons();
    //}

    //public void CreateButtonsForPlayMode()
    //{
    //    BuildMenuButton = Instantiate(BuildMenuButtonPrefab, LeftRow.transform);
    //}

    //public void DeleteButtonsForBuildMenuMode()
    //{
    //    if (MainMenuButton != null)
    //        Destroy(MainMenuButton);

    //    if (BuildMenuButton != null)
    //        Destroy(BuildMenuButton);  
    //}

    //public void DeleteButtonsForPlayMode()
    //{
        //BuildMenuContainer.Instance.DeleteAllButtons();
    //}
}
