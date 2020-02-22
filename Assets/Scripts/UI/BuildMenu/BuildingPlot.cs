using System;
using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    public PolygonCollider2D Collider;
    public RoomBlueprint RoomBlueprint;
    //public SpriteRenderer SpriteRenderer;

    private Vector2 _startingPoint = new Vector2(0, 0);
    public Material Material;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

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
        if (Material == null)
            Logger.Error(Logger.Initialisation, "Cannot find Material");
        if (MeshFilter == null)
            Logger.Error(Logger.Initialisation, "Cannot find MeshFilter");
        if (MeshRenderer == null)
            Logger.Error(Logger.Initialisation, "Cannot find MeshRenderer");
    }

    public void Setup(RoomBlueprint room, Vector2 startingPoint, RoomRotation roomRotation)
    {
        if (BuilderManager.Instance.BuildingPlotLocations.ContainsValue(startingPoint)) return;

        PlotRotation = roomRotation;
        RoomBlueprint = room;
        StartingPoint = new Vector3(startingPoint.x, startingPoint.y, 1);

        double rightUpAxisLength = PlotRotation == RoomRotation.Rotation0 || PlotRotation == RoomRotation.Rotation180 ? room.RightUpAxisLength : room.LeftUpAxisLength;
        double leftUpAxisLength = PlotRotation == RoomRotation.Rotation0 || PlotRotation == RoomRotation.Rotation180 ? room.LeftUpAxisLength : room.RightUpAxisLength;

        // == counter clockwise, starting at bottom
        Vector2 point1 = GridHelper.CalculateLocationOnGrid(StartingPoint, (int)rightUpAxisLength, 0);
        Vector2 point2 = GridHelper.CalculateLocationOnGrid(point1, 0, (int)-leftUpAxisLength);
        Vector2 point3 = GridHelper.CalculateLocationOnGrid(point2, (int)-rightUpAxisLength, 0);

        SetColliderPath(new Vector2[] { StartingPoint, point1, point2, point3, StartingPoint });

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

        CreateMesh(new Vector2[] { StartingPoint, point1, point2, point3 });
    }

    public static BuildingPlot FindBuildingPlot(Vector2 startingPoint)
    {
        BuildingPlot buildingPlot = BuilderManager.Instance.BuildingPlots[startingPoint];
        return buildingPlot;
    }

    public void CreateMesh(Vector2[] points)
    {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = points[0];
        vertices[1] = points[1];
        vertices[2] = points[2];
        vertices[3] = points[3];

        // clockwise. Represent the index of the vertices
        triangles[0] = 3;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 3;
        triangles[4] = 1;
        triangles[5] = 0;

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        MeshFilter.mesh = mesh;
        MeshRenderer.material = Material;
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
