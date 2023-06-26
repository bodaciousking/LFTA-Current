using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorRay : MonoBehaviour
{
    public static CursorRay instance;
    private UnitSelection unitSelect;
    public Tile hitTile;
    public Unit hitUnit;
    public GameObject moveHoverObj, attackHoverObj;
    public Transform hoverTransform;
    int layerMask = 1 << 11;
    Combat combat;
    CommandsTooltip cT;
    Pathfinding pF;
    TurnManagement tuM;
    Grid grid;
    Vector3 offsetVector;
    RaycastHit hit;
    private Vector3 targetNormal;
    Quaternion toRotation = new Quaternion(0, 0, 1, 0);
    private void Start()
    {
        combat = GetComponent<Combat>();
        cT = CommandsTooltip.instance;
        pF = Pathfinding.instance;
        unitSelect = UnitSelection.instance;
        tuM = TurnManagement.instance;
        grid = Grid.instance;
        Init();
    }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many cursor scripts!");
            return;
        }
        else instance = this;
    }
    public void Init()
    {
        GameObject hovObj = Instantiate(moveHoverObj);
        moveHoverObj = hovObj;
        GameObject atkObj = Instantiate(attackHoverObj);
        attackHoverObj = atkObj;
        moveHoverObj.SetActive(false);
        attackHoverObj.SetActive(false);
    }
    private void Update()
    {
        if (!unitSelect.moving)
        {
            moveHoverObj.SetActive(false);
            attackHoverObj.SetActive(false);
        }
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 5000, layerMask))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                hoverTransform = hit.collider.transform;
                if (hoverTransform.GetComponentInParent<Tile>())
                { 
                    hitTile = hoverTransform.GetComponentInParent<Tile>();
                    offsetVector = new Vector3(0f, hitTile.graphicsYOffset + 0.5f);
                    hitUnit = null;
                }

                else if (hoverTransform.GetComponent<Unit>())
                {
                    hitUnit = hoverTransform.GetComponent<Unit>();
                    unitSelect.hoverUnit = hitUnit;
                    hitTile = null;
                }
                else
                {
                    hitUnit = null;
                    hitTile = null;
                }


                if(hitUnit) //moused over a unit
                {
                    moveHoverObj.transform.position = hitUnit.transform.position;
                }
                else if (hitTile) // we've moused over a tile
                {
                    hitUnit = null;
                    unitSelect.hoverUnit = null;
                    if (hitTile.attackable || hitTile.inMoveRange)
                    {
                        targetNormal = Vector3.zero;
                        if (Physics.Raycast(hitTile.transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out hit, 1f, layerMask))
                        {
                            if (hit.normal == Vector3.up)
                            {
                                toRotation = new Quaternion(0, 0, 0, 0);
                            }
                            else if (hit.normal != targetNormal)
                            {
                                targetNormal = hit.normal;
                                toRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                            }
                        }

                        if (hitTile.attackable)
                        {
                            moveHoverObj.SetActive(false);
                            attackHoverObj.SetActive(true);
                            attackHoverObj.transform.SetPositionAndRotation(hoverTransform.position + offsetVector, toRotation);
                        }

                        else if (hitTile.inMoveRange)
                        {
                            moveHoverObj.SetActive(true);
                            attackHoverObj.SetActive(false);
                            moveHoverObj.transform.SetPositionAndRotation(hoverTransform.position + offsetVector, toRotation);
                        }
                    }
                }
                if (unitSelect.AoEtargetting && hitTile.validAbilityTarget)
                {
                    pF.DisplayBlastRadius(combat.castedAbility, hitTile);
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (hitTile) // clicking on a tile
                    {
                        if (unitSelect.targetting == true)
                        {
                            if (hitTile.inAttackRange && hitTile.attackable)
                            {
                                unitSelect.confirmedMoveTarget = hitTile;
                                unitSelect.confirmedTarget = hitTile;
                                return;
                            }
                            if (hitTile.inMoveRange)
                            {
                                unitSelect.confirmedMoveTarget = hitTile;
                                unitSelect.confirmedTarget = hitTile;
                                return;
                            }
                            if (hitTile.validAbilityTarget)
                            {
                                combat.unitTarget = hitTile.occupyingUnit;
                                combat.tileTarget = hitTile;
                                combat.ExecuteAbility();
                                return;
                            }
                            else
                            {
                                unitSelect.DeselectUnit();
                            }
                        }
                    }
                    else if (hitUnit) // clicking on a unit
                    {
                        if (hitUnit.alive)
                        {
                            if (unitSelect.selectedUnit)
                            {
                                if (tuM.teamTurn != hitUnit.team)
                                {
                                    if (hitUnit.currentTile.validAbilityTarget)
                                    {
                                        combat.unitTarget = hitUnit;
                                        combat.targetList.Add(hitUnit);
                                        combat.tileTarget = hitUnit.currentTile;
                                        combat.ExecuteAbility();
                                    }
                                    else if (hitUnit.currentTile.inAttackRange)
                                    {
                                        unitSelect.confirmedMoveTarget = grid.TileFromWorldPoint(hitUnit.transform.position);
                                        unitSelect.confirmedTarget = hitUnit;
                                    }
                                    else
                                    {
                                        unitSelect.DeselectUnit();
                                        unitSelect.SelectUnit(hitUnit);
                                    }
                                }
                                else
                                {
                                    if (hitUnit.currentTile.validAbilityTarget)
                                    {
                                        combat.ExecuteAbility();
                                        unitSelect.DeselectUnit();
                                        return;
                                    }
                                    else if (unitSelect.selectedUnit.midMove)
                                    {
                                        unitSelect.selectedUnit.CancelMove();
                                    }
                                    unitSelect.DeselectUnit();
                                    unitSelect.SelectUnit(hitUnit);
                                    cT.DisplayCommands();
                                }

                            }
                            else
                                unitSelect.SelectUnit(hitUnit);

                        }
                    }
                }
            }
        }
        //if (Input.GetKey(KeyCode.Mouse1))
        //{
        //    if (unitSelect.targetting == true)
        //    {
        //        cT.CloseMovement();
        //    }
        //}
    }
}
 