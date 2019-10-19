using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public static MainMenuCanvas Instance;

    //public GameObject PauseMenuPrefab;
    //public GameObject PauseMenu;


    public GameObject MainMenuPrefab;
    public GameObject CurrentMainMenuPanel;
    public void Awake()
    {
        Instance = this;

        if (MainMenuPrefab == null)
            Debug.LogError("Could not find MainMenuPrefab");

    }

    public void FreezeUI()
    {
        //DescriptionDisplay.HideText();
    }

    public void UnfreezeUI()
    {

    }


    public void ToggleMainMenu()
    {
        if (GameManager.MainMenuOpen)
        {
            CloseMainMenu();
            return;
        }
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        FreezeUI();
        AudioListener.pause = true;
        Time.timeScale = 0;
        GameManager.MainMenuOpen = true;
        CurrentMainMenuPanel = Instantiate(MainMenuPrefab, transform);
    }

    public void CloseMainMenu()
    {
        UnfreezeUI();
        GameManager.MainMenuOpen = false;
        Destroy(CurrentMainMenuPanel);
        Time.timeScale = 1;
    }

    public void SetCurrentMenuPanel(GameObject panel)
    {
        CurrentMainMenuPanel = panel;
    }
}
