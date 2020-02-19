using System;
using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public PolygonCollider2D Collider;
    public RoomBlueprint RoomBlueprint;

    private Vector2 _startingPoint = new Vector2(0, 0);

    public Vector2 StartingPoint
    {
        get { return _startingPoint; }
        set { _startingPoint = value; }
    }

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
        StartingPoint = startingPoint;

        double rightUpAxisLength = PlotRotation == RoomRotation.Rotation0 || PlotRotation == RoomRotation.Rotation180 ? room.RightUpAxisLength : room.LeftUpAxisLength;
        double leftUpAxisLength = PlotRotation == RoomRotation.Rotation0 || PlotRotation == RoomRotation.Rotation180 ? room.LeftUpAxisLength : room.RightUpAxisLength;

        Vector2 point1 = GridHelper.CalculateLocationOnGrid(StartingPoint, (int)rightUpAxisLength, 0);
        Vector2 point2 = GridHelper.CalculateLocationOnGrid(point1, 0, (int)-leftUpAxisLength);
        Vector2 point3 = GridHelper.CalculateLocationOnGrid(point2, (int)-rightUpAxisLength, 0);

        Vector2[] cornerPositions = new Vector2[] { StartingPoint, point1, point2, point3, StartingPoint };

        SetLineRendererPositionsPath(cornerPositions);
        SetColliderPath(cornerPositions);


        double midpointRightUpAxisLength = rightUpAxisLength / 2;
        double midpointLeftUpAxisLength = leftUpAxisLength / 2;
        if (rightUpAxisLength / 2 - Math.Truncate(rightUpAxisLength / 2) != 0)
        {
            midpointRightUpAxisLength += 1.5f;
        }
        if (leftUpAxisLength / 2 - Math.Truncate(leftUpAxisLength / 2) != 0)
        {
            midpointLeftUpAxisLength -= 1.5f;

        }

        Vector2 midGridPoint = GridHelper.CalculateLocationOnGrid(
            StartingPoint,
            (int)midpointRightUpAxisLength,
            -(int)midpointLeftUpAxisLength
        );

        BuilderManager.Instance.BuildingPlotLocations.Add(midGridPoint, StartingPoint);
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

    public void SetLineRendererPositionsPath(Vector2[] cornerPositions)
    {
        LineRenderer.positionCount = 5;
        LineRenderer.SetPosition(0, cornerPositions[0]);
        LineRenderer.SetPosition(1, cornerPositions[1]);
        LineRenderer.SetPosition(2, cornerPositions[2]);
        LineRenderer.SetPosition(3, cornerPositions[3]);
        LineRenderer.SetPosition(4, cornerPositions[4]);
    }

    public void SetColliderPath(Vector2[] cornerPositions)
    {
        Collider.SetPath(0, cornerPositions);
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
