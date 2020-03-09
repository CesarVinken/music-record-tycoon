using UnityEngine;

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
        PathfindingGrid.DrawPathfindingGridGizmos();

        if (CharacterManager.Instance != null && CharacterManager.Instance.SelectedCharacter != null && CharacterManager.Instance.SelectedCharacter.NavActor != null)
            CharacterManager.Instance.SelectedCharacter.NavActor.DrawPathGizmo();

        if (BuilderManager.Instance != null)
        {
            BuilderManager.Instance.DrawBuildingTilesGizmos();

            //BuilderManager.Instance.DrawDoorLocationGizmos();
        }
    }

    public void InitialiseLoggers()
    {
        Logger.General.enableLogs = true;
        Logger.Locomotion.enableLogs = false;
        Logger.Building.enableLogs = true;
        Logger.Pathfinding.enableLogs = false;
        Logger.Initialisation.enableLogs = true;
    }

    public GameObject InstantiatePrefab(GameObject prefab, Transform parent)
    {
        return Instantiate(prefab, parent);
    }
}
