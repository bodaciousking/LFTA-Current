using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public Vector3Int gridPosition;

    public int gCost;  //Distance from starting node
    public int hCost; // distance from end node
    public int aCost; // distance from last walkable node

    public int gridX, gridY, gridZ;

    public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ) 
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
        gridPosition = new Vector3Int(gridX, gridY, gridZ);
    }

    public int fCost // gCost + hCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
