
using System.Collections.Generic;
using UnityEngine;

public enum Availability
{
    Available,
    UpperEdge,
    Unavailable
}

public class BuildingTile
{
    public Vector2 StartingPoint;
    public Availability IsAvailable = Availability.Available;
    public List<Room> BuildingTileRooms = new List<Room>(); // the rooms of which this tile is a part

    public static BuildingTile CreateBuildingTile(int rightUpStartPosition, int leftUpStartPosition, Availability availability)
    {
        BuildingTile buildingTile = new BuildingTile();
        buildingTile.StartingPoint = GridHelper.GridToVectorLocation(rightUpStartPosition, leftUpStartPosition);
        buildingTile.IsAvailable = availability;
        BuilderManager.Instance.BuildingTileLocations.Add(buildingTile.StartingPoint);
        return buildingTile;
    }

    public static BuildingTile CreateBuildingTile(Vector2 newTilePosition, Availability availability)
    {
        BuildingTile buildingTile = new BuildingTile();
        buildingTile.StartingPoint = newTilePosition;
        buildingTile.IsAvailable = availability;
        BuilderManager.Instance.BuildingTileLocations.Add(buildingTile.StartingPoint);
        return buildingTile;
    }
}