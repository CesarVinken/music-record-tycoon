using UnityEngine;

public class OptionsMenu : MenuScreen
{
    public GameObject OpenConsoleGO;

    void Awake()
    {
        Guard.CheckIsNull(MainMenuPrefab, "MainMenuPrefab");

        if(GameManager.Instance.CurrentPlatform == Platform.Android && GameManager.Instance != null)
        {
            OpenConsoleGO.SetActive(true);
        } else
        {
            OpenConsoleGO.SetActive(false);
        }
    }
}
