using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool GamePaused { get { return MainMenuOpen; } }
    public static bool MainMenuOpen { 
        get { return _mainMenuOpen; } 
        set {
            _mainMenuOpen = value;
        } 
    }    //that should block interactivity of level ui elements

    public static bool DrawBuildingTilesGizmos;
    public static bool DrawDoorLocationGizmos;

    public static bool DrawGridVectorCoordinates
    {
        get { return _drawGridVectorCoordinates; }
        set
        {
            if (DrawGridCoordinates)
                DrawGridCoordinates = false;

            _drawGridVectorCoordinates = value;
        }
    }

    public static bool DrawGridCoordinates
    {
        get { return _drawGridCoordinates; }
        set
        {
            if (DrawGridVectorCoordinates)
                DrawGridVectorCoordinates = false;

            _drawGridCoordinates = value;
        }
    }

    public static bool DrawPathfindingCells
    {
        get { return _drawPathfindingCells; }
        set
        {
            _drawPathfindingCells = value;
            CharacterManager.Instance.DrawPathfindingCells(value);
        }
    }


    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;
    public AstarPath AstarPath;    // might move to different location

    public BuilderManager BuilderManager;
    public RoomManager RoomManager;
    public GridLayout WorldGrid;

    public GameObject AstarGO;

    private static bool _mainMenuOpen;
    private static bool _drawGridCoordinates;
    private static bool _drawGridVectorCoordinates;
    private static bool _drawPathfindingCells;

    public void Awake()
    {
        InitialiseLoggers();

        Instance = this;

        MainMenuOpen = false;

        if (Application.isMobilePlatform)
        {
            CurrentPlatform = Platform.Android;
            Configuration = new AndroidConfiguration();
        }
        else
        {
            CurrentPlatform = Platform.PC;
            Configuration = new PCConfiguration();
        }

        Guard.CheckIsNull(BuilderManager, "BuilderManager");
        Guard.CheckIsNull(BuilderManager, "RoomManager");
        Guard.CheckIsNull(BuilderManager, "PathfindingGrid");
        Guard.CheckIsNull(AstarGO, "AstarGO");

        DrawBuildingTilesGizmos = false;
        DrawDoorLocationGizmos = false;
        DrawGridVectorCoordinates = false;
        DrawGridCoordinates = true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuCanvas.Instance.ToggleMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ConsoleContainer.Instance.ToggleConsole();
        }
    }

    //Central function to turn on/off different gizmos
    public void OnDrawGizmos()
    {
        if (BuilderManager.Instance != null)
        {
            if (DrawBuildingTilesGizmos)
                BuilderManager.Instance.DrawBuildingTilesGizmos();

            if (DrawDoorLocationGizmos)
                BuilderManager.Instance.DrawDoorLocationGizmos();

            if (DrawGridCoordinates)
                BuilderManager.Instance.DrawGridCoordinates();
        }
    }

    public void InitialiseLoggers()
    {
        Logger.General.enableLogs = true;
        Logger.Time.enableLogs = false;
        Logger.Locomotion.enableLogs = false;
        Logger.Building.enableLogs = false;
        Logger.Pathfinding.enableLogs = false;
        Logger.Initialisation.enableLogs = false;
        Logger.Character.enableLogs = true;
        Logger.Interaction.enableLogs = false;
        Logger.UI.enableLogs = false;
    }

    public GameObject InstantiatePrefab(GameObject prefab, Transform parent, Vector2 position)
    {
        return Instantiate(prefab, position, new Quaternion(), parent);
    }
}
