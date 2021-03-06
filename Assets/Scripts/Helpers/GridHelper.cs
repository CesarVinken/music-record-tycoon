﻿using UnityEngine;

public static class GridHelper
{
    public static GridLocation VectorToGridLocation(Vector2 vectorPosition)
    {
        // Make sure that door positions are exactly on the correct worldVector locations, otherwise the vectorPosition may be interpreted as being part of the wrong grid coordinate.
        Vector3Int gridCoordinates = GameManager.Instance.WorldGrid.WorldToCell(vectorPosition);
        return new GridLocation(gridCoordinates.x, gridCoordinates.y);
    }

    public static Vector2 GridToVectorLocation(int rightUpAxisPosition, int leftUpAxisPosition)
    {
        return new Vector2((rightUpAxisPosition - leftUpAxisPosition) * 5f, (rightUpAxisPosition + leftUpAxisPosition) * 2.5f);
    }

    public static Vector2 GridToVectorLocation(GridLocation gridLocation)
    {
        return new Vector2((gridLocation.UpRight - gridLocation.UpLeft) * 5f, (gridLocation.UpRight + gridLocation.UpLeft) * 2.5f);
    }

    public static Vector2 GridToVectorLocation(Vector2 startingPoint, int rightUpAxisLength, int leftUpAxisLength)
    {
        return new Vector2(startingPoint.x + (rightUpAxisLength + leftUpAxisLength) * 5f, startingPoint.y + (rightUpAxisLength - leftUpAxisLength) * 2.5f);
    }

    public static GridLocation FindClosestGridTile(GridLocation originalGridLocation)
    {
        float gridUpRight = originalGridLocation.UpRight;
        float gridUpLeft = originalGridLocation.UpLeft;
        if (originalGridLocation.UpRight > 0)
        {
            gridUpRight = originalGridLocation.UpRight - (originalGridLocation.UpRight % 3);
        }
        else if (originalGridLocation.UpRight < 0)
        {
            gridUpRight = originalGridLocation.UpRight - (3 - ((originalGridLocation.UpRight % 3) * -1));
        }

        if (originalGridLocation.UpLeft > 0)
        {
            gridUpLeft = originalGridLocation.UpLeft - (originalGridLocation.UpLeft % 3);
        }
        else if (originalGridLocation.UpLeft < 0)
        {
            gridUpLeft = originalGridLocation.UpLeft - (3 - ((originalGridLocation.UpLeft % 3) * -1));
        }

        return new GridLocation(gridUpRight, gridUpLeft);
    }
}

public struct GridLocation
{
    public float UpRight;
    public float UpLeft;

    public GridLocation(float upRight, float upLeft)
    {
        UpRight = upRight;
        UpLeft = upLeft;
    }
}