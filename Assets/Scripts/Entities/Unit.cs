using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : LivingEntity 
{
    [HideInInspector]
    public Transform target;
    float speed = 0.05f;
    Vector3[] path;
    UnitSelection uS;
    Grid grid;
    ShouldFlip sF;
    Tile beginningTile;
    CommandsTooltip cT;
    TeamManager tM;
    TurnManagement tuM;
    CapsuleCollider myCollider;
    SpriteRenderer rend;
    Combat combatScript;
    Pathfinding pFinding;
    //[Header("Unit Details", order = 1)]
    public string unitName;
    public string unitDescription;
    public Vector2 health;
    public Vector2 mana;
    public float movePoints;
    public int attackDamage;
    public int attackRange = 1;
    public Sprite profile;
    public GameObject projectileObject;
    public List<Ability> abilityList = new List<Ability>();

    public int goldCost = 0;
    //[HideInInspector]
    public int unitID;
    [Space]
    [Header("Combat details")]
    int targetIndex;
    public int team;
    public Vector3 location;
    public Tile currentTile;
    Tile previousTile;
    [HideInInspector]
    public Unit enemyUnit;
    [HideInInspector]
    public bool moving, facingRight = true, moved = false,  alive = true;
    public bool canAct = true;
    public int frozenTurns;
    [HideInInspector]
    public bool defending = false, midMove = false;
    [HideInInspector]
    public Animator anim;
    public virtual void Start()
    {
        ComponentSetup();
        TestFunction();
    }

    private void Update()
    {
        if(moving)
        {
            anim.SetBool("Moving", true);
        }
        if (!moving)
        {
            anim.SetBool("Moving", false);
        }
        if(health.x <= 0 && alive)
        {
            EndLife();
        }
        if (currentTile)
        {
            currentTile.occupyingUnit = this;
            currentTile.occupied = true;
        }

    }

    void EndLife()
    {
        alive = false;
        canAct = false;
        anim.Play("Base Layer.Death");
        //this.transform.position += new Vector3(0f, -0.7f, 0f);
        currentTile.occupied = false;
        currentTile.occupyingUnit = null;
        currentTile = null;
        tM.OnUnitDied();
        
        rend.color = Color.Lerp(rend.color, Color.gray, 4f);
        //rend.sortingOrder = 1;
        Destroy(myCollider);
    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            uS.hoverUnit = this;
            if (currentTile.inAttackRange && currentTile.attackable && alive)
                uS.hoverTargetTile = currentTile.transform;
        
        }
    }

    private void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            uS.hoverUnit = null;
            uS.hoverTargetTile = null;
        }
    }

    public virtual void CastAbility1()
    {

    }
    public virtual void CastAbility2()
    {

    }
    public virtual void CastAbility3()
    {

    }
    public virtual void CastAbility4() 
    { 
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (alive)
            {
                if (uS.selectedUnit)
                {
                    if (tuM.teamTurn != team)
                    {
                        if (currentTile.inAttackRange)
                        {
                            uS.confirmedMoveTarget = grid.TileFromWorldPoint(transform.position);
                            uS.confirmedTarget = this;
                        }
                        else if (currentTile.validAbilityTarget)
                        {
                            combatScript.basicTarget = this;
                            combatScript.targetList.Add(this);
                            combatScript.tileTarget = currentTile;
                            combatScript.ExecuteAbility();
                        }
                        else
                        {
                            uS.DeselectUnit();
                            uS.SelectUnit(this);
                        }
                    }
                    else
                    {
                        if (currentTile.validAbilityTarget)
                        {
                            combatScript.ExecuteAbility();
                            uS.DeselectUnit();
                            return;
                        }
                        else if (uS.selectedUnit.midMove)
                        {
                            uS.selectedUnit.CancelMove();
                        }
                        uS.DeselectUnit();
                        uS.SelectUnit(this);
                        cT.DisplayCommands();
                    }
                
                }
                else
                    uS.SelectUnit(this);

            }
        }
    }

    void ComponentSetup()
    {
        anim = GetComponentInChildren<Animator>();
        uS = UnitSelection.instance;
        grid = Grid.instance;
        sF = GetComponent<ShouldFlip>();
        cT = CommandsTooltip.instance;
        tM = TeamManager.instance;
        tuM = TurnManagement.instance;
        pFinding = Pathfinding.instance;
        combatScript = Combat.instance;
        rend = GetComponentInChildren<SpriteRenderer>();
        myCollider = GetComponent<CapsuleCollider>();
    }
    void TestFunction()
    {
    }

    public void FinishedMoving()
    {
        moved = true;
        moving = false;
        cT.DisplayCommands();
        grid.ResetTileGrid();
        grid.ResetGrid();
    }

    public void ActionsDone()
    {
        moving = false;
        moved = true;
        canAct = false;
        midMove = false;
        uS.DeselectUnit();
        tuM.AllUnitsMoved();
        grid.ResetTileGrid();
    }

    public void Defend()
    {
        defending = true;
        anim.Play("Base Layer.Defending");
        anim.SetBool("Defending", true);
        ActionsDone();
        midMove = false;
    }

    public void ReadyUnit()
    {
        anim = GetComponentInChildren<Animator>();
        if (alive)
        {
            if (frozenTurns == 0)
            {
                defending = false;
                canAct = true;
                moved = false;
                anim.SetBool("Defending", false);
                anim.Play("Base Layer.Idle");
            }
            else
            {
                frozenTurns -= 1;
                defending = false;
                canAct = false;
            }
        }
    }

    public void Shatter()
    {
        Debug.Log("shattered");
        frozenTurns = 0;
        anim.Play("ReallyHurt");
    }

    public void CancelMove()
    {
        currentTile.occupied = false;
        currentTile.occupyingUnit = null;
        transform.position = beginningTile.transform.position;
        currentTile = beginningTile;
        currentTile.occupied = true;
        currentTile.occupyingUnit = this;
        beginningTile = null;
        moved = false;
        midMove = false;
        cT.DisplayCommands();
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    public void MoveNow()
    {
        target = uS.confirmedMoveTarget.transform;
        beginningTile = currentTile;
        pFinding.StartFindPath(transform.position, target.position, this);
        target = null;
        midMove = true;
    }

    public void PlayHurtAnim()
    {
        anim.Play("Base Layer.Hurt");
    }

    public void PlayFireballAnim()
    {
        anim.Play("Fireball");
    }

    public void PlayBurningAnim()
    {
        anim.Play("Enfuego");
    }

    public void PlayFrozenAnim()
    {
        anim.Play("Frozen");
    }

    public void PlayReallyHurtAnim()
    {
        anim.Play("ReallyHurt");
    }

    public void PlayGapCloseAnim()
    {
        anim.Play("GapClose");
    }

    public void StartAttacking()
    {
        combatScript.attacker = this;
        combatScript.basicTarget = enemyUnit;
        anim.Play("Base Layer.Attack");
        uS.DeselectUnit();
        midMove = false;
    }
    public void StartRangedAttack()
    {
        combatScript.attacker = this;
        combatScript.basicTarget = enemyUnit;
        anim.Play("Base Layer.RangedAttack");
        uS.DeselectUnit();
        midMove = false;
    }

    

    IEnumerator FollowPath()
    {
        targetIndex = 0;
        Vector3 currentWaypoint = path[0];

        bool attackMove = false;
        Tile endTile = grid.TileFromWorldPoint(path[path.Length-1]);
        if (endTile.occupyingUnit)
            if (endTile.occupyingUnit.team != uS.selectedUnit.team && endTile.occupyingUnit.alive)
            {
                attackMove = true;
                enemyUnit = endTile.occupyingUnit;
            }

        if (path.Length == 1 && attackMove)
        {
            sF.currentTarget = enemyUnit.transform.position;
            StartAttacking();
            ActionsDone();
            yield break;
        }

        while (true)
        {
            if (attackMove)
            {
                if (targetIndex >= path.Length - attackRange)
                {
                    sF.currentTarget = enemyUnit.transform.position;
                    if (attackRange > 1)
                        StartRangedAttack();
                    else
                        StartAttacking();
                    ActionsDone();
                    yield break;
                }
            }
            moving = true;
            if(transform.position == currentWaypoint)
            {
                UpdateLocation(currentWaypoint);
                targetIndex++;
                
                if(targetIndex >= path.Length)
                {
                    FinishedMoving();
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            sF.currentTarget = currentWaypoint;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed);
            yield return null;
        }
        
    }
    public void UpdateLocation(Vector3 currentVec3)
    {
        previousTile = currentTile;
        currentTile = grid.TileFromWorldPoint(currentVec3);

        location = currentTile.tilePos;
        if (!previousTile.occupied || previousTile.occupyingUnit == this)
        {
            previousTile.occupied = false;
            previousTile.occupyingUnit = null;
        }

        if (!currentTile.occupied)
        {
            currentTile.occupied = true;
            currentTile.occupyingUnit = this;
        }
    }
}
