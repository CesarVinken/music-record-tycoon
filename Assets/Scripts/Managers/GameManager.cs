using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool MainMenuOpen;    //that should block interactivity of level ui elements

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;

    public void Awake()
    {
        Instance = this;

        GameManager.MainMenuOpen = false;

        if (Application.isMobilePlatform)
        {
            CurrentPlatform = Platform.Android;
            Configuration = new AndroidConfiguration();
        } else
        {
            CurrentPlatform = Platform.PC;
            Configuration = new PCConfiguration();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuCanvas.Instance.ToggleMainMenu();
        }
    }
}
