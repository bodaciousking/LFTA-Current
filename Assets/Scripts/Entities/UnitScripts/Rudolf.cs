using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rudolf;

public class Rudolf : Unit
{
    public class Shuriken : Fireball
    {
        public Shuriken()
        {
            name = "Shuriken Toss";
            description = "Throw ninja star";
            element = null;
            range = 6;
            damage = 8;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
        }
    }

    public class DoubleSlice : GapClose
    {
        public DoubleSlice()
        {
            name = "Double Slice";
            description = "Nothin personnel, kid";
            element = null;
            range = 6;
            damage = 15;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("GapClose");
        }
    }

    public override void Start()
    {
        base.Start();
        Shuriken shuriken = new Shuriken();
        abilityList.Add(shuriken);

        DoubleSlice doubleSlice = new DoubleSlice();

        abilityList.Add(doubleSlice);
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
        else if (label == "Close Gap")
        {
            combat.attacker = this;
            combat.DoGapClose();
        }
    }
}
