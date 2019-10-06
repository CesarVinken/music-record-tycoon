﻿using UnityEngine;

public class Node : IHeapItem <Node>
{
    public bool Walkable;
    public Vector3 WorldPosition;
    public int GridX;
    public int GridY;
    public int MovementPenalty;

    public int GCost;
    public int HCost;
    public Node Parent;
    private int _heapIndex;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int penalty)
    {
        if (!walkable)
            Debug.Log("this node is not walkable ");

        Walkable = walkable;
        WorldPosition = worldPosition;
        GridX = gridX;
        GridY = gridY;
        MovementPenalty = penalty;
    }

    public int FCost
    {
        get {
            return GCost + HCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return _heapIndex;
        }
        set
        {
            _heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return -compare;
    }
}
