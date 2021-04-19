using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woody : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball woodyBall = new Fireball();
        woodyBall.name = "Energy Wave";
        woodyBall.description = "Cut through the air and launch projectile.";
        woodyBall.element = null;
        woodyBall.range = 6;
        woodyBall.cooldown = 3;
        woodyBall.damage = 12;
        abilityList.Add(woodyBall);
        FlyingKnee knee = new FlyingKnee();
        abilityList.Add(knee);
    }
    public override void CastAbility1() // Fireball
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }

    public override void CastAbility2() // Knee
    {
        base.CastAbility2();

        Combat combat = Combat.instance;
        combat.attacker = this;
        combat.GetKneeTargets();
        combat.StartMoveUnit();
        combat.MultiImpact();
        Debug.Log("A");
    }

}
