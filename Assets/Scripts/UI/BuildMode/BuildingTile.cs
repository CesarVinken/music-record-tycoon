
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile
{
    public Vector2 StartingPoint;
    public bool IsAvailable = true;

    public static BuildingTile CreateBuildingTile(int rightUpStartPosition, int leftUpStartPosition, bool isAvailable)
    {
        BuildingTile buildingTile = new BuildingTile();
        buildingTile.StartingPoint = BuilderManager.CalculateLocationOnGrid(rightUpStartPosition, leftUpStartPosition);
        buildingTile.IsAvailable = isAvailable;
        BuilderManager.Instance.BuildingTileLocations.Add(buildingTile.StartingPoint);
        return buildingTile;
    }

    public static BuildingTile CreateBuildingTile(Vector2 newTilePosition, bool isAvailable)
    {
        BuildingTile buildingTile = new BuildingTile();
        buildingTile.StartingPoint = newTilePosition;
        buildingTile.IsAvailable = isAvailable;
        BuilderManager.Instance.BuildingTileLocations.Add(buildingTile.StartingPoint);
        return buildingTile;
    }
}