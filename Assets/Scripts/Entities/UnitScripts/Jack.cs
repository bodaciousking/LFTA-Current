using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball jackball = new Fireball();
        jackball.name = "Gadouken";
        jackball.description = "Ki blast, krillin-style.";
        jackball.element = null;
        jackball.damage = 10;
        abilityList.Add(jackball);
    }
    public override void AnimationTrigger(string label)
    {
        base.AnimationTrigger(label);
        Combat combat = Combat.instance;
        if (label == "Throw Fireball")
        { 
            combat.BasicRangedAttack();
            return;
        }
    }
}
