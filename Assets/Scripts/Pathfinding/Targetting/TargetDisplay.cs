using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetDisplay : MonoBehaviour
{
    public GameObject moveObject, attackObject, abilityObject;
    public string displayHolderName = "Display Holder";
    public Transform displayHolder;
    public static TargetDisplay instance;
    Combat combat;
    UnitSelection uS;
    private void Start()
    {
        combat = Combat.instance;
        uS = UnitSelection.instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many TargetDisplays!");
            return;
        }
        instance = this;
    }

    public void Display(List<Tile> tiles)
    {
        DestroyMoveSquares();

        GameObject holder = new GameObject(displayHolderName);
        displayHolder = holder.transform;

        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                if (tile.occupied)
                {
                    if (tile.occupyingUnit.team != uS.selectedUnit.team)
                    {
                        SpawnDisplaySq(attackObject, tile);
                    }
                }
                else
                {
                    if (combat.castedAbility != null)
                    {
                        SpawnDisplaySq(abilityObject, tile);
                    }
                    else
                    {
                        SpawnDisplaySq(moveObject, tile);
                    }
                }
                
            }
        }
    }
    public void DestroyMoveSquares()
    {
        if (displayHolder != null)
        {
            Destroy(displayHolder.gameObject);
            displayHolder = null;
        }
    }
    public void SpawnDisplaySq(GameObject toSpawn, Tile tile)
    {
        Transform sq = Instantiate(toSpawn, tile.transform.position + new Vector3(0f, tile.graphicsYOffset, 0f), transform.rotation).transform;
        sq.transform.SetParent(displayHolder);
        sq.localScale = new Vector3(sq.localScale.x * 0.9f, sq.localScale.y, sq.localScale.z * 0.9f);
    }
    public List<Tile> GetTilesInRange(Tile origin, int range)
    {
        List<Node> neighborNodes = Grid.instance.Get8Neighbours(Grid.instance.NodeFromTile(origin), range);
        return Grid.instance.NodesToTiles(neighborNodes);
    }
}
