﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool MainMenuOpen;    //that should block interactivity of level ui elements

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;
    public Grid PathfindingGrid;    // might move to different location

    public BuilderManager BuilderManager;
    public RoomManager RoomManager;
    public GridLayout WorldGrid;

    public void Awake()
    {
        InitialiseLoggers();

        //Logger.Log(Logger.Locomotion, "This is an example {0} and argument 2 is {1}", 54, "ss");
        //Logger.Warning(Logger.Locomotion, "This is an example {0} and argument 2 is {1}", 54, "ss");
        //Logger.Error(Logger.Locomotion, "This is an example {0} and argument 2 is {1}", 54, "ss");
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
            Logger.Error(Logger.Initialisation, "Cannot find BuilderManager");
        if (RoomManager == null)
            Logger.Error(Logger.Initialisation, "Cannot find RoomManager");
        if (PathfindingGrid == null)
            Logger.Error(Logger.Initialisation, "Cannot find PathfindingGrid");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuCanvas.Instance.ToggleMainMenu();
        }
    }

    //Central function to turn on/off different gizmos
    public void OnDrawGizmos()
    {
        //PathfindingGrid.DrawPathfindingGridGizmos();

        if (PlayerCharacter.Instance != null)
            PlayerCharacter.Instance.PlayerNav.DrawPathGizmo();

        if (BuilderManager.Instance != null)
        {
            BuilderManager.Instance.DrawBuildingTilesGizmos();

            BuilderManager.Instance.DrawDoorLocationGizmos();
        }
    }

    public void InitialiseLoggers()
    {
        Logger.General.enableLogs = true;
        Logger.Locomotion.enableLogs = true;
        Logger.Building.enableLogs = true;
        Logger.Pathfinding.enableLogs = true;
        Logger.Initialisation.enableLogs = true;
    }
}
