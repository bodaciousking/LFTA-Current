using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    public static Pathfinding instance;
    UnitSelection uS;
    public GameObject moveObject, attackObject;
    Combat combat;

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

    public void StartFindMoveRange(Vector3 startPos, Unit requestingUnit)
    {
        StartCoroutine(FindMoveRange(startPos, requestingUnit));
    }

    public void StartFindAttackRange(Vector3 startPos, Unit requestingUnit)
    {
        StartCoroutine(FindMoveRange(startPos, requestingUnit));
    }

    public void StartFindLineRange(Ability projectile, Unit castingUnit)
    {
        StartCoroutine(FindLineRange(projectile, castingUnit));
    }

    public void StartFindAoERange(Ability ability, Unit castingUnit)
    {
        StartCoroutine(FindAoERange(ability, castingUnit));
    }

    public void StartFindMultiTargetLineRange(Ability ability, Unit castingUnit)
    {
        StartCoroutine(FindMultiTargetLineRange(ability, castingUnit));
    }

    public void StartFindGCPath(Vector3 start, Vector3 end)
    {
        StartCoroutine(FindGCPath(start, end));
    }

    public IEnumerator FindMoveRange(Vector3 startPos, Unit movingUnit)
    {
        int range = movingUnit.moveRange;
        if (movingUnit.pendingMove)
            range = 0;
        int attackRange = movingUnit.attackRange;
        int unitTeam = movingUnit.team;
        int jumpRange = movingUnit.jumpRange;
        List<Node> nodesInRange = new List<Node>();
        List<Node> nodesInAttackRange = new List<Node>();

        int gCostRange = range * 10;
        int aCostRange = attackRange * 10;

        bool filledAll = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);

        if (startNode.walkable)
        {
            Queue<Node> moveSet = new Queue<Node>(grid.MaxSize); // the set of movement nodes to be evaluated
            Queue<Node> atkSet = new Queue<Node>(grid.MaxSize); // the set of attackable nodes to be evaluated
            HashSet<Node> closedSet = new HashSet<Node>(); // the set of nodes already evaluated
            moveSet.Enqueue(startNode);

            while (moveSet.Count > 0 || atkSet.Count > 0)
            {
                Node node;
                bool atkNode = false;
                if (moveSet.Count > 0)
                {
                    node = moveSet.Dequeue();
                    if (node.gCost <= gCostRange)
                    {
                        if (!grid.TileFromNode(node).occupied && node != startNode && node.walkable)
                        {
                            nodesInRange.Add(node);
                        }
                    }

                }
                else
                {
                    node = atkSet.Dequeue();
                    atkNode = true;
                    if (node.aCost <= aCostRange && !nodesInRange.Contains(node) && node != startNode && node.walkable)
                    {
                        nodesInAttackRange.Add(node);
                    }
                }

                closedSet.Add(node);

                Tile tileScript = grid.TileFromNode(node);
                if (tileScript.occupyingUnit)
                {
                    Unit occupyingUnit = tileScript.occupyingUnit;
                    if (occupyingUnit.team != unitTeam)
                    {
                        tileScript.attackable = true;
                        tileScript.inMoveRange = false;

                        if (nodesInRange.Contains(node))
                            continue;
                    }
                }

                foreach (Node neighbour in grid.GetNeighbours(node, 1, jumpRange))
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    if (grid.TileFromNode(neighbour))
                    {
                        Tile neighbourTile = grid.TileFromNode(neighbour);

                        int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                        int newAtkCostToNeighbour = node.aCost + GetDistance(node, neighbour);

                        if (newCostToNeighbour <= neighbour.gCost || !moveSet.Contains(neighbour) || newAtkCostToNeighbour <= neighbour.aCost)
                        {
                            neighbour.gCost = newCostToNeighbour;
                            grid.TileFromNode(neighbour).gCost = neighbour.gCost;

                            neighbour.parent = node;

                            if (!moveSet.Contains(neighbour) && newCostToNeighbour <= gCostRange && !neighbourTile.occupied
                                && neighbour.walkable && !atkNode)
                            {
                                // We can move there
                                neighbour.aCost = 0;
                                neighbourTile.aCost = 0;
                                moveSet.Enqueue(neighbour);
                            }
                            else
                            {
                                if (!atkSet.Contains(neighbour) && !moveSet.Contains(neighbour))
                                {
                                    neighbour.aCost = newAtkCostToNeighbour;
                                    neighbourTile.aCost = newAtkCostToNeighbour;
                                }
                                if (!atkSet.Contains(neighbour) && newAtkCostToNeighbour <= aCostRange && !neighbourTile.inMoveRange && node.walkable)
                                {
                                    // We can attack there
                                    atkSet.Enqueue(neighbour);
                                }

                            }
                        }
                    }

                }

                if (moveSet.Count == 0 && atkSet.Count == 0)
                {
                    filledAll = true;
                    break;
                }
            }
        }

        yield return null;
        if (filledAll == true)
        {
            MoveRangeHandler(nodesInRange);
            if (nodesInAttackRange.Count > 0)
            {
                AttackRangeHandler(nodesInAttackRange);
            }
        }
    }

    public IEnumerator OldFindLineRange(Ability ability, Unit castingUnit)
    {
        Node unitNode = grid.NodeFromWorldPoint(castingUnit.transform.position);
        List<Node> nodesInRange = grid.GetNeighbours(unitNode, ability.range, ability.vRange); // This needs to be changed to a pathfinder
        List<Node> nodesInAbilityRange = nodesInRange;
        yield return null;

        if (nodesInAbilityRange.Count > 0)
        {
            AbilityRangeHandler(nodesInAbilityRange);
        }
    }

    public IEnumerator FindLineRange(Ability ability, Unit castingUnit)
    {
        int range = ability.range;
        int unitTeam = castingUnit.team;
        int vRange = ability.vRange;
        List<Node> nodesInAbilityRange = new List<Node>();

        int gCostRange = range * 10;

        bool filledAll = false;

        Node startNode = grid.NodeFromWorldPoint(castingUnit.transform.position);

        if (startNode.walkable)
        {
            Queue<Node> openSet = new Queue<Node>(grid.MaxSize); // the set of movement nodes to be evaluated
            HashSet<Node> closedSet = new HashSet<Node>(); // the set of nodes already evaluated
            openSet.Enqueue(startNode);

            while (openSet.Count > 0)
            {
                Node node;
                node = openSet.Dequeue();
                if (node.gCost <= gCostRange)
                {
                    if (!grid.TileFromNode(node).occupied 
                        && node != startNode
                        && node.walkable)
                    {
                        nodesInAbilityRange.Add(node);
                    }
                }

                closedSet.Add(node);


                foreach (Node neighbour in grid.GetNeighbours(node, 1, ability.vRange))
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    if (grid.TileFromNode(neighbour))
                    {
                        Tile neighbourTile = grid.TileFromNode(neighbour);

                        int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);

                        if (newCostToNeighbour <= neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newCostToNeighbour;
                            grid.TileFromNode(neighbour).gCost = neighbour.gCost;

                            neighbour.parent = node;

                            if (!openSet.Contains(neighbour) && newCostToNeighbour <= gCostRange && !neighbourTile.occupied
                                && neighbour.walkable && (neighbour.gridX == startNode.gridX || neighbour.gridZ == startNode.gridZ))
                            {
                                // it's an empty tile in range
                                openSet.Enqueue(neighbour);
                            }
                            else // 
                            {
                                if (!openSet.Contains(neighbour) && neighbourTile.occupied && (neighbour.gridX == startNode.gridX || neighbour.gridZ == startNode.gridZ))
                                {
                                    if(neighbourTile.occupyingUnit.team != castingUnit.team)
                                    {
                                        nodesInAbilityRange.Add(neighbour);
                                        continue;
                                    }
                                }

                            }
                        }
                    }

                }

                if (openSet.Count == 0 )
                {
                    filledAll = true;
                    break;
                }
            }
        }

        yield return null;
        if (filledAll == true)
        {
            AbilityRangeHandler(nodesInAbilityRange);
        }
    }

    public IEnumerator FindAoERange(Ability ability, Unit castingUnit)
    {
        uS.AoEtargetting = true;
        int abilityRange = ability.range;
        Node unitNode = grid.NodeFromWorldPoint(castingUnit.transform.position);
        List<Node> nodesInAbilityRange = new List<Node>();
        //Simply take all the nodes in range of the AoE and light em up red.The targetable nodes will be InAbilityRange.
        if (ability.name == "Explosion") //this ability can only target the casting unit's position.
        {
            nodesInAbilityRange.Add(unitNode);
            DisplayBlastRadius(combat.castedAbility, grid.TileFromNode(unitNode));
        }
        yield return null;
        AbilityRangeHandler(nodesInAbilityRange);
    }

    public IEnumerator FindMultiTargetLineRange(Ability ability, Unit castingUnit)
    {
        int abilityRange = ability.range - 1;
        Node unitNode = grid.NodeFromWorldPoint(castingUnit.transform.position);
        List<Node> nodesInRange = new List<Node>();
        List<Node> nodesInAbilityRange = new List<Node>();
        nodesInRange = grid.GetNeighbours(unitNode, abilityRange, ability.vRange);

        yield return null;
        if (nodesInAbilityRange.Count > 0)
        {
            AbilityRangeHandler(nodesInAbilityRange);
            foreach (Node node in nodesInAbilityRange)
            {
                nodesInRange.Remove(node);
            }
        }
        MoveRangeHandler(nodesInRange);
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
        combat.GetKneeTargets(waypoints);
        combat.GetVector3LZ(waypoints);
    }

    public void DisplayBlastRadius(Ability ability, Tile targetTile)
    {
        int blastRadius = ability.radius;
        Node targetNode = grid.NodeFromWorldPoint(targetTile.transform.position);
        List<Node> blastZoneNodes = grid.Get8Neighbours(targetNode, blastRadius);

        string displayHolderName = "BlastZoneSquares";
        if (GameObject.Find(displayHolderName))
        {
            DestroyImmediate(GameObject.Find(displayHolderName).gameObject);
        }
        Transform displayHolder = new GameObject(displayHolderName).transform;

        combat.targetList.Clear();
        combat.tileTargetList.Clear();
        foreach (Node blastNode in blastZoneNodes)
        {
            Tile blastTile;
            if (grid.TileFromNode(blastNode))
            {
                blastTile = grid.TileFromNode(blastNode);

                Transform displaySq = Instantiate(attackObject, new Vector3(blastTile.transform.position.x, blastTile.transform.position.y + blastTile.graphicsYOffset, blastTile.transform.position.z), transform.rotation).transform;
                displaySq.parent = displayHolder;
                displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
                blastTile.inBlastZone = true;
                blastTile.validAbilityTarget = true;
                if (blastTile.occupied)
                {
                    combat.targetList.Add(blastTile.occupyingUnit);
                }
                combat.tileTargetList.Add(blastTile);
            }
            else continue;
        }
    }

    public void MoveRangeHandler(List<Node> nodesInRange)
    {
        string displayHolderName = "DisplaySquares";
        if (transform.Find(displayHolderName))
        {
            DestroyImmediate(transform.Find(displayHolderName).gameObject);
        }
        Transform displayHolder = new GameObject(displayHolderName).transform;

        foreach (Node item in nodesInRange)
        {
            targetNormal = Vector3.zero;
            Tile nowTile = grid.TileFromNode(item);
            if (Physics.Raycast(nowTile.transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out hit, 1f, layerMask))
            {
                if (hit.normal == transform.up)
                {
                    toRotation = new Quaternion(0, 0, 1, 0);
                }
                else if (hit.normal != targetNormal)
                {
                    targetNormal = hit.normal;
                    toRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
            }

            Transform displaySq = Instantiate(moveObject, nowTile.transform.position + Vector3.up * 0.02f + new Vector3(0f, nowTile.graphicsYOffset, 0f), toRotation).transform;
            displaySq.parent = displayHolder;
            displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
            nowTile.inMoveRange = true;
        }
    }

    public void AbilityRangeHandler(List<Node> nodesInRange)
    {
        string displayHolderName = "AbilityDisplaySquares";
        if (transform.Find(displayHolderName))
        {
            DestroyImmediate(transform.Find(displayHolderName).gameObject);
        }
        Transform displayHolder = new GameObject(displayHolderName).transform;

        foreach (Node item in nodesInRange)
        {
            Tile nowTile = grid.TileFromNode(item);
            if (nowTile)
            {
                if (nowTile.occupied)
                    if (nowTile.occupyingUnit.team != combat.attacker.team)
                    {
                        nowTile.validAbilityTarget = true;
                        Transform attackSq = Instantiate(attackObject, item.worldPosition + new Vector3(0f, nowTile.graphicsYOffset, 0f), transform.rotation).transform;
                        attackSq.parent = displayHolder;
                        attackSq.localScale = new Vector3(attackSq.localScale.x * 0.9f, attackSq.localScale.y, attackSq.localScale.z * 0.9f);
                    }

                if (!nowTile.validAbilityTarget)
                {
                    Transform displaySq = Instantiate(moveObject, item.worldPosition + new Vector3(0f, nowTile.graphicsYOffset, 0f), transform.rotation).transform;
                    displaySq.parent = displayHolder;
                    displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
                }
            }
        }
    }

    public void AttackRangeHandler(List<Node> nodesInRange)
    {
        string displayHolderName = "AttackDisplaySquares";
        if (transform.Find(displayHolderName))
        {
            DestroyImmediate(transform.Find(displayHolderName).gameObject);
        }
        Transform displayHolder = new GameObject(displayHolderName).transform;

        foreach (Node item in nodesInRange)
        {
            Tile nowTile = grid.TileFromNode(item);

            Transform displaySq = Instantiate(attackObject, nowTile.transform.position + Vector3.up * 0.02f + new Vector3(0f, nowTile.graphicsYOffset, 0f), transform.rotation).transform;
            displaySq.parent = displayHolder;
            displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
            nowTile.inAttackRange = true;
        }
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
