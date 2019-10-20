using UnityEngine;

public class RoomBuildPlot : MonoBehaviour
{
    public LineRenderer LineRenderer;

    public GameObject BuildTrigger;
    private Vector2 _furthestPoint = new Vector2(0, 0);
    private Vector2 _startingPoint = new Vector2(0, 0);

    private GameObject _confirmationModal;

    public void Awake()
    {
        if (LineRenderer == null)
            Debug.LogError("Cannot find LineRenderer");

        Guard.CheckIsNull(BuildTrigger, "BuildTrigger");
    }
    void Start()
    {
        Vector2 startingPoint = CalculateLocationOnGrid(Vector2.zero, 0, 0);
        int rightUpAxisLength = 7;
        int leftUpAxisLength = 5;

        Vector2 point1 = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 point2 = CalculateLocationOnGrid(point1, 0, -leftUpAxisLength);
        Vector2 point3 = CalculateLocationOnGrid(point2, -rightUpAxisLength, 0);

        LineRenderer.positionCount = 5;
        LineRenderer.SetPosition(0, startingPoint);
        LineRenderer.SetPosition(1, point1);
        LineRenderer.SetPosition(2, point2);
        LineRenderer.SetPosition(3, point3);
        LineRenderer.SetPosition(4, startingPoint);

        _furthestPoint = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, -leftUpAxisLength);
        _startingPoint = startingPoint;
        BuildTrigger.transform.position = Camera.main.WorldToScreenPoint(new Vector2(_furthestPoint.x / 2, _furthestPoint.y / 2));
    }

    public void Update()
    {
        BuildTrigger.transform.position = Camera.main.WorldToScreenPoint(new Vector2(_furthestPoint.x / 2, _furthestPoint.y / 2));
        if(_confirmationModal)
        {
            _confirmationModal.transform.position = Camera.main.WorldToScreenPoint(new Vector2(_furthestPoint.x / 2, _furthestPoint.y / 2));
        }
    }

    public Vector2 CalculateLocationOnGrid(Vector2 startingPoint, int rightUpAxis, int leftUpAxis)
    {
        return new Vector2(startingPoint.x + (rightUpAxis + leftUpAxis) * 5f, startingPoint.y + (rightUpAxis - leftUpAxis) * 2.5f);
    }

    public void CreateBuildConfirmation()
    {
        if (_confirmationModal)
            return;

        Debug.Log("Do you really want to build?");
        GameObject modal = Instantiate(MainCanvas.Instance.ConfirmationModalPrefab);
        modal.transform.position = Camera.main.WorldToScreenPoint(new Vector2(_furthestPoint.x / 2, _furthestPoint.y / 2));
        modal.transform.SetParent(MainCanvas.Instance.transform);
        _confirmationModal = modal;
        _confirmationModal.GetComponent<ConfirmationModal>().Setup(this);
        
    }

    public void Build()
    {
        GameObject room = Instantiate(BuilderManager.Instance.SelectedRoomPrefab, BuilderManager.Instance.RoomsContainer.transform);
        room.transform.position = _startingPoint;
        BuildModeContainer.Instance.DestroyBuildingPlots();
        GameManager.Instance.PathfindingGrid.CreateGrid();  // May have to change to partly recreating the grid.
    }
}
