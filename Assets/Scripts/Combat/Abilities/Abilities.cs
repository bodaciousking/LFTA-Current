using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ability
{
    public string name;
    public string description;
    public int cooldown;
    public int manaCost;
    public int range;
    public Sprite icon;
    public int radius;
    public int damage;
    public string element;

    public enum TargetMode
    {
        MultiTarget,
        SingleLineTarget,
        MultiLineTarget,
        SingleAoETarget
    }

    public TargetMode targetMethod;

    public Ability()
    {
        name = "dummyName";
        description = "this is a dummy ability.";
        cooldown = 0;
        manaCost = 0;
        range = 0;
        icon = null;
        targetMethod = TargetMode.MultiTarget;
    }
    public virtual void UseAbility()
    {
        Debug.Log("Casting " + name);
        Combat combat = Combat.instance;
        combat.attacker.mana.x -= manaCost;
    }
}

public class AoEAbility : Ability
{
    public AoEAbility()
    {
        name = "AoEAbility";
        description = "This is an AoE Ability.";
        cooldown = 0;
        manaCost = 0;
        damage = 0;
        range = 0;
        targetMethod = TargetMode.SingleAoETarget;
    }
}

public class ProjectileAbility : Ability
{
    public ProjectileAbility()
    {
        name = "projectile";
        description = "This is a projectile ability.";
        cooldown = 0;
        manaCost = 0;
        damage = 0;
        range = 0;
        targetMethod = TargetMode.SingleLineTarget;
    }
}


public class FlyingKnee : Ability
{
    public FlyingKnee()
    {
        name = "Flying Knee";
        targetMethod = TargetMode.MultiLineTarget;
        description = "Captain Who?";
        cooldown = 4;
        manaCost = 4;
        range = 6;
        damage = 12;

    }

    public override void UseAbility()
    {
        base.UseAbility();
        Combat combat = Combat.instance;
        Unit castingUnit = combat.attacker.gameObject.GetComponent<Unit>();
        castingUnit.PlayGapCloseAnim();
    }
}
