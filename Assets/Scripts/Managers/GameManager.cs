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
            Debug.LogError("Cannot find BuilderManager");
        if (RoomManager == null)
            Debug.LogError("Cannot find RoomManager");
        if (PathfindingGrid == null)
            Debug.LogError("Cannot find PathfindingGrid");
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
}
