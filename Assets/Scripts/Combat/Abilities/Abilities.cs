using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public abstract class Ability
{
    public Sprite icon;
    public string name;
    public string description;
    public string element;
    public int cooldown;
    public int manaCost;
    public int range;
    public int vRange;
    public int radius;
    public int damage;
    public int knockback;
    public bool pathfindingIgnoresAllies;
    public bool requiresLandingTile = false;
    public TargetMode targetMethod;

    public enum TargetMode
    {
        MultiTarget,
        CircleTarget,
        MultiLineTarget,
        SingleAoETarget,
        SingleTarget
    }


    public Ability()
    {
        name = "dummyName";
        description = "this is a dummy ability.";
        cooldown = 0;
        manaCost = 0;
        range = 0;
        vRange = 0;
        icon = null;
        targetMethod = TargetMode.MultiTarget;
    }
    public virtual void UseAbility(Unit caster)
    {
        Combat combat = Combat.instance;
        caster.mana.x -= manaCost;
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

public class Throw : Ability
{
    public Throw()
    {
        name = "Throw";
        description = "This is a throwing ability.";
        cooldown = 0;
        manaCost = 0;
        damage = 0;
        range = 0;
        targetMethod = TargetMode.SingleTarget;
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
        targetMethod = TargetMode.CircleTarget;
    }
}


public class GapClose : Ability
{
    public GapClose()
    {
        name = "Flying Knee";
        targetMethod = TargetMode.CircleTarget;
        description = "Captain Who?";
        requiresLandingTile = true;
        cooldown = 4;
        manaCost = 4;
        range = 6;
        damage = 12;

    }

    public override void UseAbility(Unit caster)
    {
        base.UseAbility(caster);
    }
}

public class Push : Ability
{
    public Push()
    {
        name = "Push";
        targetMethod = TargetMode.CircleTarget;
        description = "Force";
        cooldown = 4;
        manaCost = 4;
        range = 6;
        damage = 12;
    }
    public override void UseAbility(Unit caster)
    {
        base.UseAbility(caster);
        Combat combat = Combat.instance;
        caster.PlayAnimation("Push");
    }
}

public class Explosion : AoEAbility
{
    public Explosion()
    {
        name = "Explosion";
        description = "Like self-destruct, but without the self destruction";
        cooldown = 8;
        manaCost = 8;
        range = 2;
        radius = 2;
        damage = 15;
        element = "fire";
        icon = null;
    }

    public override void UseAbility(Unit caster)
    {
        base.UseAbility(caster);
        Combat combat = Combat.instance;
        Firen castingUnit = combat.attacker.GetComponent<Firen>();
        castingUnit.PlayExplosionAnim();
    }
}

public class Fireball : ProjectileAbility
{
    public Fireball()
    {
        name = "Fireball";
        description = "Create powerful flame in hand";
        cooldown = 3;
        manaCost = 4;
        range = 5;
        damage = 5;
        element = "fire";
        icon = null;
    }

    public override void UseAbility(Unit caster)
    {
        base.UseAbility(caster);
        caster.PlayAnimation("Fireball");
    }
}

public class Slam : Throw
{
    public Slam()
    {
        name = "Slam";
        description = "Welcome to the jam";
        cooldown = 0;
        manaCost = 0;
        range = 1;
        damage = 10;
        icon = null;
    }

    public override void UseAbility(Unit caster)
    {
        base.UseAbility(caster);
        Combat combat = Combat.instance;
        Unit castingUnit = combat.attacker;
    }
}
