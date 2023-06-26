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
    public bool inMoveRange, occupied, attackable, blocked;
    [HideInInspector] public bool inBlastZone;
    public bool validAbilityTarget;
    public Vector3 nodeWorldPoint;
    [HideInInspector] public Renderer rend;
    public GameObject displayObj;
    public float graphicsYOffset;
    CommandsTooltip cT;
    CursorRay cR;
    Combat combat;
    Pathfinding pF;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        unitSelect = UnitSelection.instance;
        //gml = GridMapLink.instance;
        //unitMask = LayerMask.NameToLayer("Unit");
        inMoveRange = false;
        cT = CommandsTooltip.instance;
        pF = Pathfinding.instance;
        combat = Combat.instance;
        cR = CursorRay.instance;
    }

    private void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            cR.hoverTransform = null;
    }
    
}
