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

    public static bool ShowPathfindingGridGizmos;
    public static bool DrawCharacterPathGizmo;
    public static bool DrawBuildingTilesGizmos;
    public static bool DrawDoorLocationGizmos;


    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;
    public Grid PathfindingGrid;    // might move to different location

    public BuilderManager BuilderManager;
    public RoomManager RoomManager;
    public GridLayout WorldGrid;

    private static bool _mainMenuOpen;

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

        ShowPathfindingGridGizmos = false;
        DrawCharacterPathGizmo = false;
        DrawBuildingTilesGizmos = false;
        DrawDoorLocationGizmos = false;
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
        if(ShowPathfindingGridGizmos)
            PathfindingGrid.DrawPathfindingGridGizmos();

        if (DrawCharacterPathGizmo)
        {
            if (CharacterManager.Instance != null && CharacterManager.Instance.SelectedCharacter != null && CharacterManager.Instance.SelectedCharacter.NavActor != null)
                CharacterManager.Instance.SelectedCharacter.NavActor.DrawPathGizmo();
        }


        if (BuilderManager.Instance != null)
        {
            if(DrawBuildingTilesGizmos)
                BuilderManager.Instance.DrawBuildingTilesGizmos();

            if(DrawDoorLocationGizmos)
                BuilderManager.Instance.DrawDoorLocationGizmos();
        }
    }

    public void InitialiseLoggers()
    {
        Logger.General.enableLogs = true;
        Logger.Time.enableLogs = true;
        Logger.Locomotion.enableLogs = false;
        Logger.Building.enableLogs = false;
        Logger.Pathfinding.enableLogs = false;
        Logger.Initialisation.enableLogs = true;
        Logger.Character.enableLogs = false;
    }

    public GameObject InstantiatePrefab(GameObject prefab, Transform parent, Vector2 position)
    {
        return Instantiate(prefab, position, new Quaternion(), parent);
    }
}
