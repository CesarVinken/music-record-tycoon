using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public static BuilderManager Instance;

    public static bool InBuildMode;
    public static bool HasRoomSelected;

    public GameObject Room1Prefab; 
    public GameObject SelectedRoomPrefab;
    public GameObject SelectedRoom;
    public GameObject Room1BuildPlotPrefab;
    public GameObject RoomsContainer;

    void Awake()
    {
        Instance = this;
        InBuildMode = false;
        HasRoomSelected = false;

        SelectedRoom = null;

        Guard.CheckIsNull(Room1Prefab, "Room1Prefab");
        Guard.CheckIsNull(Room1BuildPlotPrefab, "Room1BuildPlotPrefab");
        Guard.CheckIsNull(RoomsContainer, "RoomsContainer");


        SelectedRoomPrefab = Room1Prefab;  // Should become generic later on.
    }

    void Update()
    {
        if (HasRoomSelected && !GameManager.MainMenuOpen)
        {
            if (Input.GetMouseButtonDown(1)) {
                UnsetSelectedRoom();
            }
        }
    }

    public void EnterBuildMode()
    {
        InBuildMode = true;
        InGameButtons.Instance.CreateButtonsForBuildMode();
        InGameButtons.Instance.DeleteButtonsForBuildMode();
    }

    public void ExitBuildMode()
    {
        InBuildMode = false;
        HasRoomSelected = false;
        SelectedRoom = null;

        InGameButtons.Instance.CreateButtonsForPlayMode();
        InGameButtons.Instance.DeleteButtonsForPlayMode();
        BuildModeContainer.Instance.DestroyBuildingPlots();

        if (ConfirmationModal.CurrentConfirmationModal)
            ConfirmationModal.CurrentConfirmationModal.DestroyConfirmationModal();
    }

    public void SetSelectedRoom(GameObject room)
    {
        // Should maybe become Room type? For example: SelectedRoom = room.Prefab
        SelectedRoom = room;

        HasRoomSelected = true;
        DrawAvaiablePlots();
    }

    public void UnsetSelectedRoom()
    {
        SelectedRoom = null;
        HasRoomSelected = false;
        Debug.Log("no room selected.");
    }

    public void DrawAvaiablePlots()
    {
        BuildModeContainer.Instance.DestroyBuildingPlots();

        // calculate where there should be plots

        // create room build plot
        BuildModeContainer.Instance.CreateBuildingPlot(Room1BuildPlotPrefab);
    }

    //public void OnDrawGizmos()
    //{
    //    Vector2 startingPoint = CalculateLocationOnGrid(Vector2.zero, 4, 0);
    //    int rightUpAxisLength = 7;
    //    int leftUpAxisLength = 5;

    //    Gizmos.DrawCube(new Vector2(0, 0), new Vector3(1, 1));
    //    Vector2 furthestPoint = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, -leftUpAxisLength);
    //    Gizmos.DrawCube(new Vector2(furthestPoint.x / 2, furthestPoint.y / 2), new Vector3(1, 1));

    //    DrawRect(startingPoint, rightUpAxisLength, leftUpAxisLength);
    //}

    //public void DrawRect(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    //{
    //    Vector2 point1 = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, 0);
    //    Vector2 point2 = CalculateLocationOnGrid(point1, 0, -leftUpAxisLength);
    //    Vector2 point3 = CalculateLocationOnGrid(point2, -rightUpAxisLength, 0);

    //    Gizmos.DrawLine(startingPoint, point1);
    //    Gizmos.DrawLine(point1, point2);
    //    Gizmos.DrawLine(point2, point3);
    //    Gizmos.DrawLine(point3, startingPoint);
    //}

    //public Vector2 CalculateLocationOnGrid(Vector2 startingPoint, int rightUpAxis, int leftUpAxis)
    //{
    //    return new Vector2(startingPoint.x + (rightUpAxis + leftUpAxis) * 5f, startingPoint.y + (rightUpAxis - leftUpAxis) * 2.5f);
    //}
}
