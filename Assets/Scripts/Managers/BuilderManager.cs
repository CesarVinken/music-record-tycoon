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
    public Dictionary<RoomName, Dictionary<ObjectRotation, GameObject>> RegisteredRoomPrefabs = new Dictionary<RoomName, Dictionary<ObjectRotation, GameObject>>();
    public Dictionary<RoomObjectName, Dictionary<ObjectRotation, GameObject>> PlaceableRoomObjectPrefabs = new Dictionary<RoomObjectName, Dictionary<ObjectRotation, GameObject>>();
    public HashSet<Vector2> BuildingTileLocations = new HashSet<Vector2>();
    public Dictionary<Vector2, Vector2> BuildingPlotLocations = new Dictionary<Vector2, Vector2>();   // The middle position of the building plot, which is the location that triggers the build, AND the starting point location of the plot (bottom)

    private BuildingPlotBuilder _buildingPlotBuilder;
    private BuildingTileBuilder _buildingTileBuilder;
    private RoomBuilder _roomBuilder;
    private RoomObjectBuilder _roomObjectBuilder;

    public List<RoomName> RegisteredRooms;
    public List<RoomObjectName> RegisteredPlaceableRoomObjects;

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

        //////////////////
        // Room prefabs
        //////////////////

        RegisterRooms();
        LoadRoomPrefabs();

        //////////////////
        // Room object prefabs
        //////////////////

        RegisterPlaceableRoomObjects();
        LoadPlaceableRoomObjects();





        _buildingPlotBuilder = new BuildingPlotBuilder();
        _buildingTileBuilder = new BuildingTileBuilder();
        _roomBuilder = new RoomBuilder();
        _roomObjectBuilder = new RoomObjectBuilder();

    }

    public async void Start()
    {
        _buildingTileBuilder.SetupInitialBuildingTiles();    // Temporary, should only happen for empty map!

        await SetupInitialRoom();
    }

    private void RegisterRooms()
    {
        RegisteredRooms = new List<RoomName>();

        RegisteredRooms.Add(RoomName.Room1);
        RegisteredRooms.Add(RoomName.Hallway);
        RegisteredRooms.Add(RoomName.RecordingStudio1);
    }
    
    private void LoadRoomPrefabs()
    {
        for (int i = 0; i < RegisteredRooms.Count; i++)
        {
            RoomName roomName = RegisteredRooms[i];
            Dictionary<ObjectRotation, GameObject> RoomPrefabs = new Dictionary<ObjectRotation, GameObject>();

            foreach (ObjectRotation rotation in ObjectRotation.GetValues(typeof(ObjectRotation)))
            {
                GameObject roomPrefab = (GameObject)Resources.Load("Prefabs/Scenery/Rooms/" + roomName + "/" + roomName + rotation, typeof(GameObject));
                if (roomPrefab != null) RoomPrefabs.Add(rotation, roomPrefab);
            }

            RegisteredRoomPrefabs.Add(roomName, RoomPrefabs);
        }
    }

    private void RegisterPlaceableRoomObjects()
    {
        RegisteredPlaceableRoomObjects = new List<RoomObjectName>();

        RegisteredPlaceableRoomObjects.Add(RoomObjectName.Guitar);
        RegisteredPlaceableRoomObjects.Add(RoomObjectName.Piano);
    }

    private void LoadPlaceableRoomObjects()
    {
        for (int i = 0; i < RegisteredPlaceableRoomObjects.Count; i++)
        {
            RoomObjectName roomObjectName = RegisteredPlaceableRoomObjects[i];
            Dictionary<ObjectRotation, GameObject> RoomObjectPrefabs = new Dictionary<ObjectRotation, GameObject>();

            foreach (ObjectRotation rotation in ObjectRotation.GetValues(typeof(ObjectRotation)))
            {
                GameObject roomObjectPrefab = (GameObject)Resources.Load("Prefabs/Scenery/RoomObjects/" + roomObjectName + "/" + roomObjectName + rotation, typeof(GameObject));
                if (roomObjectPrefab != null) RoomObjectPrefabs.Add(rotation, roomObjectPrefab);
            }

            PlaceableRoomObjectPrefabs.Add(roomObjectName, RoomObjectPrefabs);
        }
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
                        Logger.Warning("Let's build a {0}", SelectedRoom.Name);
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

    public async Task SetupInitialRoom()
    {
        RoomBlueprint room1 = RoomBlueprint.CreateBlueprint(RoomName.Room1);
        await BuildRoom(room1, new Vector2(0, 0), ObjectRotation.Rotation0);
        return;
    }

    public void SetSelectedRoom(RoomBlueprint selectedRoom)
    {
        SelectedRoom = selectedRoom;
    }

    public async Task BuildRoom(RoomBlueprint roomBlueprint, Vector2 startingPoint, ObjectRotation roomRotation)
    {
        await _roomBuilder.BuildRoom(roomBlueprint, _buildingTileBuilder, _buildingPlotBuilder, startingPoint, roomRotation);
    }

    public async void BuildRoom(RoomBlueprint roomBlueprint, BuildingPlot buildingPlot)
    {
        await _roomBuilder.BuildRoom(roomBlueprint, _buildingTileBuilder, _buildingPlotBuilder, buildingPlot);
    }

    public void BuildRoomObject(RoomObjectBlueprint roomObjectBlueprint, GridLocation roomObjectLocation, Room parentRoom)
    {
        _roomObjectBuilder.BuildRoomObject(roomObjectBlueprint, roomObjectLocation, parentRoom);
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
        room.ReenableWallpiecesFromAdjacentRooms();
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
}
