using UnityEngine;

public class RoomBuildPlot : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public PolygonCollider2D Collider;

    public GameObject BuildTrigger;

    private Vector2 _furthestPoint = new Vector2(0, 0);
    private Vector2 _startingPoint = new Vector2(0, 0);

    private GameObject _confirmationModal;

    private bool plotIsFree = true;

    public void Awake()
    {
        if (LineRenderer == null)
            Debug.LogError("Cannot find LineRenderer");
        //if (Collider == null)
        //    Debug.LogError("Cannot find PolygonCollider2D");

        Guard.CheckIsNull(BuildTrigger, "BuildTrigger");
    }
    void Start()
    {
        Vector2 startingPoint = CalculateLocationOnGrid(Vector2.zero, 0, 0);  // later not hardcoded
        int rightUpAxisLength = 7;  // later not hardcoded but taken from Room property
        int leftUpAxisLength = 5; // later not hardcoded

        Vector2 point1 = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 point2 = CalculateLocationOnGrid(point1, 0, -leftUpAxisLength);
        Vector2 point3 = CalculateLocationOnGrid(point2, -rightUpAxisLength, 0);

        LineRenderer.positionCount = 5;
        LineRenderer.SetPosition(0, startingPoint);
        LineRenderer.SetPosition(1, point1);
        LineRenderer.SetPosition(2, point2);
        LineRenderer.SetPosition(3, point3);
        LineRenderer.SetPosition(4, startingPoint);

        SetColliderPath(startingPoint, rightUpAxisLength, leftUpAxisLength);

        _furthestPoint = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, -leftUpAxisLength);
        _startingPoint = startingPoint;

        BuildTrigger.transform.SetParent(MainCanvas.Instance.transform);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("something entered collider");
        if (plotIsFree)
        {
            MakePlotUnavailable();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!plotIsFree)
        {
            MakePlotAvailable();
        }
    }

    public void SetColliderPath(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    {
        Vector2 colliderPoint1 = CalculateColliderLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 colliderPoint2 = CalculateColliderLocationOnGrid(colliderPoint1, 0, -leftUpAxisLength);
        Vector2 colliderPoint3 = CalculateColliderLocationOnGrid(colliderPoint2, -rightUpAxisLength, 0);

        Vector2[] positions = new Vector2[] { startingPoint, colliderPoint1, colliderPoint2, colliderPoint3, startingPoint };
        Collider.SetPath(0, positions);
    }

    public Vector2 CalculateLocationOnGrid(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    {
        return new Vector2(startingPoint.x + (rightUpAxisLength + leftUpAxisLength) * 5f, startingPoint.y + (rightUpAxisLength - leftUpAxisLength) * 2.5f);
    }

    public Vector2 CalculateColliderLocationOnGrid(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    {
        return new Vector2(startingPoint.x + (rightUpAxisLength + leftUpAxisLength) * 6f, startingPoint.y + (rightUpAxisLength - leftUpAxisLength) * 3f);
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
        GameManager.Instance.PathfindingGrid.CreateGrid();  // May have to change to partly recreating the grid.
    }

    public void MakePlotAvailable()
    {
        plotIsFree = true;
        BuildTrigger.SetActive(true);

        if (_confirmationModal)
            _confirmationModal.SetActive(true);
    }

    public void MakePlotUnavailable()
    {
        plotIsFree = false;
        BuildTrigger.SetActive(false);

        if (_confirmationModal)
            _confirmationModal.SetActive(false);
    }

    public void DestroySelf()
    {
        if (_confirmationModal)
            Destroy(_confirmationModal);
        Destroy(BuildTrigger);
        Destroy(gameObject);
    }
}
