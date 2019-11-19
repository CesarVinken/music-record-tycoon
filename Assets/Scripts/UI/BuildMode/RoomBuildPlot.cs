using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuildPlot : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public PolygonCollider2D Collider;

    public GameObject BuildTrigger;

    private Vector2 _midpoint = new Vector2(0, 0);
    private Vector2 _startingPoint = new Vector2(0, 0);

    private GameObject _confirmationModal;

    private bool plotIsFree = true;

    public void Awake()
    {
        if (LineRenderer == null)
            Debug.LogError("Cannot find LineRenderer");

        Guard.CheckIsNull(BuildTrigger, "BuildTrigger");
    }
    public void Setup(Room room, Vector2 startingPoint)
    {
        int rightUpAxisLength = RoomBlueprint.RightUpAxisLength;  // later not hardcoded but taken from Room database specifics
        int leftUpAxisLength = RoomBlueprint.LeftUpAxisLength;

        Vector2 point1 = BuilderManager.CalculateLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 point2 = BuilderManager.CalculateLocationOnGrid(point1, 0, -leftUpAxisLength);
        Vector2 point3 = BuilderManager.CalculateLocationOnGrid(point2, -rightUpAxisLength, 0);

        LineRenderer.positionCount = 5;
        LineRenderer.SetPosition(0, startingPoint);
        LineRenderer.SetPosition(1, point1);
        LineRenderer.SetPosition(2, point2);
        LineRenderer.SetPosition(3, point3);
        LineRenderer.SetPosition(4, startingPoint);

        SetColliderPath(startingPoint, rightUpAxisLength, leftUpAxisLength);

        _startingPoint = startingPoint;
        _midpoint = _startingPoint + (point2 - _startingPoint) / 2;

        BuildTrigger.transform.SetParent(MainCanvas.Instance.transform);
        BuildTrigger.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
    }

    public void Update()
    {
        BuildTrigger.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
        if(_confirmationModal)
        {
            _confirmationModal.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
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
        Vector2 colliderPoint1 = BuilderManager.CalculateColliderLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 colliderPoint2 = BuilderManager.CalculateColliderLocationOnGrid(colliderPoint1, 0, -leftUpAxisLength);
        Vector2 colliderPoint3 = BuilderManager.CalculateColliderLocationOnGrid(colliderPoint2, -rightUpAxisLength, 0);

        Vector2[] positions = new Vector2[] { startingPoint, colliderPoint1, colliderPoint2, colliderPoint3, startingPoint };
        Collider.SetPath(0, positions);
    }

    public void CreateBuildConfirmation()
    {
        if (_confirmationModal)
            return;

        GameObject modal = Instantiate(MainCanvas.Instance.ConfirmationModalPrefab);
        modal.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
        modal.transform.SetParent(MainCanvas.Instance.transform);
        _confirmationModal = modal;
        _confirmationModal.GetComponent<ConfirmationModal>().Setup(this);
        
    }

    public void Build()
    {
        GameObject roomGO = Instantiate(BuilderManager.Instance.SelectedRoomPrefab, BuilderManager.Instance.RoomsContainer.transform);
        roomGO.transform.position = _startingPoint;

        Room room = roomGO.GetComponent<Room>();
        RoomManager.Instance.AddRoom(room);

        Vector2 point1 = BuilderManager.CalculateLocationOnGrid(_startingPoint, RoomBlueprint.RightUpAxisLength, 0);
        Vector2 point2 = BuilderManager.CalculateLocationOnGrid(point1, 0, -RoomBlueprint.LeftUpAxisLength);
        Vector2 point3 = BuilderManager.CalculateLocationOnGrid(point2, -RoomBlueprint.RightUpAxisLength, 0);

        Dictionary<Direction, Vector2> roomCorners = new Dictionary<Direction, Vector2>()
        {
            { Direction.Down, _startingPoint },
            { Direction.Right, point1 },
            { Direction.Up, point2 },
            { Direction.Left, point3 },
        };
        room.SetupCorners(roomCorners);

        BuilderManager.Instance.UpdateBuildingTiles(room);
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
