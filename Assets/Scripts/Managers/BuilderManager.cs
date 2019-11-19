using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public static BuilderManager Instance;

    public static bool InBuildMode;
    public static bool HasRoomSelected;

    public GameObject Room1Prefab; 
    public GameObject SelectedRoomPrefab;
    public Room SelectedRoom;
    public GameObject Room1BuildPlotPrefab;
    public GameObject RoomsContainer;

    public List<BuildingTile> BuildingTiles = new List<BuildingTile>();
    public HashSet<Vector2> BuildingTileLocations = new HashSet<Vector2>();

    void Awake()
    {
        Instance = this;
        InBuildMode = false;
        HasRoomSelected = false;

        SelectedRoom = null;

        Guard.CheckIsNull(Room1Prefab, "Room1Prefab");
        Guard.CheckIsNull(Room1BuildPlotPrefab, "Room1BuildPlotPrefab");
        Guard.CheckIsNull(RoomsContainer, "RoomsContainer");


        SelectedRoomPrefab = Room1Prefab;  // Should become generic later on.

        SetupInitialBuildingTiles();    // Temporary, should only happen for empty map!
    }

    void Update()
    {
        if (HasRoomSelected && !GameManager.MainMenuOpen)
        {
            if (Input.GetMouseButtonDown(1)) {
                UnsetSelectedRoom();
            }
        }
    }

    public void SetupInitialBuildingTiles()
    {
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(0, 0, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(3, 0, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(6, 0, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(9, 0, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(0, -3, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(3, -3, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(6, -3, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(9, -3, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(0, -6, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(3, -6, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(6, -6, true));
        BuildingTiles.Add(BuildingTile.CreateBuildingTile(9, -6, true));
    }

    public void EnterBuildMode()
    {
        InBuildMode = true;
        InGameButtons.Instance.CreateButtonsForBuildMode();
        InGameButtons.Instance.DeleteButtonsForBuildMode();
    }

    public void ExitBuildMode()
    {
        InBuildMode = false;
        HasRoomSelected = false;
        SelectedRoom = null;

        InGameButtons.Instance.CreateButtonsForPlayMode();
        InGameButtons.Instance.DeleteButtonsForPlayMode();
        BuildModeContainer.Instance.DestroyBuildingPlots();

        if (ConfirmationModal.CurrentConfirmationModal)
            ConfirmationModal.CurrentConfirmationModal.DestroyConfirmationModal();
    }

    public void SetSelectedRoom(Room room)
    {
        // Should maybe become Room type? For example: SelectedRoom = room.Prefab
        SelectedRoom = room;

        HasRoomSelected = true;
        DrawAvailablePlots(room);
    }

    public void UnsetSelectedRoom()
    {
        SelectedRoom = null;
        HasRoomSelected = false;
        Debug.Log("no room selected.");
    }

    public void DrawAvailablePlots(Room selectedRoom)
    {
        BuildModeContainer.Instance.DestroyBuildingPlots();

        // for each door, check if it aligns with the doors of the selected room
        for (int i = 0; i < RoomManager.Rooms.Count; i++)
        {
            Room room = RoomManager.Rooms[i];
            // for each door, check if it aligns with the doors of the selected room
            foreach (KeyValuePair<Door, bool> door in room.Doors)
            {
                if (door.Value) continue; // if the door is already activated, no need to check for new connections

                // the doors that are already on the map
                GridLocation roomStartPosition = CalculateGridLocationFromVector2(room.RoomCorners[Direction.Down]);
                GridLocation doorPosition = CalculateGridLocationFromVector2(door.Key.transform.position);

                for (int j = 0; j < RoomBlueprint.DoorLocations.Length; j++)
                {
                    // for each door location, check if a building plot is available starting at the roomDoorPosition - bluePrintDoorPosition
                    GridLocation blueprintDoorPosition = RoomBlueprint.DoorLocations[j];

                    if ((doorPosition.UpRight == blueprintDoorPosition.UpRight) && (doorPosition.UpLeft == blueprintDoorPosition.UpLeft)) continue;
                    if (doorPosition.UpRight % 3 != blueprintDoorPosition.UpRight % 3 && doorPosition.UpLeft % 3 != blueprintDoorPosition.UpLeft % 3) continue;

                    GridLocation blueprintRoomStartPosition = new GridLocation(doorPosition.UpRight - blueprintDoorPosition.UpRight, -(doorPosition.UpLeft - blueprintDoorPosition.UpLeft));

                    Vector2 blueprintRoomStartPositionVector = CalculateLocationOnGrid((int)blueprintRoomStartPosition.UpRight, (int)blueprintRoomStartPosition.UpLeft);
                    if (GetPlotIsAvailable(selectedRoom, blueprintRoomStartPositionVector))
                    {
                        BuildModeContainer.Instance.CreateBuildingPlot(Room1BuildPlotPrefab, selectedRoom, blueprintRoomStartPositionVector);
                    }
                }
            }
        }

        //Initial room to avoid an empty map.
        if (RoomManager.Rooms.Count == 0)
        {
            BuildModeContainer.Instance.CreateBuildingPlot(Room1BuildPlotPrefab, selectedRoom, new Vector2(0, 0));
        }
    }

    public bool GetPlotIsAvailable(Room selectedRoomType, Vector2 existingRoomStartingPoint, int rightUpAxisLocationFromCurrentRoom, int leftUpAxisLocationFromCurrentRoom)
    {
        bool isAvailable = true;
        Vector2 plotLocationStartingPoint = CalculateLocationOnGrid(existingRoomStartingPoint, rightUpAxisLocationFromCurrentRoom, leftUpAxisLocationFromCurrentRoom);
        Vector2 point1 = CalculateLocationOnGrid(plotLocationStartingPoint, RoomBlueprint.RightUpAxisLength, 0);
        Vector2 point2 = CalculateLocationOnGrid(point1, 0, -RoomBlueprint.LeftUpAxisLength);
        Vector2 point3 = CalculateLocationOnGrid(point2, -RoomBlueprint.RightUpAxisLength, 0);

        List<BuildingTile> roomSquareTiles = BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= point3.x &&
            tile.StartingPoint.x <= point1.x &&
            tile.StartingPoint.y <= point2.y &&
            tile.StartingPoint.y >= plotLocationStartingPoint.y
        );

        for (int i = 3; i <= RoomBlueprint.RightUpAxisLength; i += 3)
        {
            for (int j = RoomBlueprint.LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = CalculateLocationOnGrid(plotLocationStartingPoint, i, -j);

                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                if (i == 0 || i == RoomBlueprint.RightUpAxisLength || j == 0 || j == RoomBlueprint.LeftUpAxisLength)
                {
                    // skip tiles that are at the edge because they may overlap with the walls of adjacent rooms
                    continue;
                }

                if (tile == null)
                {
                    Debug.LogError("Could not find tile at " + location);
                }

                if (!tile.IsAvailable)
                {
                    isAvailable = false;
                    break;
                }
            }
        }

        return isAvailable;
    }

    public bool GetPlotIsAvailable(Room selectedRoomType, Vector2 roomStartingPoint)
    {
        bool isAvailable = true;
        Vector2 plotLocationStartingPoint = CalculateLocationOnGrid(roomStartingPoint, 0, 0);
        Vector2 point1 = CalculateLocationOnGrid(plotLocationStartingPoint, RoomBlueprint.RightUpAxisLength, 0);
        Vector2 point2 = CalculateLocationOnGrid(point1, 0, -RoomBlueprint.LeftUpAxisLength);
        Vector2 point3 = CalculateLocationOnGrid(point2, -RoomBlueprint.RightUpAxisLength, 0);

        List<BuildingTile> roomSquareTiles = BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= point3.x &&
            tile.StartingPoint.x <= point1.x &&
            tile.StartingPoint.y <= point2.y &&
            tile.StartingPoint.y >= plotLocationStartingPoint.y
        );

        for (int i = 3; i <= RoomBlueprint.RightUpAxisLength; i += 3)
        {
            for (int j = RoomBlueprint.LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = CalculateLocationOnGrid(plotLocationStartingPoint, i, -j);

                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);

                if (i == 0 || i == RoomBlueprint.RightUpAxisLength || j == 0 || j == RoomBlueprint.LeftUpAxisLength)
                {
                    // skip tiles that are at the edge because they may overlap with the walls of adjacent rooms
                    continue;
                }

                if (tile == null)
                {
                    Debug.LogError("Could not find tile at " + location);
                }

                if (!tile.IsAvailable)
                {
                    isAvailable = false;
                    break;
                }
            }
        }

        return isAvailable;
    }

    public static GridLocation CalculateGridLocationFromVector2(Vector2 vectorPosition)
    {
        Vector3Int gridCoordinates = GameManager.Instance.WorldGrid.WorldToCell(vectorPosition);
        return new GridLocation(gridCoordinates.x + 13, gridCoordinates.y + 13);
    }

    public static Vector2 CalculateLocationOnGrid(int rightUpAxisPosition, int leftUpAxisPosition)
    {
        return new Vector2((rightUpAxisPosition + leftUpAxisPosition) * 5f, (rightUpAxisPosition - leftUpAxisPosition) * 2.5f);
    }

    public static Vector2 CalculateLocationOnGrid(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    {
        return new Vector2(startingPoint.x + (rightUpAxisLength + leftUpAxisLength) * 5f, startingPoint.y + (rightUpAxisLength - leftUpAxisLength) * 2.5f);
    }

    public static Vector2 CalculateColliderLocationOnGrid(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    {
        return new Vector2(startingPoint.x + (rightUpAxisLength + leftUpAxisLength) * 6f, startingPoint.y + (rightUpAxisLength - leftUpAxisLength) * 3f);
    }

    public void UpdateBuildingTiles(Room room)
    {
        // Get all building tiles in the location of the room and make them UNAVAILABLE
        List<BuildingTile> roomEdgeTiles = room.GetRoomEdgeTiles();

        // All tiles in the square of the room + the distance up to the fourth rank of tilse
        List<BuildingTile> surroundingSquareTiles = BuildingTiles.FindAll(tile =>
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

        SetMapPanMaximum(room.RoomCorners);

        IEnumerator coroutine = WaitAndUpdateGrid();
        StartCoroutine(coroutine);
    }

    public List<BuildingTile> CreateTileRing(List<BuildingTile> tileRing, List<BuildingTile> surroundingSquareTiles)
    {
        List<BuildingTile> newTiles = new List<BuildingTile>();
        for (int k = 0; k < tileRing.Count; k++)
        {
            //Debug.Log("tilering tile " + tileRing[k].StartingPoint);
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

    public BuildingTile CreateNeighbourTile(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength, List<BuildingTile> surroundingSquareTiles)
    {
        Vector2 neighbourLocation = CalculateLocationOnGrid(startingPoint, rightUpAxisLength, leftUpAxisLength);
        if (!BuildingTileLocations.Contains(neighbourLocation))
        {
            BuildingTile tile = BuildingTile.CreateBuildingTile(neighbourLocation, true);
            BuildingTiles.Add(tile);

            return tile;
        }

        BuildingTile existingTile = surroundingSquareTiles.Find(surroundingSquareTile => surroundingSquareTile.StartingPoint == neighbourLocation);
            
        //Some tiles at the direct edge may already exist, but the tile one row further still needs to be created. Therefore, also add this tile to the list.
        if(existingTile != null && existingTile.IsAvailable) {
            return existingTile;
        }
        return null;
    }

    public void SetMapPanMaximum(Dictionary<Direction, Vector2> newRoomCorners)
    {
        float currentMaxY = CameraController.PanLimits[Direction.Up];
        float currentMaxX = CameraController.PanLimits[Direction.Right];
        float currentMinY = CameraController.PanLimits[Direction.Down];
        float currentMinX = CameraController.PanLimits[Direction.Left];
        float panPadding = 20f;

        if (currentMaxY <= newRoomCorners[Direction.Up].y + panPadding)
            CameraController.PanLimits[Direction.Up] = newRoomCorners[Direction.Up].y + panPadding;
        if (currentMaxX <= newRoomCorners[Direction.Right].x + panPadding)
            CameraController.PanLimits[Direction.Right] = newRoomCorners[Direction.Right].x + panPadding;
        if (currentMinY >= newRoomCorners[Direction.Down].y - panPadding)
            CameraController.PanLimits[Direction.Down] = newRoomCorners[Direction.Down].y - panPadding;
        if (currentMinX >= newRoomCorners[Direction.Left].x - panPadding)
            CameraController.PanLimits[Direction.Left] = newRoomCorners[Direction.Left].x - panPadding;

    }

    public void DrawBuildingTilesGizmos()
    {
        // Draw available/unavailable building tiles
        for (int i = 0; i < BuildingTiles.Count; i++)
        {
            Vector2 startingPoint = BuildingTiles[i].StartingPoint;
            if(BuildingTiles[i].IsAvailable)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawCube(startingPoint, new Vector3(1, 1));
        }
    }

    public IEnumerator WaitAndUpdateGrid()
    {
        yield return new WaitForSeconds(0.01f);
        GameManager.Instance.PathfindingGrid.CreateGrid();  // May have to change to partly recreating the grid.
    }

    public void DrawDoorLocationGizmos()
    {
        for (int i = 0; i < RoomManager.Rooms.Count; i++)
        {
            Room room = RoomManager.Rooms[i];

            foreach (KeyValuePair<Door, bool> door in room.Doors)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(door.Key.transform.position, new Vector3(1.5f, 1.5f));

            }
        }
    }
}
