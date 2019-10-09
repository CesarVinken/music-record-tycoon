using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MenuScreen
{
    public GameObject SavePanelPrefab;
    public GameObject LoadPanelPrefab;
    public GameObject OptionsPanelPrefab;

    public GameObject NewGameBtn;
    public GameObject ToSavePanel;
    public GameObject ResumeGameBtn;
    public GameObject QuitGameBtn;

    public void Awake()
    {
        //if(MainMenuCanvas.Instance != null)
        //    MainMenuCanvas.Instance.PauseMenu = gameObject;
    }

    public void Start()
    {
        // If there is a game manager, then we already started a game
        if(GameManager.Instance == null)
        {
            ToSavePanel.SetActive(false);
            ResumeGameBtn.SetActive(false);
        }
        else
        {
            NewGameBtn.SetActive(false);      
        }

        if(GameManager.Instance.CurrentPlatform == Platform.Android)
        {
            QuitGameBtn.SetActive(false);
        }
    }

    public void NewGame()
    {
        // Will load Sample Scene with default beginning setup
        SceneManager.LoadScene("SampleScene");
    }

    public void ToSave()
    {
        InstantiatePanel(SavePanelPrefab);
    }

    public void ToLoad()
    {
        InstantiatePanel(LoadPanelPrefab);
    }

    public void ToOptions()
    {
        InstantiatePanel(OptionsPanelPrefab);
    }

    public void ResumeGame()
    {
        if (MainMenuCanvas.Instance == null)
            Debug.LogError("Cannot find MainMenuCanvas");

        MainMenuCanvas.Instance.CloseMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
