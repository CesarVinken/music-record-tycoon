using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool MainMenuOpen;    //that should block interactivity of level ui elements

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;

    public BuilderManager BuilderManager;

    public void Awake()
    {
        Instance = this;

        MainMenuOpen = false;

        if (Application.isMobilePlatform)
        {
            CurrentPlatform = Platform.Android;
            Configuration = new AndroidConfiguration();
        } else
        {
            CurrentPlatform = Platform.PC;
            Configuration = new PCConfiguration();
        }

        if (BuilderManager == null)
            Debug.LogError("Cannot find BuilderManager");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuCanvas.Instance.ToggleMainMenu();
        }
    }
}
