using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int RightUpAxisLength = 9;
    public int LeftUpAxisLength = 6;   //LATER not hardcoded but derived from room specifics in database or something.

    private List<BuildingTile> _roomEdgeTiles = new List<BuildingTile>();

    public Dictionary<RoomCorner, Vector2> RoomCorners;

    public void SetupCorners(Dictionary<RoomCorner, Vector2> roomCorners)
    {
        if(RightUpAxisLength % 3 != 0 || LeftUpAxisLength % 3 != 0)
        {
            Debug.LogError($"RightUpAxisLength ({RightUpAxisLength}) and LeftUpAxisLength ({LeftUpAxisLength}) of room should always be divisible by 3");
        }

        if (roomCorners.Count < 4)
        {
            Debug.LogError("There should be 4 roomCorners for this room");
        }

        RoomCorners = roomCorners;
    }

    public List<BuildingTile> GetRoomEdgeTiles()
    {
        Debug.Log("Room" + RoomCorners[RoomCorner.Bottom]);

        // Get all building tiles in the location of the room and make them UNAVAILABLE
        List<BuildingTile> roomSquareTiles = BuilderManager.Instance.BuildingTiles.FindAll(tile =>
            tile.StartingPoint.x >= RoomCorners[RoomCorner.Left].x &&
            tile.StartingPoint.x <= RoomCorners[RoomCorner.Right].x &&
            tile.StartingPoint.y <= RoomCorners[RoomCorner.Top].y &&
            tile.StartingPoint.y >= RoomCorners[RoomCorner.Bottom].y
        );

        for (int i = 0; i <= RightUpAxisLength; i += 3)
        {
            for (int j = LeftUpAxisLength; j >= 0; j -= 3)
            {
                Vector2 location = BuilderManager.CalculateLocationOnGrid(RoomCorners[RoomCorner.Bottom], i, -j);
                BuildingTile tile = roomSquareTiles.FirstOrDefault(t => t.StartingPoint == location);
                tile.IsAvailable = false;
                if (i == 0 || i == RightUpAxisLength || j == 0 || j == LeftUpAxisLength)
                    _roomEdgeTiles.Add(tile);
            }
        }

        return _roomEdgeTiles;
    }
}

public enum RoomCorner
{
    Top, Right, Bottom, Left
}