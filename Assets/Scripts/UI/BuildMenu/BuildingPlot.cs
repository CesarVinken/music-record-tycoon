using System;
using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public PolygonCollider2D Collider;

    private Vector2 _startingPoint = new Vector2(0, 0);

    public static Vector2 AvailablePlotVectorPosition = new Vector2(0, 0);

    private bool plotIsFree = true;

    public void Awake()
    {
        if (LineRenderer == null)
            Logger.Error(Logger.Initialisation, "Cannot find LineRenderer");
    }
    public void Setup(RoomBlueprint room, Vector2 startingPoint)
    {
        if (BuilderManager.Instance.BuildingPlotLocations.ContainsValue(startingPoint)) return;

        double rightUpAxisLength = RoomBlueprint.RightUpAxisLength;  // later not hardcoded but taken from Room database specifics
        double leftUpAxisLength = RoomBlueprint.LeftUpAxisLength;

        Vector2 point1 = BuilderManager.CalculateLocationOnGrid(startingPoint, (int)rightUpAxisLength, 0);
        Vector2 point2 = BuilderManager.CalculateLocationOnGrid(point1, 0, (int)-leftUpAxisLength);
        Vector2 point3 = BuilderManager.CalculateLocationOnGrid(point2, (int)-rightUpAxisLength, 0);

        LineRenderer.positionCount = 5;
        LineRenderer.SetPosition(0, startingPoint);
        LineRenderer.SetPosition(1, point1);
        LineRenderer.SetPosition(2, point2);
        LineRenderer.SetPosition(3, point3);
        LineRenderer.SetPosition(4, startingPoint);

        SetColliderPath(startingPoint, (int)rightUpAxisLength, (int)leftUpAxisLength);

        _startingPoint = startingPoint;

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

        Vector2 midGridPoint = BuilderManager.CalculateLocationOnGrid(
            startingPoint,
            (int)midpointRightUpAxisLength,
            -(int)midpointLeftUpAxisLength
        );

        BuilderManager.Instance.BuildingPlotLocations.Add(midGridPoint, startingPoint);
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
        Vector2 colliderPoint1 = BuilderManager.CalculateColliderLocationOnGrid(startingPoint, rightUpAxisLength, 0);
        Vector2 colliderPoint2 = BuilderManager.CalculateColliderLocationOnGrid(colliderPoint1, 0, -leftUpAxisLength);
        Vector2 colliderPoint3 = BuilderManager.CalculateColliderLocationOnGrid(colliderPoint2, -rightUpAxisLength, 0);

        Vector2[] positions = new Vector2[] { startingPoint, colliderPoint1, colliderPoint2, colliderPoint3, startingPoint };
        Collider.SetPath(0, positions);
    }

    public void Build()
    {
        BuilderManager.Instance.BuildRoom(_startingPoint);
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
