using System;
using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public PolygonCollider2D Collider;
    public RoomBlueprint RoomBlueprint;

    public Vector2 StartingPoint = new Vector2(0, 0);

    public static Vector2 AvailablePlotVectorPosition = new Vector2(0, 0);

    private bool plotIsFree = true;
    public RoomRotation PlotRotation;

    public void Awake()
    {
        if (LineRenderer == null)
            Logger.Error(Logger.Initialisation, "Cannot find LineRenderer");
    }

    public void Setup(RoomBlueprint room, Vector2 startingPoint, RoomRotation roomRotation)
    {
        if (BuilderManager.Instance.BuildingPlotLocations.ContainsValue(startingPoint)) return;

        PlotRotation = roomRotation;
        RoomBlueprint = room;

        double rightUpAxisLength = PlotRotation == RoomRotation.Rotation0 || PlotRotation == RoomRotation.Rotation180 ? room.RightUpAxisLength : room.LeftUpAxisLength;
        double leftUpAxisLength = PlotRotation == RoomRotation.Rotation0 || PlotRotation == RoomRotation.Rotation180 ? room.LeftUpAxisLength : room.RightUpAxisLength;

        Vector2 point1 = GridHelper.CalculateLocationOnGrid(startingPoint, (int)rightUpAxisLength, 0);
        Vector2 point2 = GridHelper.CalculateLocationOnGrid(point1, 0, (int)-leftUpAxisLength);
        Vector2 point3 = GridHelper.CalculateLocationOnGrid(point2, (int)-rightUpAxisLength, 0);

        LineRenderer.positionCount = 5;
        LineRenderer.SetPosition(0, startingPoint);
        LineRenderer.SetPosition(1, point1);
        LineRenderer.SetPosition(2, point2);
        LineRenderer.SetPosition(3, point3);
        LineRenderer.SetPosition(4, startingPoint);

        // TODO:: Collider proportions are incorrect!
        SetColliderPath(startingPoint, (int)rightUpAxisLength, (int)leftUpAxisLength);

        StartingPoint = startingPoint;

        double midpointRightUpAxisLength = rightUpAxisLength / 2;
        double midpointLeftUpAxisLength = leftUpAxisLength / 2;
        if (rightUpAxisLength / 2 - Math.Truncate(rightUpAxisLength / 2) != 0)
        {
            midpointRightUpAxisLength += 1.5f;
        }
        if (leftUpAxisLength / 2 - Math.Truncate(leftUpAxisLength / 2) != 0)
        {
            midpointLeftUpAxisLength -= 1.5f;   //DOUBLE CHECK IF IT SHOULD BE - OR +
        }

        Vector2 midGridPoint = GridHelper.CalculateLocationOnGrid(
            startingPoint,
            (int)midpointRightUpAxisLength,
            -(int)midpointLeftUpAxisLength
        );

        BuilderManager.Instance.BuildingPlotLocations.Add(midGridPoint, startingPoint);
    }

    public static BuildingPlot FindBuildingPlot(Vector2 startingPoint)
    {
        BuildingPlot buildingPlot = BuilderManager.Instance.BuildingPlots[startingPoint];
        return buildingPlot;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Logger.Log(Logger.Locomotion, "something entered collider");
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
        Vector2 colliderPoint1 = GridHelper.CalculateColliderLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 colliderPoint2 = GridHelper.CalculateColliderLocationOnGrid(colliderPoint1, 0, -leftUpAxisLength);
        Vector2 colliderPoint3 = GridHelper.CalculateColliderLocationOnGrid(colliderPoint2, -rightUpAxisLength, 0);

        Vector2[] positions = new Vector2[] { startingPoint, colliderPoint1, colliderPoint2, colliderPoint3, startingPoint };
        Collider.SetPath(0, positions);
    }

    public void MakePlotAvailable()
    {
        plotIsFree = true;
    }

    public void MakePlotUnavailable()
    {
        plotIsFree = false;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
