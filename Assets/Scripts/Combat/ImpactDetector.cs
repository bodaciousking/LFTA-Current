using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactDetector : MonoBehaviour
{
    Combat combatScript = Combat.instance;
    Unit myUnit;
    public void BasicAttack()
    {
        combatScript.BasicImpact(false);
    }

    public void SpawnProjectile()
    {
        combatScript.BasicRangedAttack();
    }

    public void CastAbility1()
    {
        myUnit = GetComponentInParent<Unit>();
        myUnit.CastAbility1();
    }
    public void CastAbility2()
    {
        myUnit = GetComponentInParent<Unit>();
        myUnit.CastAbility2();
    }
    public void CastAbility3()
    {
        myUnit = GetComponentInParent<Unit>();
        myUnit.CastAbility3();
    }
    public void CastAbility4()
    {
        myUnit = GetComponentInParent<Unit>();
        myUnit.CastAbility4();
    }
}
