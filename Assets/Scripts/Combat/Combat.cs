using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public static Combat instance;
    public Unit attacker;
    public Unit basicTarget;
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
        soundObject = GameObject.Find("2DSoundManager");
        soundPlayer = soundObject.GetComponent<SoundPlayer>();
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
                ElementalEffects(targetUnit, damageDealt);

            targetUnit.health.x -= damageDealt;
        }
    }

    public void ElementalEffects(Unit unit, int damageDealt)
    {
        ShouldFlip unitSF = unit.GetComponent<ShouldFlip>();
        unitSF.StartFlipDelay(2f);

        if (castedAbility != null)
        {
            if (castedAbility.element == "fire")
                unit.PlayBurningAnim();
            else if (castedAbility.element == "ice")
            {
                unit.PlayFrozenAnim();
                unit.frozenTurns = 1;
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
            unit.PlayReallyHurtAnim();
        else
            unit.PlayHurtAnim();
    }

    public void GetKneeTargets()
    {
        Vector3 attackerPos = attacker.currentTile.tilePos;
        Vector3 endPos = GetVector2LZ();
        Debug.Log(attackerPos);
        Debug.Log(endPos);
        Tile hitTile;

        if (attackerPos.x != endPos.x)
        {
            for (int i = 0; i < Mathf.Abs(attackerPos.x - endPos.x); i++)
            {
                if (endPos.x > attackerPos.x)
                {
                    hitTile = grid.TileFromVector2(new Vector2(attackerPos.x + i, attackerPos.y));
                }
                else
                {
                    hitTile = grid.TileFromVector2(new Vector2(attackerPos.x - i, attackerPos.y));
                }

                if (hitTile.occupied)
                {
                    Unit hitUnit = hitTile.occupyingUnit;
                    if (hitUnit != attacker && !targetList.Contains(hitUnit))
                    {
                        targetList.Add(hitUnit);
                        //Debug.Log("Adding " + hitUnit.name);
                    }
                }
            }
        }
        else if (attackerPos.y != endPos.y)
        {
            for (int i = 0; i < Mathf.Abs(attackerPos.y - endPos.y); i++)
            {
                if (endPos.y > attackerPos.y)
                    hitTile = grid.TileFromVector2(new Vector2(attackerPos.x, attackerPos.y + i));
                else
                    hitTile = grid.TileFromVector2(new Vector2(attackerPos.x, attackerPos.y - i));

                if (hitTile.occupied)
                {
                    Unit hitUnit = hitTile.occupyingUnit;
                    if (hitUnit != attacker && !targetList.Contains(hitUnit))
                    {
                        targetList.Add(hitUnit);
                        Debug.Log("Adding " + hitUnit.name);
                    }
                }
            }
        }

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
        basicTarget = null;
        attacker = null;
        castedAbility = null;
        targetList.Clear();
    }

    public void BasicImpact(bool obstructed)
    {
        ShouldFlip unitSF = attacker.GetComponent<ShouldFlip>();
        unitSF.StartFlipDelay(2f);

        int damageDealt = attacker.attackDamage;
        if (obstructed)
            damageDealt = damageDealt / 2;
        if (basicTarget.defending)
            damageDealt = damageDealt / 2;

        if(basicTarget.frozenTurns > 0)
        {
            damageDealt *= 2;
            basicTarget.Shatter();
            soundPlayer.PlayClip(1);
        }
        else
            ElementalEffects(basicTarget, damageDealt);

        basicTarget.health.x -= damageDealt;
        ResetParams();
    }

    public void ExecuteAbility()
    {
        if (basicTarget != null || targetList.Count > 0)
        {
            if (basicTarget != null)
                FaceTarget(attacker, basicTarget);
            else
                FaceTarget(attacker, targetList[0]);
        }
        castedAbility.UseAbility();
        cT.CloseMovement();
        attacker.ActionsDone();
    }
    public void StartMoveUnit()
    {
        StartCoroutine(MoveUnit(attacker.gameObject));
    }

    public void BasicRangedAttack()
    {
        FaceTarget(attacker, basicTarget);
        GameObject attackerProjectile = Instantiate(attacker.projectileObject, attacker.transform.position + attacker.transform.forward * 0.05f - (Vector3.up /2), Quaternion.identity);
        if(castedAbility!= null)
        {
            attackerProjectile.transform.localScale *= 1.5f;
        }
        
        ProjectileFlipping pF = attackerProjectile.GetComponent<ProjectileFlipping>();
        pF.target = basicTarget.transform.Find("UI Orient");

        bool obstructed = false; ;
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 origin = attacker.transform.position + new Vector3(0, 0.6f, 0);
        Vector3 endPoint = basicTarget.transform.position + new Vector3(0, 0.6f, 0);
        Vector3 dir = (endPoint - origin).normalized;
        float dis = Vector3.Distance(attacker.transform.position, basicTarget.transform.position) - 0.6f;
        if (Physics.Raycast(origin, dir, out hit, dis, layerMask))
        {
            if (hit.collider.gameObject != basicTarget.gameObject)
            {
                Debug.DrawRay(origin, dir * hit.distance, Color.red, 5f);
                GameObject obsObj = Instantiate(obstructedObject, hit.collider.transform.position, Quaternion.identity);
                obsObj.transform.localScale *= 0.2f;
                Destroy(obsObj, 2f);
                obstructed = true;
            }
            else
            {
                Debug.DrawRay(origin, dir * dis, Color.green, 5f);
            }
        }
        else
        {
            Debug.DrawRay(origin, dir * dis, Color.green, 5f);
        }


        StartCoroutine(MoveProjectile(attackerProjectile, obstructed));
    }

    public Vector2 GetVector2LZ()
    {
        Vector3 landingPos = tileTarget.tilePos;
        if (tileTarget.occupied)
        {
            if (attacker.currentTile.tilePos.x > tileTarget.tilePos.x)
            {
                Tile testTile = mI.tileGrid[(int)tileTarget.tilePos.x + 1, (int)tileTarget.tilePos.y, (int)tileTarget.tilePos.z];
                if (testTile.occupied)
                {
                    testTile = mI.tileGrid[(int)tileTarget.tilePos.x - 1, (int)tileTarget.tilePos.y, (int)tileTarget.tilePos.z];
                }
                landingPos = testTile.tilePos;
            }
            else if (attacker.currentTile.tilePos.x < tileTarget.tilePos.x)
            {
                Tile testTile = mI.tileGrid[(int)tileTarget.tilePos.x - 1, (int)tileTarget.tilePos.y, (int)tileTarget.tilePos.z];
                if (testTile.occupied)
                {
                    testTile = mI.tileGrid[(int)tileTarget.tilePos.x + 1, (int)tileTarget.tilePos.y, (int)tileTarget.tilePos.z];
                }
                landingPos = testTile.tilePos;
            }
            else if (attacker.currentTile.tilePos.y > tileTarget.tilePos.y)
            {
                Tile testTile = mI.tileGrid[(int)tileTarget.tilePos.x, (int)tileTarget.tilePos.y - 1, (int)tileTarget.tilePos.z];
                if (testTile.occupied)
                {
                    testTile = mI.tileGrid[(int)tileTarget.tilePos.x, (int)tileTarget.tilePos.y + 1, (int)tileTarget.tilePos.z];
                }
                landingPos = testTile.tilePos;
            }
            else if (attacker.currentTile.tilePos.y < tileTarget.tilePos.y)
            {
                Tile testTile = mI.tileGrid[(int)tileTarget.tilePos.x, (int)tileTarget.tilePos.y - 1, (int)tileTarget.tilePos.z];
                if (testTile.occupied)
                {
                    testTile = mI.tileGrid[(int)tileTarget.tilePos.x, (int)tileTarget.tilePos.y + 1, (int)tileTarget.tilePos.z];
                }
                landingPos = testTile.tilePos;
            }
        }
        return landingPos;
    }


    public IEnumerator MoveProjectile(GameObject projectile, bool Obstructed)
    {
        Vector3 startPos = projectile.transform.position + (Vector3.up) + (Vector3.forward * 0.7f);
        int frameCount = 35;
        Transform actualtarget = basicTarget.transform.Find("UI Orient");

        for (int i = 0; i < frameCount; i++)
        {
            projectile.transform.position = Vector3.Lerp(startPos, actualtarget.transform.position, (float)i / (float)frameCount);
            yield return null;
        }

        BasicImpact(Obstructed);
        Destroy(projectile);
    }

    public IEnumerator MoveUnit(GameObject unit)
    {
        Vector3 startPos = unit.transform.position;
        Vector2 endTilePos = GetVector2LZ();
       // Vector3 endPos = mI.tileGrid[(int)endTilePos.x, (int)endTilePos.y].transform.position;

        int frameCount = 20;

        for (int i = 0; i < frameCount; i++)
        {
           // unit.transform.position = Vector3.Lerp(startPos, endPos, (float)i / (float)frameCount);
            yield return null;
        }

        Unit unitUnit = unit.GetComponent<Unit>();
       // unit.transform.position = endPos;
        unitUnit.UpdateLocation(unit.transform.position);
    }
}
