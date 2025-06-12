using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TargettingActual : MonoBehaviour
{
    Combat combat;
    UnitSelection uS;
    Grid gridScript;
    public static TargettingActual instance;
    LayerMask mask;
    void Start()
    {
        combat = Combat.instance;
        uS = UnitSelection.instance; 
        gridScript = Grid.instance;
        mask = LayerMask.GetMask("MouseOverableObject");
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many TargettingActuals!");
            return;
        }
        instance = this;
    }

    public List<Tile> GetTilesInRange(Tile origin, int range)
    {
        List<Node> neighborNodes = Grid.instance.Get8Neighbours(Grid.instance.NodeFromTile(origin), range);
        return Grid.instance.NodesToTiles(neighborNodes);
    }

    public void ValidateTargets(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile != null) {
                if (tile.occupied)
                {
                    if (tile.occupyingUnit.team != uS.selectedUnit.team)
                    {
                        if (combat.castedAbility!= null) 
                        {
                            if (combat.castedAbility.requiresLandingTile)
                                tile.validAbilityTarget = CheckGCValidity(tile);
                            else
                                tile.validAbilityTarget = true;
                        }
                        else
                        {
                            tile.inAttackRange = true;
                        }
                    }
                    else continue;
                }
                else
                {
                    if (uS.moving)
                    {
                        tile.inMoveRange = true;
                    }
                }
            }
        }
    }
    public bool CheckGCValidity(Tile tile)
    {
        bool valid = false;
        valid = HasLineOfSight(uS.selectedUnit.transform, tile.occupyingUnit.transform, mask, combat.castedAbility.pathfindingIgnoresAllies);
        if (valid == false)
        {
            //Debug.Log("No LoS");
            return valid;
        }
        else
        valid = HasValidAdjacentLandingTile(uS.selectedUnit, tile.occupyingUnit); 

        if (valid == false)
        {
            //Debug.Log("No adjacent tile");
        }
        return valid;
    }
    public bool HasValidAdjacentLandingTile(Unit caster, Unit target)
    {
        Vector3Int casterPos = gridScript.NodeFromWorldPoint(caster.transform.position).gridPosition;
        Vector3Int targetPos = gridScript.NodeFromWorldPoint(target.transform.position).gridPosition;

        Vector3Int dir = targetPos - casterPos;

        // Clamp to direction steps of -1, 0, or 1
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = 0; // Assume 2D grid (XZ), ignore Y
        dir.z = Mathf.Clamp(dir.z, -1, 1);
        //Debug.Log("Checking for adjacent tiles near "+ target + ". X:"+ dir.x + ". Z:"+ dir.z); 

        List<Vector3Int> directionsToCheck = new List<Vector3Int>();

        // Diagonal case
        if (Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.z) == 1)
        {
            // Step 1: Get vector from target to caster
            Vector3Int toCaster = (casterPos - targetPos);
            toCaster.x = Mathf.Clamp(toCaster.x, -1, 1);
            toCaster.z = Mathf.Clamp(toCaster.z, -1, 1);

            // Step 2: Add directions that lie in-between target and caster
            // These are the *adjacent* tiles toward the caster’s direction
            directionsToCheck.Add(new Vector3Int(toCaster.x, 0, 0)); // horizontal side
            directionsToCheck.Add(new Vector3Int(0, 0, toCaster.z)); // vertical side
        }
        else
        {
            // Straight case (e.g. (0,1) or (-1,0))
            directionsToCheck.Add(-dir); // tile behind the target from caster's approach
        }

        foreach (var offset in directionsToCheck)
        {
            Vector3Int checkPos = targetPos + offset;

            if (gridScript.IsWithinBounds(checkPos))
            {
                Tile tile = gridScript.TileFromGrid(checkPos);

                if (tile != null && !tile.blocked && !tile.occupied)
                {
                    return true; // Found a good tile
                }
            }
        }

        return false; // No valid adjacent landing tile
    }
    public bool HasLineOfSight(Transform caster, Transform target, LayerMask obstructionMask, bool passThroughAllies)
    {
        Vector3 origin = caster.position + Vector3.up * 0.5f;  // Slightly above ground to avoid floor collisions
        Vector3 destination = target.position + Vector3.up * 0.5f;
        Vector3 direction = (destination - origin).normalized;
        float distance = Vector3.Distance(origin, destination);

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            // Ignore self and the actual target
            if (hit.transform == caster || hit.transform == target)
                continue;

            Unit hitUnit = hit.transform.GetComponent<Unit>();

            if (hitUnit != null)
            {
                if (passThroughAllies && hitUnit.team == caster.GetComponent<Unit>().team)
                    continue;
                else
                    return false; // Blocked by unit
            }

            // Blocked by terrain or other collider
            return false;
        }

        return true; // Clear line of sight
    }
}
