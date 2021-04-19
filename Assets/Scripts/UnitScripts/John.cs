using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class John : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball johnBall = new Fireball();
        johnBall.name = "Living Blast";
        johnBall.description = "Blast foe with energy orb at range";
        johnBall.range = 10;
        johnBall.element = null;
        johnBall.damage = 10;
        abilityList.Add(johnBall);
    }
    public override void CastAbility1()
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }
}
