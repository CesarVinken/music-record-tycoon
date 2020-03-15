using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public RoomObjectBlueprint RoomObjectBlueprint;
    public PolygonCollider2D Collider;
    public ObjectRotation RoomObjectRotation;
    public Vector2 LocationInRoom; // needed to calculate location in rotated room
    public Room Room;

    //public void Setup(string name)
    //{

    //}
}
