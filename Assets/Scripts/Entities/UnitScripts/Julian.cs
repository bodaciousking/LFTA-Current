using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Julian : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball skullBall = new Fireball();
        skullBall.name = "skullBall";
        skullBall.description = "Throw ninja star";
        skullBall.element = null;
        skullBall.range = 6;
        skullBall.damage = 8;
        abilityList.Add(skullBall);

        GapClose darkFist = new GapClose();
        darkFist.name = "Dark Fist";
        darkFist.description = "nbd, but i slammed ya";
        darkFist.element = null;
        darkFist.range = 4;
        darkFist.vRange = 1;
        darkFist.damage = 20;
        abilityList.Add(darkFist);
    }
    public override void AnimationTrigger(string label)
    {
        base.AnimationTrigger(label);
        Combat combat = Combat.instance;
        if (label == "Skull")
        {
            combat.BasicRangedAttack();
            return;
        }
        else if (label == "Gap Close")
        {
            combat.attacker = this;
            combat.DoGapClose();
        }
    }
}