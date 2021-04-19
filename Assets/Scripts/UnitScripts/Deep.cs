using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deep : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball deepBall = new Fireball();
        deepBall.name = "Air Cutter";
        deepBall.description = "Short-range, powerful Ki wave";
        deepBall.range = 3;
        deepBall.element = null;
        deepBall.damage = 15;
        abilityList.Add(deepBall);
    }
    public override void CastAbility1()
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }
}
