using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class BuilderManager : MonoBehaviour
{
    public static BuilderManager Instance;

    public static bool InBuildMode; // Build mode = either build panel is open or the player is dragging the building icon around
    public static bool InDeleteObjectMode;
    public static bool PointerIsOnAvailablePlot;

    public GameObject BuildPlotPrefab;
    public GameObject BuildHallwayTriggerPrefab;
    public GameObject DeleteRoomTriggerPrefab;

    public RoomBlueprint SelectedRoom;

    public GameObject RoomsContainer;

    public List<BuildingTile> BuildingTiles = new List<BuildingTile>();
    public Dictionary<Vector2, BuildingPlot> BuildingPlots = new Dictionary<Vector2, BuildingPlot>();
    public Dictionary<RoomName, Dictionary<RoomRotation, GameObject>> RoomPrefabs = new Dictionary<RoomName, Dictionary<RoomRotation, GameObject>>();
    public HashSet<Vector2> BuildingTileLocations = new HashSet<Vector2>();
    public Dictionary<Vector2, Vector2> BuildingPlotLocations = new Dictionary<Vector2, Vector2>();   // The middle position of the building plot, which is the location that triggers the build, AND the starting point location of the plot (bottom)

    private BuildingPlotBuilder _buildingPlotBuilder;
    private BuildingTileBuilder _buildingTileBuilder;
    private RoomBuilder _roomBuilder;

    void Awake()
    {
        Instance = this;
        InBuildMode = false;
        InDeleteObjectMode = false;
        PointerIsOnAvailablePlot = false;

        SelectedRoom = null;

        //Guard.CheckIsNull(Room1Prefab, "Room1Prefab");
        Guard.CheckIsNull(BuildPlotPrefab, "BuildPlotPrefab");
        Guard.CheckIsNull(RoomsContainer, "RoomsContainer");
        Guard.CheckIsNull(BuildHallwayTriggerPrefab, "BuildHallwayTriggerPrefab");
        Guard.CheckIsNull(DeleteRoomTriggerPrefab, "DeleteRoomTriggerPrefab");

        BuildingPlots.Clear();
        BuildingPlotLocations.Clear();

        Dictionary<RoomRotation, GameObject> Room1Prefabs = new Dictionary<RoomRotation, GameObject>();
        Room1Prefabs.Add(RoomRotation.Rotation0, (GameObject)Resources.Load("Prefabs/Scenery/Rooms/Room1/Room1Rotation0", typeof(GameObject)));
        Room1Prefabs.Add(RoomRotation.Rotation90, (GameObject)Resources.Load("Prefabs/Scenery/Rooms/Room1/Room1Rotation90", typeof(GameObject)));
        Room1Prefabs.Add(RoomRotation.Rotation180, (GameObject)Resources.Load("Prefabs/Scenery/Rooms/Room1/Room1Rotation180", typeof(GameObject)));
        Room1Prefabs.Add(RoomRotation.Rotation270, (GameObject)Resources.Load("Prefabs/Scenery/Rooms/Room1/Room1Rotation270", typeof(GameObject)));
        RoomPrefabs.Add(RoomName.Room1, Room1Prefabs);

        Dictionary<RoomRotation, GameObject> HallwayPrefabs = new Dictionary<RoomRotation, GameObject>();
        HallwayPrefabs.Add(RoomRotation.Rotation0, (GameObject)Resources.Load("Prefabs/Scenery/Rooms/Hallway", typeof(GameObject)));

        RoomPrefabs.Add(RoomName.Hallway, HallwayPrefabs);

        _buildingPlotBuilder = new BuildingPlotBuilder();
        _buildingTileBuilder = new BuildingTileBuilder();
        _roomBuilder = new RoomBuilder();

    }

    public void Start()
    {
        _buildingTileBuilder.SetupInitialBuildingTiles();    // Temporary, should only happen for empty map!

        SetupInitialRoom();
    }

    void Update()
    {
        if (InBuildMode && MainCanvas.Instance.IsDraggingIcon && !BuildMenuContainer.Instance.IsOpen)
        {
            if(PointerIsOnAvailablePlot)
            {
                if (GameManager.Instance.CurrentPlatform == Platform.PC)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Logger.Warning("Let's build!");
                        BuildingPlot buildingPlot = BuildingPlot.FindBuildingPlot(BuildingPlot.AvailablePlotVectorPosition);
                        BuildRoom(SelectedRoom, buildingPlot);

                        if (BuildMenuContainer.Instance.PanelAnimationPlaying)
                        {
                            ReopenBuildMenu();
                        }
                        else
                        {
                            BuildMenuContainer.Instance.ActivateAnimationFreeze();
                            ActivateBuildMenuMode();
                        }
                    }
                }
            }
            else
            {
                if (GameManager.Instance.CurrentPlatform == Platform.PC)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Logger.Warning("Cannot build here!");
                        GameObject notificationGO = Instantiate(MainCanvas.Instance.NotificationPrefab, MainCanvas.Instance.transform);
                        Notification notification = notificationGO.transform.GetComponent<Notification>();
                        notification.Setup(NotificationType.FromPointer, "Cannot build in location");

                        if (BuildMenuContainer.Instance.PanelAnimationPlaying)
                        {
                            ReopenBuildMenu();
                        }
                        else
                        {
                            BuildMenuContainer.Instance.ActivateAnimationFreeze();
                            ActivateBuildMenuMode();
                        }
                    }
                }
            }
        }

        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                if (MainCanvas.Instance.PointerImage.sprite != null && !BuildMenuContainer.Instance.IsOpen)
                {
                    if (PointerIsOnAvailablePlot)
                    {
                        Logger.Warning("Let's build!");
                        BuildingPlot buildingPlot = BuildingPlot.FindBuildingPlot(BuildingPlot.AvailablePlotVectorPosition);
                        BuildRoom(SelectedRoom, buildingPlot);
                    }
                    else
                    {
                        if (MainCanvas.Instance.IsDraggingIcon && !BuildMenuContainer.Instance.IsOpen)
                        {
                            GameObject notificationGO = Instantiate(MainCanvas.Instance.NotificationPrefab, MainCanvas.Instance.transform);
                            Notification notification = notificationGO.transform.GetComponent<Notification>();
                            notification.Setup(NotificationType.FromPointer, "Cannot build in location");
                        }
                    }

                    if (BuildMenuContainer.Instance.PanelAnimationPlaying)
                    {
                        ReopenBuildMenu();
                    }
                    else
                    {
                        BuildMenuContainer.Instance.ActivateAnimationFreeze();
                        ActivateBuildMenuMode();
                    }
                }
                else
                {
                    MainCanvas.Instance.UnsetPointerImage();
                }
            }
        }

        if (ConfirmationModal.CurrentConfirmationModal != null)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began))
            {
                bool isPointerOverGameObject = PointerHelper.IsPointerOverGameObject();
                if(!isPointerOverGameObject)
                {
                    ConfirmationModal.CurrentConfirmationModal.ResetDeleteTrigger();
                    ConfirmationModal.CurrentConfirmationModal.DestroyConfirmationModal();
                }
            }
        }
    }

    public void DrawAvailablePlots()
    {
        _buildingPlotBuilder.DrawAvailablePlots();
    }

    public async void SetupInitialRoom()
    {
        RoomBlueprint room1 = RoomBlueprint.CreateBlueprint(RoomName.Room1);
        await BuildRoom(room1, new Vector2(0, 0), RoomRotation.Rotation0);
    }

    public void SetSelectedRoom(RoomBlueprint selectedRoom)
    {
        SelectedRoom = selectedRoom;
    }

    public async Task BuildRoom(RoomBlueprint roomBlueprint, Vector2 startingPoint, RoomRotation roomRotation)
    {
        await _roomBuilder.BuildRoom(roomBlueprint, _buildingTileBuilder, _buildingPlotBuilder, startingPoint, roomRotation);
    }

    public void BuildRoom(RoomBlueprint roomBlueprint, BuildingPlot buildingPlot)
    {
        _roomBuilder.BuildRoom(roomBlueprint, _buildingTileBuilder, _buildingPlotBuilder, buildingPlot);
    }

    public void ActivateBuildMenuMode()
    {
        MainCanvas.Instance.UnsetPointerImage();

        if(BuildingPlots.Count > 0)
        {
            BuildMenuWorldSpaceContainer.Instance.DestroyBuildingPlots();
        }

        BuildMenuContainer.Instance.IsOpen = true;
        BuildMenuContainer.Instance.IsBuilding = true;
        BuildMenuContainer.Instance.LoadBuildMenuContent(BuildMenuTabType.Rooms);

        BuildMenuTabContainer.Instance.ActivateBuildMenuTabs();

        BuildMenuContainer.Instance.CompletePanelActivation();
    }

    // Only happens when the playing does a building action while the panel is still closing
    public void ReopenBuildMenu()
    {
        MainCanvas.Instance.UnsetPointerImage();

        if (BuildingPlots.Count > 0)
        {
            BuildMenuWorldSpaceContainer.Instance.DestroyBuildingPlots();
        }

        // wait for closing to complete
        IEnumerator waitAndReopenPanelRoutine = BuildMenuContainer.Instance.WaitAndReopenPanelRoutine();
        StartCoroutine(waitAndReopenPanelRoutine);
    }

    public void DeactivateBuildMenuMode()
    {
        InBuildMode = false;
        BuildMenuContainer.Instance.IsOpen = false;
        BuildMenuContainer.Instance.IsBuilding = false;

        BuildMenuContainer.Instance.RemoveBuildMenuContent(0.5f);
        MainCanvas.Instance.UnsetPointerImage();
        DeleteAllTriggers();

        BuildMenuWorldSpaceContainer.Instance.DestroyBuildingPlots();
        BuildMenuTabContainer.Instance.ResetCurrentBuildMenuTab();

        if (InDeleteObjectMode)
            Instance.DeactivateDeleteRoomMode();
    }

    public void SetMapPanMaximum(Dictionary<Direction, Vector2> newRoomCorners)
    {
        float currentMaxY = CameraController.PanLimits[Direction.Up];
        float currentMaxX = CameraController.PanLimits[Direction.Right];
        float currentMinY = CameraController.PanLimits[Direction.Down];
        float currentMinX = CameraController.PanLimits[Direction.Left];
        float panPadding = 20f;

        if (currentMaxY <= newRoomCorners[Direction.Up].y + panPadding)
            CameraController.PanLimits[Direction.Up] = newRoomCorners[Direction.Up].y + panPadding;
        if (currentMaxX <= newRoomCorners[Direction.Right].x + panPadding)
            CameraController.PanLimits[Direction.Right] = newRoomCorners[Direction.Right].x + panPadding;
        if (currentMinY >= newRoomCorners[Direction.Down].y - panPadding)
            CameraController.PanLimits[Direction.Down] = newRoomCorners[Direction.Down].y - panPadding;
        if (currentMinX >= newRoomCorners[Direction.Left].x - panPadding)
            CameraController.PanLimits[Direction.Left] = newRoomCorners[Direction.Left].x - panPadding;
    }

    public void DrawDoorLocationGizmos()
    {
        for (int i = 0; i < RoomManager.Rooms.Count; i++)
        {
            Room room = RoomManager.Rooms[i];

            for (int j = 0; j < room.Doors.Count; j++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(room.Doors[j].transform.position, new Vector3(1.5f, 1.5f));
            }      
        }
    }

    public void DrawBuildingTilesGizmos()
    {
        _buildingTileBuilder.DrawBuildingTilesGizmos();
    }


    public void ActivateDeleteRoomMode()
    {
        if (MainCanvas.Instance.IsDraggingIcon)
        {
            MainCanvas.Instance.UnsetPointerImage();
        }

        DeleteAllTriggers();

        InDeleteObjectMode = true;

        foreach (Room room in RoomManager.Rooms)
        {
            DeleteRoomTrigger deleteRoomTrigger = Instantiate(DeleteRoomTriggerPrefab, MainCanvas.Instance.TriggersContainer.transform).GetComponent<DeleteRoomTrigger>();
            if (room.CharactersInRoom.Count > 0) deleteRoomTrigger.gameObject.SetActive(false);
            deleteRoomTrigger.Setup(room);
        }
    }

    public void DeactivateDeleteRoomMode()
    {
        InDeleteObjectMode = false;

        DeleteRoomTrigger.DeleteAllDeleteRoomTriggers();
        if (ConfirmationModal.CurrentConfirmationModal)
            ConfirmationModal.CurrentConfirmationModal.DestroyConfirmationModal();
    }

    public void DeleteAllTriggers()
    {
        BuildHallwayTrigger.DeleteAllHallwayTriggers();
    }

    public void DeleteRoom(Room room)
    {
        Room tempRoomCopy = room;
        room.RemoveDoorConnectionFromAdjacentRooms();
        room.RemoveThisRoomFromAdjacentRooms();
        room.DeleteRoom();
        Logger.Warning(Logger.Building, "Deleting room: {0}", tempRoomCopy.Id);
        tempRoomCopy.CleanUpDeletedRoomTiles();


        for (int l = 0; l < RoomManager.Rooms.Count; l++)
        {
            Logger.Log(Logger.Building, "{0} has {1} adjacent rooms", RoomManager.Rooms[l].Id, RoomManager.Rooms[l].AdjacentRooms.Count);
        }
    }

    //public void WaitAndBuild(RoomBlueprint roomBlueprint, Vector2 startingPoint, RoomRotation roomRotation)
    //{
    //    IEnumerator waitAndBuildRoutine = WaitAndBuildRoutine(roomBlueprint, startingPoint, roomRotation);
    //    StartCoroutine(waitAndBuildRoutine);
    //}

    //public IEnumerator WaitAndBuildRoutine(RoomBlueprint roomBlueprint, Vector2 startingPoint, RoomRotation roomRotation)
    //{
    //    yield return new WaitForSeconds(1f);
    //    BuildRoom(roomBlueprint, startingPoint, roomRotation);

    //}
}
