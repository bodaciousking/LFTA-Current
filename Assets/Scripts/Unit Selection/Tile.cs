using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    private UnitSelection unitSelect;
    public Vector3 tilePos;
    public int gCost, aCost;
   // private GridMapLink gml;
    public float distanceToSource;
    public bool inAttackRange;
    public Unit occupyingUnit;
    public bool inRange, occupied, attackable, blocked;
    [HideInInspector] public bool inBlastZone;
    public bool validAbilityTarget;
    public Vector3 nodeWorldPoint;
    [HideInInspector] public Renderer rend;
    public GameObject displayObj;
    CommandsTooltip cT;
    Combat combat;
    Pathfinding pF;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        unitSelect = UnitSelection.instance;
        //gml = GridMapLink.instance;
        //unitMask = LayerMask.NameToLayer("Unit");
        inRange = false;
        cT = CommandsTooltip.instance;
        pF = Pathfinding.instance;
        combat = Combat.instance;
    }

    private void Update()
    {
    }
    
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (unitSelect.targetting == true)
            {
                if (inAttackRange && attackable)
                {
                    Debug.Log("Clicked attack tile");
                    unitSelect.confirmedMoveTarget = this;
                    unitSelect.confirmedTarget = this;
                    return;
                }
                if (inRange)
                {
                    unitSelect.confirmedMoveTarget = this;
                    unitSelect.confirmedTarget = this;
                    return;
                }
                if (validAbilityTarget)
                {
                    Debug.Log("A");
                    combat.basicTarget = this.occupyingUnit;
                    combat.tileTarget = this;
                    combat.ExecuteAbility();
                    return;
                }
                else
                {
                    cT.CloseMovement();
                    return;
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (inRange || inAttackRange && attackable)
                unitSelect.hoverTargetTile = transform;
            if (unitSelect.AoEtargetting && validAbilityTarget)
            {
                pF.DisplayBlastRadius(combat.castedAbility, this);
            }
        }
    }

    private void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            unitSelect.hoverTargetTile = null;
    }
    
}
