using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Davis;

public class Davis : Unit
{
    SpriteRenderer sR;
    public class Davisball : Fireball
    {
        public Davisball()
        {
            name = "Hadouken";
            description = "You know what it is!!";
            element = null;
            damage = 10;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("Fireball");
        }
    }

    public class Shoryuken : Ability
    {
        public Shoryuken()
        {
            name = "Shoryu";
            description = "You know what it is!!";
            element = null;
            damage = 20;
            range = 1;
            knockback = 2;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("Shoryuken");
        }
    }

    public override void Start()
    {
        sR= GetComponentInChildren<SpriteRenderer>();
        base.Start();

        Davisball davisBall = new Davisball();
        abilityList.Add(davisBall);

        Shoryuken shoryu = new Shoryuken();
        abilityList.Add(shoryu);
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
        if (label == "Shoryu Impact")
        {
            combat.FullImpact();
            return;
        }
        if(label== "Flip")
        {
            sR.flipX = !sR.flipX;
        }
    }
}
