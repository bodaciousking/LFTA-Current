using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Unit
{
    public class Iceball : Fireball
    {
        public Iceball()
        {
            name = "Iceball";
            description = "Launch a sub-zero iceball at foe, freezing on hit";
            element = "ice";
            range = 6;
            cooldown = 3;
            damage = 12;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("Fireball");
        }
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Iceball freezeball = new Iceball();
        abilityList.Add(freezeball);
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
