using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball freezeball = new Fireball();
        freezeball.name = "Iceball";
        freezeball.description = "Launch a sub-zero iceball at foe, freezing on hit";
        freezeball.element = "ice";
        freezeball.damage = 10;
        abilityList.Add(freezeball);
    }
    public override void CastAbility1()
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }
}
