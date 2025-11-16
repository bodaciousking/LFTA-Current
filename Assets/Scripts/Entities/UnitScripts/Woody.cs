using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woody : Unit
{
    public class FlyingKnee : GapClose
    {
        public FlyingKnee()
        {
            name = "Flying Knee";
            description = "Captain Falcon style";
            element = null;
            range = 7;
            damage = 15;
            knockback = 1;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
            caster.PlayAnimation("GapClose");
        }
    }
    public class EnergyWave : Fireball
    {
        public EnergyWave()
        {
            name = "Energy Wave";
            description = "Cut through the air to launch projectile.";
            element = null;
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
        EnergyWave woodyBall = new EnergyWave();
        abilityList.Add(woodyBall);
        FlyingKnee knee = new FlyingKnee();
        abilityList.Add(knee);
    }
    public override void AnimationTrigger(string label) // Fireball
    {
        base.AnimationTrigger(label);

        Combat combat = Combat.instance;
        if (label == "Throw Fireball")
        {
            combat.BasicRangedAttack();
            return;
        }
        if (label == "Close Gap")
        {
            combat.attacker = this;
            combat.DoGapClose();
        }
    }
}
