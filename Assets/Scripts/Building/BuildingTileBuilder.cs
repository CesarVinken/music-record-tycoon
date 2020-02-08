using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTileBuilder : MonoBehaviour
{ 
    private BuilderManager _builderManager;

    public BuildingTileBuilder()
    {
        _builderManager = BuilderManager.Instance;
    }
    public void SetupInitialBuildingTiles()
    {
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(0, 0, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(3, 0, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(6, 0, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(9, 0, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(0, 3, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(3, 3, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(6, 3, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(9, 3, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(0, 6, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(3, 6, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(6, 6, Availability.Available));
        _builderManager.BuildingTiles.Add(BuildingTile.CreateBuildingTile(9, 6, Availability.Available));
    }

    public void UpdateBuildingTiles(Room room)
    {
        // Get all building tiles in the location of the room and make them UNAVAILABLE
        List<BuildingTile> roomEdgeTiles = room.GetRoomEdgeTiles();

        room.setAdjacentRooms();

        room.EnableDoors();

        // All tiles in the square of the room + the distance up to the fourth rank of tilse
        List<BuildingTile> surroundingSquareTiles = _builderManager.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= room.RoomCorners[Direction.Left].x - 3 * 15 &&
            tile.StartingPoint.x <= room.RoomCorners[Direction.Right].x + 3 * 15 &&
            tile.StartingPoint.y <= room.RoomCorners[Direction.Up].y + 3 * 15 &&
            tile.StartingPoint.y >= room.RoomCorners[Direction.Down].y - 3 * 15
        );

        // Create new AVAILABLE tiles rings around the room
        List<BuildingTile> firstRankNewTiles = CreateTileRing(roomEdgeTiles, surroundingSquareTiles);
        List<BuildingTile> secondRankNewTiles = CreateTileRing(firstRankNewTiles, surroundingSquareTiles);
        List<BuildingTile> thirdRankNewTiles = CreateTileRing(secondRankNewTiles, surroundingSquareTiles);
        List<BuildingTile> fourthRankNewTiles = CreateTileRing(thirdRankNewTiles, surroundingSquareTiles);

        _builderManager.SetMapPanMaximum(room.RoomCorners);

        BuilderManager.Instance.UpdatePathfindingGrid();
    }

    public BuildingTile CreateNeighbourTile(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength, List<BuildingTile> surroundingSquareTiles)
    {
        Vector2 neighbourLocation = GridHelper.CalculateLocationOnGrid(startingPoint, rightUpAxisLength, leftUpAxisLength);
        if (!_builderManager.BuildingTileLocations.Contains(neighbourLocation))
        {
            BuildingTile tile = BuildingTile.CreateBuildingTile(neighbourLocation, Availability.Available);
            _builderManager.BuildingTiles.Add(tile);

            return tile;
        }

        BuildingTile existingTile = surroundingSquareTiles.Find(surroundingSquareTile => surroundingSquareTile.StartingPoint == neighbourLocation);

        //Some tiles at the direct edge may already exist, but the tile one row further still needs to be created. Therefore, also add this tile to the list.
        if (existingTile != null && existingTile.IsAvailable == Availability.Available)
        {
            return existingTile;
        }
        return null;
    }

    public List<BuildingTile> CreateTileRing(List<BuildingTile> tileRing, List<BuildingTile> surroundingSquareTiles)
    {
        List<BuildingTile> newTiles = new List<BuildingTile>();
        for (int k = 0; k < tileRing.Count; k++)
        {
            Logger.Log(Logger.Building, "tilering tile {0}", tileRing[k].StartingPoint);
            BuildingTile upRight = CreateNeighbourTile(tileRing[k].StartingPoint, 3, 3, surroundingSquareTiles);
            if (upRight != null)
                newTiles.Add(upRight);

            BuildingTile upLeft = CreateNeighbourTile(tileRing[k].StartingPoint, -3, 3, surroundingSquareTiles);
            if (upLeft != null)
                newTiles.Add(upLeft);

            BuildingTile downRight = CreateNeighbourTile(tileRing[k].StartingPoint, 3, -3, surroundingSquareTiles);
            if (downRight != null)
                newTiles.Add(downRight);

            BuildingTile downLeft = CreateNeighbourTile(tileRing[k].StartingPoint, -3, -3, surroundingSquareTiles);
            if (downLeft != null)
                newTiles.Add(downLeft);
        }
        return newTiles;
    }

    public IEnumerator WaitAndUpdatePathfindingGrid()
    {
        yield return new WaitForSeconds(0.01f);
        GameManager.Instance.PathfindingGrid.CreateGrid();  // May have to change to partly recreating the grid.
        //PlayerCharacter.Instance.PlayerLocomotion.StopLocomotion();
        PlayerCharacter.Instance.PlayerNav.IsReevaluating = true;
        yield return new WaitForSeconds(0.08f);

        // TODO: update routes for all moving characters on the map

        PlayerCharacter.Instance.PlayerLocomotion.RetryReachLocomotionTarget();
    }

    public void DrawBuildingTilesGizmos()
    {
        // Draw available/unavailable building tiles
        for (int i = 0; i < BuilderManager.Instance.BuildingTiles.Count; i++)
        {
            Vector2 startingPoint = BuilderManager.Instance.BuildingTiles[i].StartingPoint;
            if (BuilderManager.Instance.BuildingTiles[i].IsAvailable == Availability.Available)
                Gizmos.color = Color.green;
            else if (BuilderManager.Instance.BuildingTiles[i].IsAvailable == Availability.UpperEdge)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawCube(startingPoint, new Vector3(1, 1));
        }
    }
}
