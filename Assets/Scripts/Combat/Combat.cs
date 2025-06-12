using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public static Combat instance;
    public Unit attacker;
    public Unit unitTarget;
    public List<Unit> targetList;
    public List<Tile> tileTargetList;
    public GameObject obstructedObject;
    public Tile tileTarget;
    public Tile moveUnitTileTarget;
    public Ability castedAbility;
    public List<GameObject> combatEffects = new List<GameObject>();
    Grid grid;
    CommandsTooltip cT;
    GameObject soundObject;
    SoundPlayer soundPlayer;
    MapInfo mI;
    MapFunctions mF;
    Pathfinding pF;


    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Too many Combat instances! Destroying the one just found.");
            Destroy(instance);
        }
        instance = this;
    }
    private void Start()
    {
        cT = CommandsTooltip.instance;
        grid = Grid.instance;
        mI = MapInfo.instance;
        mF = MapFunctions.instance;
        pF = Pathfinding.instance;
        soundObject = GameObject.Find("2DSoundManager");
        soundPlayer = soundObject.GetComponent<SoundPlayer>();
    }
    private void Update()
    {
    }
    public void MultiImpact()
    {
        ShouldFlip unitSF = attacker.GetComponent<ShouldFlip>();
        unitSF.StartFlipDelay(2f);

        foreach (Unit target in targetList)
        {
            Impact(target);
        }
        ResetParams();
    }

    public void SpawnFlames()
    {
        foreach (Tile tile in tileTargetList)
        {
            GameObject flame = Instantiate(combatEffects[0], tile.transform.position, Quaternion.identity);
            Destroy(flame, 1.2f);
        }
    }

    public void Impact(Unit targetUnit)
    {
        int damageDealt = castedAbility.damage;
        //Debug.Log(targetUnit.unitName);
        if (targetUnit.alive)
        {
            if (targetUnit.frozenTurns > 0)
            {
                damageDealt *= 2;
                targetUnit.Shatter();
                soundPlayer.PlayClip(1);
            }
            else
                ApplyAbilityEffects(targetUnit, damageDealt);

            targetUnit.health.x -= damageDealt;
        }
    }

    public void ExecuteAbility()
    {
        if (unitTarget != null || targetList.Count > 0)
        {
            if (unitTarget != null && unitTarget.frozenTurns <= 0)
                FaceTarget(attacker, unitTarget);
            else
                FaceTarget(attacker, targetList[0]);
        }
        castedAbility.UseAbility(attacker);
        cT.CloseMovement();
        //TargetDisplay.instance.DestroyMoveSquares();
        attacker.ActionsDone();
    }
    public bool CheckObstruction()
    {
        bool obstructed = false;
        int layerMask = 1 << 8;

        layerMask = ~layerMask;

        RaycastHit hit;
        Vector3 origin = attacker.transform.position + new Vector3(0, 0.6f, 0);
        Vector3 endPoint = unitTarget.transform.position + new Vector3(0, 0.6f, 0);
        Vector3 dir = (endPoint - origin).normalized;
        float dis = Vector3.Distance(attacker.transform.position, unitTarget.transform.position) - 0.6f;
        if (Physics.Raycast(origin, dir, out hit, dis, layerMask))
        {
            if (hit.collider.gameObject != unitTarget.gameObject)
            {
                Debug.DrawRay(origin, dir * hit.distance, Color.red, 5f);
                GameObject obsObj = Instantiate(obstructedObject, hit.collider.transform.position, Quaternion.identity);
                obsObj.transform.localScale *= 0.2f;
                Destroy(obsObj, 2f);
                obstructed = true;
            }
            
        }
        return obstructed;
    }
    public void BasicRangedAttack()
    {
        FaceTarget(attacker, unitTarget);
        GameObject attackerProjectile = Instantiate(attacker.projectileObject, attacker.transform.position + attacker.transform.forward * 0.05f - (Vector3.up / 2), Quaternion.identity);
        attackerProjectile.transform.localScale *= 1.5f;

        ProjectileFlipping pF = attackerProjectile.GetComponent<ProjectileFlipping>();
        pF.target = unitTarget.transform.Find("UI Orient");

        bool obstructed = CheckObstruction();

        StartCoroutine(MoveProjectile(attackerProjectile, obstructed));
    }

    public void FaceTarget(Unit attacker, Unit defender)
    {
        ShouldFlip sF = attacker.GetComponent<ShouldFlip>();
        ShouldFlip sFD = defender.GetComponent<ShouldFlip>();

        sF.currentTarget = defender.transform.position;
        sFD.currentTarget = attacker.transform.position;

        sF.CompareLocation();
        sFD.CompareLocation();
    }

    public void ResetParams()
    {
        unitTarget = null;
        attacker = null;
        castedAbility = null;
        targetList.Clear();
    }
    public void DealDamage(int damage)
    {
        ShouldFlip unitSF = attacker.GetComponent<ShouldFlip>();
        unitSF.StartFlipDelay(2f);
        int damageDealt = damage;
        unitTarget.health.x -= damageDealt;
        PlayCorrespondingHurtAnim(unitTarget, damageDealt);
    }
    public void FullImpact()
    {
        ShouldFlip unitSF = attacker.GetComponent<ShouldFlip>();
        unitSF.StartFlipDelay(2f);
        int damageDealt = attacker.attackDamage;
        bool castingAbility = castedAbility != null;

        if (castingAbility)
        {
            damageDealt = castedAbility.damage;
        }

        if(unitTarget.frozenTurns > 0)
        {
            damageDealt *= 2;
            unitTarget.Shatter();
            soundPlayer.PlayClip(1);
        }
        if (castingAbility)
        {
            ApplyAbilityEffects(unitTarget, damageDealt);
        }
        else
            PlayCorrespondingHurtAnim(unitTarget, damageDealt);

        unitTarget.health.x -= damageDealt;

        ResetParams();
    }

    public void ApplyAbilityEffects(Unit unit, int damageDealt)
    {
        ShouldFlip unitSF = unit.GetComponent<ShouldFlip>();
        unitSF.StartFlipDelay(2f);
        if (castedAbility != null)
        {
            if (castedAbility.element == "fire")
                unit.PlayAnimation("Enfuego");
            else if (castedAbility.element == "ice")
            {
                unit.PlayAnimation("Frozen");
                Debug.Log("freezing");
                unit.frozenTurns = 1;
            }
            else if (castedAbility.knockback > 0)
            {
                unitTarget.ApplyKnockback(castedAbility.knockback); // knockback 2 tiles
                unit.PlayAnimation("KnockedBack");
            }
            else
            {
                PlayCorrespondingHurtAnim(unit, damageDealt);
            }
        }
        else
            PlayCorrespondingHurtAnim(unit, damageDealt);
    }

    public void PlayCorrespondingHurtAnim(Unit unit, int damageDealt)
    {
        if (damageDealt >= 10)
            unit.PlayAnimation("ReallyHurt");
        else
            unit.PlayAnimation("Base Layer.Hurt");
    }

    public void DoGapClose()
    {
        pF.StartFindGCPath(attacker.transform.position, tileTarget.transform.position);
    }

    public void GetVector3LZ(Vector3[] kneePath)
    {
        Vector3 lZ = attacker.transform.position;
        for (int i = kneePath.Length-1; i > -1; i--)
        {
            Tile tile = grid.TileFromWorldPoint(kneePath[i]);
            if (tile.occupied)
            {
                continue;
            }
            else
            {
                lZ = kneePath[i];
                break;
            }
        }
        StartCoroutine(MoveUnit(attacker.gameObject, lZ));
    }

    public List<Unit> GetKneeTargets(Vector3[] positions)
    {
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < positions.Length; i++)
        {
            Tile tile = grid.TileFromWorldPoint(positions[i]);
            tiles.Add(tile);
        }

        List<Unit> kneeTargets = new List<Unit>();
        for (int i = 0; i < tiles.Count; i++)
        {
            Tile hitTile = tiles[i];
            if (hitTile.occupied)
            {
                if (hitTile.occupyingUnit.team != attacker.team)
                { 
                    kneeTargets.Add(hitTile.occupyingUnit);
                    break;
                }
            }
        }

        return kneeTargets;
    }


    public IEnumerator MoveProjectile(GameObject projectile, bool Obstructed)
    {
        Vector3 startPos = projectile.transform.position + (Vector3.up) + (Vector3.forward * 0.7f);
        int frameCount = 35;
        Transform actualtarget = unitTarget.transform.Find("UI Orient");

        for (int i = 0; i < frameCount; i++)
        {
            projectile.transform.position = Vector3.Lerp(startPos, actualtarget.transform.position, (float)i / (float)frameCount);
            yield return null;
        }

        FullImpact();
        Animator anim = projectile.GetComponentInChildren<Animator>(); 
        anim.Play("Death");
        Destroy(projectile, 0.5f);
    }

    public IEnumerator MoveUnit(GameObject unitObj, Vector3 lz)
    {
        Vector3 startPos = unitObj.transform.position;
        Tile endTile = grid.TileFromWorldPoint(lz);
        Vector3 endPos = endTile.transform.position;

        int frameCount = 20;

        for (int i = 0; i < frameCount; i++)
        {
            unitObj.transform.position = Vector3.Lerp(startPos, endPos, (float)i / (float)frameCount);
            yield return null;
        }

        Unit unitUnit = unitObj.GetComponent<Unit>();
        unitObj.transform.position = endPos;
        unitUnit.UpdateLocation(unitObj.transform.position);
        FullImpact();
    }
}
