using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelection : MonoBehaviour
{
    public static UnitSelection instance;
    public Unit selectedUnit;
    public Unit hoverUnit;
    public Tile confirmedMoveTarget;
    public bool targetting, AoEtargetting, moving, confirmedTarget;
    public GameObject panelHolder;
    public Unit confirmedAbilityTarget;
    Camera viewCamera;
    Combat combat;
    CommandsTooltip cT;
    MapInfo mI;
    Grid grid;
    CameraControll cC;
    TurnManagement turnManagement;
    Tile hoverTile;
    

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
        combat = Combat.instance;
        grid = Grid.instance;
        turnManagement = TurnManagement.instance;
        cC = viewCamera.GetComponent<CameraControll>();
    }

    public void DeselectUnit()
    {
        cT.CloseMovement();
        if (selectedUnit)
            if (selectedUnit.pendingMove)
            { 
                selectedUnit.CancelMove();
            }
        selectedUnit = null;
        cT.HideCommands();
    }

    public void SelectUnit(Unit unitToSelect)
    {
        selectedUnit = unitToSelect;
        if (unitToSelect.team == turnManagement.teamTurn && unitToSelect.canAct)
        {
            cT.OpenMovement();
            cT.DisplayCommands();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        
        if (selectedUnit || hoverUnit)
        {
            panelHolder.SetActive(true);
        }
        else
            panelHolder.SetActive(false);

        hoverTile = null;
    }

    public void MoveNow()
    {
        selectedUnit.MoveNow();
        confirmedTarget = false;
        targetting = false;
        moving = false;
        cT.CloseMovement();
        cT.HideCommands();
        //TargetDisplay.instance.DestroyMoveSquares();
    }

}
