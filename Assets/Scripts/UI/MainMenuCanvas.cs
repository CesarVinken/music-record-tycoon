﻿using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public static MainMenuCanvas Instance;

    public GameObject MainMenuPrefab;
    public GameObject CurrentMainMenuPanel;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(MainMenuPrefab, "MainMenuPrefab");
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

        InGameButtons.Instance.ShowMainMenuButton(false);   
        InGameButtons.Instance.ShowBuildMenuButton(false);
    }

    public void CloseMainMenu()
    {
        UnfreezeUI();
        GameManager.MainMenuOpen = false;
        Destroy(CurrentMainMenuPanel);
        Time.timeScale = 1;

        InGameButtons.Instance.ShowMainMenuButton(true);
        InGameButtons.Instance.ShowBuildMenuButton(true);
    }

    public void SetCurrentMenuPanel(GameObject panel)
    {
        CurrentMainMenuPanel = panel;
    }
}
