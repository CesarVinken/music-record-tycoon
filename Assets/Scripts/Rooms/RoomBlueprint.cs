using System;
using UnityEngine;

public class RoomBlueprint
{
    // CONTAINS STATIC VALUES OF A ROOM. INCLUDING PROPORTIONS AND DOOR LOCATIONS. ROOM BLUEPRINT SHOULD LATER BE PARENT CLASS FOR SPECIFIC DOORS, OR TAKE INFO FROM DATABASE
    public static int RightUpAxisLength = 9;
    public static int LeftUpAxisLength = 6;

    public static GridLocation[] DoorLocations = new GridLocation[]
    {
        new GridLocation(3, 0),
        new GridLocation(3, 6),
        new GridLocation(0, 3),
        new GridLocation(9, 5),
        new GridLocation(0, 2),
    };

}

public class GridLocation
{
    public float UpRight;
    public float UpLeft;

    public GridLocation(float upRight, float upLeft)
    {
        UpRight = upRight;    
        UpLeft = upLeft;
    }
}