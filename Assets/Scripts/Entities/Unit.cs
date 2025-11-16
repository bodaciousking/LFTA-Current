using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using FishNet.Object;

public class Unit : NetworkBehaviour 
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
    public int moveRange;
    public int attackRange = 1;
    public int jumpRange = 1;
    public int attackDamage;
    public bool alive = true;
    public Sprite profile;
    Combat combat;
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
    public bool moving, facingRight = true, pendingMove = false;
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
        anim.SetBool("alive", false);
        anim.Play("Base Layer.Death");
        //this.transform.position += new Vector3(0f, -0.7f, 0f);
        currentTile.occupied = false;
        currentTile.occupyingUnit = null;
        tM.OnUnitDied();
        
        rend.color = Color.Lerp(rend.color, Color.gray, 4f);
        //rend.sortingOrder = 1;
        Destroy(myCollider);
    }

    public virtual void AnimationTrigger(string label)
    {
        Combat combat = Combat.instance;
        if(label == "Basic attack")
        {
            combat.attacker = this;
            combat.FullImpact();
            return;
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
        combat = Combat.instance;
    }
    void TestFunction()
    {
    }

    public void FinishedMoving()
    {
        pendingMove = true;
        moving = false;
        cT.DisplayCommands();
        cT.OpenAttack();
    }

    public void ActionsDone()
    {
        moving = false;
        pendingMove = false;
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
                pendingMove = false;
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
        pendingMove = false;
        midMove = false;
        if (combat.castedAbility != null)
        {
            combat.castedAbility = null;
        }
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
    public void PlayAnimation(string animation)
    {
        anim.Play(animation);
    }
    public void StartAttacking()
    {
        combatScript.attacker = this;
        combatScript.unitTarget = enemyUnit;
        PlayAnimation("Base Layer.Attack");
        midMove = false;
    }
    public void StartRangedAttack()
    {
        combatScript.attacker = this;
        combatScript.unitTarget = enemyUnit;
        PlayAnimation("Base Layer.RangedAttack");
        uS.DeselectUnit();
        midMove = false;
    }
    Vector3 RoundToCardinal(Vector3 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            return new Vector3(Mathf.Sign(dir.x), 0, 0);
        else
            return new Vector3(0, 0, Mathf.Sign(dir.z));
    }
    public Vector3 ComputeDirection()
    {
        // During an ability or attack
        Combat combat = Combat.instance;
        Vector3 knockbackDir = RoundToCardinal(combat.unitTarget.location - combat.attacker.location);
        return knockbackDir;
    }
    public void ApplyKnockback(int distance)
    {
        List<Vector3> knockbackPath = new List<Vector3>();
        Vector3 direction = ComputeDirection();
        Vector3 currentPos = location;

        for (int i = 0; i < distance; i++)
        {
            Vector3 nextPos = currentPos + direction;
            Tile nextTile = grid.TileFromGrid(nextPos);

            if (nextTile == null || nextTile.blocked || nextTile.occupied)
            {
                break; 
            }

            // Add adjusted position for visual y-offset
            knockbackPath.Add(nextPos + new Vector3(0, nextTile.graphicsYOffset, 0));
            currentPos = nextPos;

            if (nextTile.tilePos.y < grid.TileFromGrid(location).tilePos.y)
            {
                Debug.Log("Fall");
                //FallTo(nextTile);
                return;
            }
        }

        if (knockbackPath.Count > 0)
        {
            path = knockbackPath.ToArray();
            StartCoroutine(FollowKBPath());
        }
    }
    IEnumerator FollowKBPath()
    {
        for (int i = 0; i < path.Length; i++)
        {
            Tile wayPointTile = grid.TileFromGrid(path[i]);
            path[i] += new Vector3(0f, wayPointTile.graphicsYOffset, 0f);
        }
        targetIndex = 0;
        Vector3 currentWaypoint = grid.TileFromGrid(path[0]).transform.position;
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                UpdateLocation(currentWaypoint);
                targetIndex++;

                if (targetIndex >= path.Length)
                {
                    UpdateLocation(currentWaypoint);
                    yield break;
                }
                currentWaypoint = grid.TileFromGrid(path[targetIndex]).transform.position;
            }

            sF.currentTarget = currentWaypoint;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed);
            yield return null;
        }
    }
    IEnumerator FollowPath()
    {
        for (int i = 0; i < path.Length; i++)
        {
            Tile wayPointTile = grid.TileFromWorldPoint(path[i]);
            path[i] += new Vector3 (0f, wayPointTile.graphicsYOffset, 0f);
        }

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
        if (previousTile!=null 
            && !previousTile.occupied 
            || previousTile.occupyingUnit == this)
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
