using UnityEngine;

public class OptionsMenu : MenuScreen
{
    void Awake()
    {
        Guard.CheckIsNull(MainMenuPrefab, "MainMenuPrefab");
    }
}
