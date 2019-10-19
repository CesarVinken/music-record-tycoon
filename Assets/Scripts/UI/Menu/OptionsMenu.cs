using UnityEngine;

public class OptionsMenu : MenuScreen
{
    void Awake()
    {
        if (MainMenuPrefab == null)
            Debug.LogError("Canot find MainMenuPrefab");
    }
}
