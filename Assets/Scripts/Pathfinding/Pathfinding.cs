using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.UI.Image;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    public static Pathfinding instance;
    UnitSelection uS;
    Combat combat;
    public GameObject moveObject, attackObject;

    RaycastHit hit;
    int layerMask = 1 << 11;
    private Vector3 targetNormal;
    Quaternion toRotation = new Quaternion(0, 0, 1, 0);

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.Log("Too many pathfinders!");
            return;
        }
        instance = this;

    }

    private void Start()
    {
        grid = GetComponent<Grid>();
        combat = Combat.instance;
        uS = GetComponentInParent<UnitSelection>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos, Unit requestingUnit)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node endNode = grid.NodeFromWorldPoint(targetPos);
        Tile startTile = grid.TileFromNode(startNode);
        Tile endTile = grid.TileFromNode(endNode);
        Unit startUnit = startTile.occupyingUnit;
        Unit endUnit = null;
        if (endTile.occupied)
        {
            endUnit = endTile.occupyingUnit;
        }

        if (endUnit != null) // assume we are attacking the unit
        {
            int attackDistance = GetDistance(startNode, endNode);
            if (attackDistance <= startUnit.attackRange * 10) // if we're already in range of the unit, no need to find a path.
                                                              // We just have to play the correct animation based on distance.
            {
                if (attackDistance == 10) //Play the melee animation.
                {
                    startUnit.enemyUnit = endUnit;
                    startUnit.StartAttacking();
                    startUnit.ActionsDone();
                }
                else if (attackDistance > 10) // Play the ranged animation.
                {
                    startUnit.enemyUnit = endUnit;
                    startUnit.StartRangedAttack();
                    startUnit.ActionsDone();
                }
                return;
            }
            else // otherwise we're too far from the unit so we need to attack move.
                StartCoroutine(AttackMove(startPos, targetPos, requestingUnit));
        }
        else // if there's no unit on the end tile, we need to path and move as normal.
            StartCoroutine(FindPath(startPos, targetPos, requestingUnit));

    }

    public void StartFindGCPath(Vector3 start, Vector3 end)
    {
        StartCoroutine(FindGCPath(start, end));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, Unit requestingUnit)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode, 1, requestingUnit.jumpRange))
                {
                    if (grid.TileFromNode(neighbour))
                    {
                        Tile neighbourTile = grid.TileFromNode(neighbour);
                        if (!neighbour.walkable || closedSet.Contains(neighbour) || neighbourTile.blocked)
                            continue;
                        if (neighbourTile.occupied && neighbour != targetNode)
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add((neighbour));
                            }
                            else openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = ReversePath(startNode, targetNode);
        }
        requestingUnit.OnPathFound(waypoints, true);
    }

    IEnumerator AttackMove(Vector3 startPos, Vector3 targetPos, Unit requestingUnit)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        int unitAtkRange = requestingUnit.attackRange * 10;

        Node startNode = grid.NodeFromWorldPoint(targetPos);
        Node targetNode = grid.NodeFromWorldPoint(startPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);
            startNode.gCost = 0;

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode, 1, requestingUnit.jumpRange))
                {
                    if (grid.TileFromNode(neighbour))
                    {
                        if (!closedSet.Contains(neighbour))
                        {//
                            int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                            Tile neighbourTile = grid.TileFromNode(neighbour);
                            if (newMovementCostToNeighbour > unitAtkRange - 10)
                            {
                                if (!neighbour.walkable)
                                    continue;
                                if (neighbourTile.occupied && neighbour != targetNode)
                                {
                                    continue;
                                }
                            }
                            else UnityEngine.Debug.Log("D");

                            if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                            {
                                neighbour.gCost = newMovementCostToNeighbour;
                                neighbour.hCost = GetDistance(neighbour, targetNode);
                                neighbour.parent = currentNode;

                                if (!openSet.Contains(neighbour))
                                {
                                    openSet.Add((neighbour));
                                }
                                else openSet.UpdateItem(neighbour);
                            }
                        }
                        else continue;
                    }
                    else continue;
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        requestingUnit.OnPathFound(waypoints, true);
    }
    
    public IEnumerator FindGCPath(Vector3 startPos, Vector3 endPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(endPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode, 1, 1))
                {
                    if (grid.TileFromNode(neighbour))
                    {
                        Tile neighbourTile = grid.TileFromNode(neighbour);
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                            continue;
                        if (neighbourTile.occupied && neighbour != targetNode)
                            continue;
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add((neighbour));
                            }
                            else openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = ReversePath(startNode, targetNode);
        }
        //combat.GetKneeTargets(waypoints);
        combat.GetVector3LZ(waypoints);
    }

    Vector3[] ReversePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        Vector3[] waypoints = SimplifyPath(path);
        //Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            waypoints.Add(path[i].worldPosition);
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        return (dstY + dstX + dstZ) * 10;
    }

}
