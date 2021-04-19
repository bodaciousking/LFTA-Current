using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Davis : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball davisBall = new Fireball();
        davisBall.name = "Hadouken";
        davisBall.description = "You know what it is!!";
        davisBall.element = null;
        davisBall.damage = 10;
        abilityList.Add(davisBall);
    }
    public override void CastAbility1()
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }
}
