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

    public void StartFindRange(Vector3 startPos, int range, int attackrange, int team)
    {
        StartCoroutine(FindMoveRange(startPos, range, attackrange, team));
    }

    public void StartFindAttackRange(Vector3 startPos, int attackRange, int team)
    {
        StartCoroutine(FindMoveRange(startPos, 0, attackRange, team));
    }

    public void StartFindProjectileRange(Ability projectile, Unit castingUnit)
    {
        //StartCoroutine(FindProjectileRange(projectile, castingUnit));
    }
    public void StartFindAoERange(Ability ability, Unit castingUnit)
    {
        StartCoroutine(FindAoERange(ability, castingUnit));
    }

    public void StartFindMultiTargetLineRange(Ability ability, Unit castingUnit)
    {
        //StartCoroutine(FindMultiLineRange(ability, castingUnit));
    }

    public IEnumerator FindMoveRange(Vector3 startPos, int range, int attackRange, int unitTeam)
    {
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
                        if (!grid.TileFromNode(node).occupied && node != startNode)
                        {
                            nodesInRange.Add(node);
                        }
                    }

                }
                else
                {
                    node = atkSet.Dequeue();
                    atkNode = true;
                    if (node.aCost <= aCostRange && !nodesInRange.Contains(node) && node != startNode)
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
                        tileScript.inRange = false;

                        if (nodesInRange.Contains(node))
                            continue;
                    }
                }

                foreach (Node neighbour in grid.GetNeighbours(node, 1))
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
                                if (!atkSet.Contains(neighbour) && newAtkCostToNeighbour <= aCostRange && !neighbourTile.inRange)
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
            RangeHandler(nodesInRange);
            if (nodesInAttackRange.Count > 0)
            {
                AttackRangeHandler(nodesInAttackRange);
            }
        }
    }

    //public IEnumerator FindProjectileRange(Ability projectileAbility, Unit castingUnit)
    //{
    //    int abilityRange = projectileAbility.range - 1;
    //    Node unitNode = grid.NodeFromWorldPoint(castingUnit.transform.position);
    //    List<Node> nodesInRange = new List<Node>();
    //    List<Node> nodesInAbilityRange = new List<Node>();
    //    bool done = false;
    //    int directionsChecked = 0;

    //    for (int i = 0; i <= 3; i++) // For each of the cardinal directions,
    //    {
    //        for (int j = 0; j <= abilityRange; j++) // get a path abilityRange length in that direction.
    //        {
    //            if (i == 0) //North
    //            {
    //                int checkY = unitNode.gridY + j + 1;

    //                if (checkY >= 0 && checkY < grid.gridSizeY)
    //                {
    //                    Vector2 checkNodePos = new Vector2(unitNode.gridX, checkY);
    //                    if (!grid.grid[unitNode.gridX, checkY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        directionsChecked++;
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                        }
    //                        break;
    //                    }
    //                }
    //            }
    //            if (i == 1) //East
    //            {
    //                int checkX = unitNode.gridX + j + 1;
    //                if (checkX >= 0 && checkX < grid.gridSizeX)
    //                {
    //                    Vector2 checkNodePos = new Vector2(checkX, unitNode.gridY);
    //                    if (!grid.grid[checkX, unitNode.gridY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        directionsChecked++;
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            nodesInAbilityRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                        }
    //                        break;
    //                    }
    //                }
    //            }
    //            if (i == 2) //South
    //            {
    //                int checkY = unitNode.gridY - j - 1;
    //                if (checkY >= 0 && checkY < grid.gridSizeY)
    //                {
    //                    Vector2 checkNodePos = new Vector2(unitNode.gridX, checkY);
    //                    if (!grid.grid[unitNode.gridX, checkY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        directionsChecked++;
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                        }
    //                        break;
    //                    }
    //                }
    //            }
    //            if (i == 3) //West
    //            {
    //                int checkX = unitNode.gridX - j - 1;
    //                if (checkX >= 0 && checkX < grid.gridSizeX)
    //                {
    //                    Vector2 checkNodePos = new Vector2(checkX, unitNode.gridY);
    //                    if (!grid.grid[checkX, unitNode.gridY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        directionsChecked++;
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            nodesInAbilityRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                        }
    //                        break;
    //                    }
    //                }
    //            }

    //            if (j == abilityRange)
    //                directionsChecked++;
    //        }

    //        if (directionsChecked == 4)
    //            done = true;
    //    }
    //    yield return null;
    //    if (done == true)
    //    {
    //        if (nodesInAbilityRange.Count > 0)
    //        {
    //            AbilityRangeHandler(nodesInAbilityRange);
    //            foreach (Node node in nodesInAbilityRange)
    //            {
    //                nodesInRange.Remove(node);
    //            }
    //        }
    //        RangeHandler(nodesInRange);
    //    }
    //}

    public IEnumerator FindAoERange(Ability ability, Unit castingUnit)
    {
        uS.AoEtargetting = true;
        int abilityRange = ability.range;
        Node unitNode = grid.NodeFromWorldPoint(castingUnit.transform.position);
        List<Node> nodesInAbilityRange = new List<Node>();
        // Simply take all the nodes in range of the AoE and light em up red. The targetable nodes will be InAbilityRange.
        if (ability.name == "Explosion") //this ability can only target the casting unit's position.
        {
            nodesInAbilityRange.Add(unitNode);
            DisplayBlastRadius(combat.castedAbility, grid.TileFromNode(unitNode));
        }
        yield return null;
        AbilityRangeHandler(nodesInAbilityRange);
    }

    //public IEnumerator FindMultiLineRange(Ability ability, Unit castingUnit)
    //{
    //    int abilityRange = ability.range - 1;
    //    Node unitNode = grid.NodeFromWorldPoint(castingUnit.transform.position);
    //    List<Node> nodesInRange = new List<Node>();
    //    List<Node> nodesInAbilityRange = new List<Node>();
    //    bool done = false;
    //    int directionsChecked = 0;

    //    for (int i = 0; i <= 3; i++) // For each of the cardinal directions,
    //    {
    //        for (int j = 0; j <= abilityRange; j++) // get a path n = abilityRange length in that direction.
    //        {
    //            if (i == 0) //North
    //            {
    //                int checkY = unitNode.gridY + j + 1;

    //                if (checkY >= 0 && checkY < grid.gridSizeY)
    //                {
    //                    Vector2 checkNodePos = new Vector2(unitNode.gridX, checkY);
    //                    if (!grid.grid[unitNode.gridX, checkY].walkable)
    //                    {   // Obstacle found
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        //directionsChecked++;
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            //We need to check if there's a free space adjacent to this unit.
    //                            if (checkY - 1 > 0)
    //                                if (grid.grid[unitNode.gridX, checkY - 1].walkable && !grid.TileFromVector2(new Vector2(unitNode.gridX, checkY - 1)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[unitNode.gridX, checkY - 1]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY - 1]);
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                                    }

    //                                }
    //                            if (grid.gridSizeY > checkY + 1)
    //                                if (grid.grid[unitNode.gridX, checkY + 1].walkable && !grid.TileFromVector2(new Vector2(unitNode.gridX, checkY + 1)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[unitNode.gridX, checkY + 1]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY + 1]);
    //                                    }
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[unitNode.gridX, checkY + 1]))
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                                }
    //                        }
    //                        //break;
    //                    }
    //                }
    //            }
    //            if (i == 1) //East
    //            {
    //                int checkX = unitNode.gridX + j + 1;
    //                if (checkX >= 0 && checkX < grid.gridSizeX)
    //                {
    //                    Vector2 checkNodePos = new Vector2(checkX, unitNode.gridY);
    //                    if (!grid.grid[checkX, unitNode.gridY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {   //Enemy unit found
    //                            //We need to check if there's a free space adjacent to this unit.
    //                            if (checkX - 1 > 0)
    //                                if (grid.grid[checkX - 1, unitNode.gridY].walkable && !grid.TileFromVector2(new Vector2(checkX - 1, unitNode.gridY)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[checkX - 1, unitNode.gridY]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[checkX - 1, unitNode.gridY]);
    //                                        nodesInAbilityRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                                    }

    //                                }
    //                            if (grid.gridSizeX > checkX + 1)
    //                                if (grid.grid[checkX + 1, unitNode.gridY].walkable && !grid.TileFromVector2(new Vector2(checkX + 1, unitNode.gridY)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[checkX + 1, unitNode.gridY]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[checkX + 1, unitNode.gridY]);
    //                                    }
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[checkX, unitNode.gridY]))
    //                                        nodesInAbilityRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                                }
    //                        }
    //                    }
    //                }
    //            }
    //            if (i == 2) //South
    //            {
    //                int checkY = unitNode.gridY - j - 1;
    //                if (checkY >= 0 && checkY < grid.gridSizeY)
    //                {
    //                    Vector2 checkNodePos = new Vector2(unitNode.gridX, checkY);
    //                    if (!grid.grid[unitNode.gridX, checkY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            //We need to check if there's a free space adjacent to this unit.
    //                            if (checkY - 1 > 0)
    //                                if (grid.grid[unitNode.gridX, checkY - 1].walkable && !grid.TileFromVector2(new Vector2(unitNode.gridX, checkY - 1)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[unitNode.gridX, checkY - 1]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY - 1]);
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                                    }
    //                                }
    //                            if (checkY + 1 < grid.gridSizeY)
    //                                if (grid.grid[unitNode.gridX, checkY + 1].walkable && !grid.TileFromVector2(new Vector2(unitNode.gridX, checkY + 1)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[unitNode.gridX, checkY + 1]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY + 1]);
    //                                    }
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[unitNode.gridX, checkY]))
    //                                        nodesInAbilityRange.Add(grid.grid[unitNode.gridX, checkY]);
    //                                }
    //                        }
    //                    }
    //                }
    //            }
    //            if (i == 3) //West
    //            {
    //                int checkX = unitNode.gridX - j - 1;
    //                if (checkX >= 0 && checkX < grid.gridSizeX)
    //                {
    //                    Vector2 checkNodePos = new Vector2(checkX, unitNode.gridY);
    //                    if (!grid.grid[checkX, unitNode.gridY].walkable)
    //                    {
    //                        directionsChecked++;
    //                        break;
    //                    }
    //                    else
    //                        nodesInRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                    if (grid.TileFromVector2(checkNodePos).occupied)
    //                    {
    //                        if (grid.TileFromVector2(checkNodePos).occupyingUnit.team != castingUnit.team)
    //                        {
    //                            //We need to check if there's a free space adjacent to this unit.
    //                            if (checkX - 1 > 0)
    //                                if (grid.grid[checkX - 1, unitNode.gridY].walkable && !grid.TileFromVector2(new Vector2(checkX - 1, unitNode.gridY)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[checkX - 1, unitNode.gridY]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[checkX - 1, unitNode.gridY]);
    //                                        nodesInAbilityRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                                    }

    //                                }
    //                            if (grid.gridSizeX > checkX + 1)
    //                                if (grid.grid[checkX + 1, unitNode.gridY].walkable && !grid.TileFromVector2(new Vector2(checkX + 1, unitNode.gridY)).occupied)
    //                                {
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[checkX + 1, unitNode.gridY]))
    //                                    {
    //                                        nodesInAbilityRange.Add(grid.grid[checkX + 1, unitNode.gridY]);
    //                                    }
    //                                    if (!nodesInAbilityRange.Contains(grid.grid[checkX, unitNode.gridY]))
    //                                        nodesInAbilityRange.Add(grid.grid[checkX, unitNode.gridY]);
    //                                }
    //                        }
    //                    }
    //                }
    //            }

    //            if (j == abilityRange)
    //                directionsChecked++;
    //        }

    //        if (directionsChecked == 4)
    //            done = true;
    //    }
    //    yield return null;
    //    if (done == true)
    //    {
    //        if (nodesInAbilityRange.Count > 0)
    //        {
    //            AbilityRangeHandler(nodesInAbilityRange);
    //            foreach (Node node in nodesInAbilityRange)
    //            {
    //                nodesInRange.Remove(node);
    //            }
    //        }
    //        RangeHandler(nodesInRange);
    //    }
    //}

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
            Tile blastTile = grid.TileFromNode(blastNode);

            Transform displaySq = Instantiate(attackObject, blastNode.worldPosition, transform.rotation).transform;
            displaySq.parent = displayHolder;
            displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
            blastTile.inBlastZone = true;
            if (blastTile.occupied)
            {
                combat.targetList.Add(blastTile.occupyingUnit);
            }
            combat.tileTargetList.Add(blastTile);
        }
    }

    public void RangeHandler(List<Node> nodesInRange)
    {
        string displayHolderName = "DisplaySquares";
        if (transform.Find(displayHolderName))
        {
            DestroyImmediate(transform.Find(displayHolderName).gameObject);
        }
        Transform displayHolder = new GameObject(displayHolderName).transform;

        foreach (Node item in nodesInRange)
        {
            Tile nowTile = grid.TileFromNode(item);

            Transform displaySq = Instantiate(moveObject, nowTile.transform.position + Vector3.up * 0.02f, transform.rotation).transform;
            displaySq.parent = displayHolder;
            displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
            nowTile.inRange = true;
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

            Transform displaySq = Instantiate(attackObject, item.worldPosition, transform.rotation).transform;
            displaySq.parent = displayHolder;
            displaySq.localScale = new Vector3(displaySq.localScale.x * 0.9f, displaySq.localScale.y, displaySq.localScale.z * 0.9f);
            nowTile.validAbilityTarget = true;
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

            Transform displaySq = Instantiate(attackObject, nowTile.transform.position + Vector3.up * 0.02f, transform.rotation).transform;
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

                foreach (Node neighbour in grid.GetNeighbours(currentNode, 1))
                {
                    if (grid.TileFromNode(neighbour))
                    {
                        Tile neighbourTile = grid.TileFromNode(neighbour);
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
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

                foreach (Node neighbour in grid.GetNeighbours(currentNode, 1))
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
