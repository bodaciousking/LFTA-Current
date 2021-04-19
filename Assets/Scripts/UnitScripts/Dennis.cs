using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dennis : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball dennisBall = new Fireball();
        dennisBall.name = "Hadouken";
        dennisBall.description = "You know what it is!!";
        dennisBall.element = null;
        dennisBall.damage = 10;
        abilityList.Add(dennisBall);
    }
    public override void CastAbility1()
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }
}
