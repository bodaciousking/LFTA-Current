using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelection : MonoBehaviour
{
    public static UnitSelection instance;
    public Unit selectedUnit;
    public Unit hoverUnit;
    public Transform hoverTargetTile;
    public Tile confirmedMoveTarget;
    public bool targetting, AoEtargetting, moving, confirmedTarget, attacking;
    public GameObject hoverObj, attackHoverObj, panelHolder;
    public Unit confirmedAbilityTarget;
    Camera viewCamera;
    CommandsTooltip cT;
    MapInfo mI;
    Grid grid;
    CameraControll cC;
    TurnManagement turnManagement;
    

    private void Awake()
    {
        if(instance!= null)
        {
            Debug.Log("Too many Unit selections!");
            return;
        }
        instance = this;
    }
    void Start()
    {
        selectedUnit = null;
        viewCamera = Camera.main;
        cT = CommandsTooltip.instance;
        mI = MapInfo.instance;
        grid = Grid.instance;
        turnManagement = TurnManagement.instance;
        cC = viewCamera.GetComponent<CameraControll>();
    }

    public void DeselectUnit()
    {
        cT.CloseMovement();
        selectedUnit = null;
        cT.HideCommands();
    }

    public void SelectUnit(Unit unitToSelect)
    {
        selectedUnit = unitToSelect;
        if (unitToSelect.team == turnManagement.teamTurn)
        {
            cT.DisplayCommands();
        }
        cC.GoToPos(unitToSelect.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        if (selectedUnit && confirmedTarget)
        {
            MoveNow();
        }
        if (selectedUnit || hoverUnit)
        {
            panelHolder.SetActive(true);
        }
        else
            panelHolder.SetActive(false);

        if (moving)
        {
            Tile hoverTile = null;
            if (hoverTargetTile)
            {
                hoverTile = grid.TileFromWorldPoint(hoverTargetTile.position);
                if (hoverTile.attackable)
                {
                    hoverObj.SetActive(false);
                    attackHoverObj.SetActive(true);
                    attackHoverObj.transform.position = hoverTargetTile.position;
                }
                else
                {
                    hoverObj.SetActive(true);
                    attackHoverObj.SetActive(false);
                    hoverObj.transform.position = hoverTargetTile.position;
                }
            }
            else
            {
                hoverObj.transform.position = selectedUnit.transform.position;
            }
        }
        if (attacking)
        {
            hoverObj.SetActive(false);
            attackHoverObj.SetActive(true);
            if (hoverTargetTile)
                attackHoverObj.transform.position = hoverTargetTile.position;
            else
                attackHoverObj.transform.position = selectedUnit.transform.position;
        }
        
    }

    private void MoveNow()
    {
        selectedUnit.MoveNow();
        confirmedTarget = false;
        targetting = false;
        moving = false;
        cT.CloseMovement();
    }

}
