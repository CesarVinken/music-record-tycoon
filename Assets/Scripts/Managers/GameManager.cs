using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool MainMenuOpen;    //that should block interactivity of level ui elements

    public void Awake()
    {
        Instance = this;

        GameManager.MainMenuOpen = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuCanvas.Instance.ToggleMainMenu();
        }
    }
}
