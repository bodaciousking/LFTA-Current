using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LouisEX;

public class LouisEX : Unit
{
    SpriteRenderer sR;
    public int swingDamage = 5;
    public class Swing : Ability
    {
        public Swing()
        {
            name = "Flourish";
            description = "Get em!!";
            element = null;
            damage = 10;
            range = 1;
            knockback = 2;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("Flippy");
        }
    }

    public class ForcePush : Ability
    {
        public ForcePush()
        {
            name = "Force Push";
            description = "GET OUT OF HERE!!";
            element = null;
            damage = 5;
            range = 4;
            knockback = 10;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("Push");
        }
    }

    public override void Start()
    {
        sR = GetComponentInChildren<SpriteRenderer>();
        base.Start();

        ForcePush push = new ForcePush();
        abilityList.Add(push);

        Swing swing = new Swing();
        abilityList.Add(swing);
    }
    public override void AnimationTrigger(string label)
    {
        base.AnimationTrigger(label);
        Combat combat = Combat.instance;
        if (label == "Swing damage")
        {
            combat.DealDamage(swingDamage);
            return;
        }
        if (label == "Execute Ability")
        {
            combat.FullImpact();
            return;
        }
        if (label == "Flip")
        {
            sR.flipX = !sR.flipX;
        }
    }
}
