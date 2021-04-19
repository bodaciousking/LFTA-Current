using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firen : Unit
{
    public List<GameObject> abilityObjects = new List<GameObject>();
    public override void Start()
    {
        base.Start();
        Fireball firenBall = new Fireball();
        abilityList.Add(firenBall);
        Explosion firenSplosion = new Explosion();
        abilityList.Add(firenSplosion);
    }

    public void PlayExplosionAnim()
    {
        anim.Play("Explosion");
    }

    public override void CastAbility1()
    {
        base.CastAbility1();
        GameObject explosionObject = Instantiate(abilityObjects[0], this.transform.position + Vector3.up, Quaternion.identity);
        Destroy(explosionObject, 2f);
        Combat combat = Combat.instance;
        combat.MultiImpact();
        combat.SpawnFlames();
    }

    public override void CastAbility2()
    {
        base.CastAbility2();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
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

    public override void UseAbility()
    {
        base.UseAbility();
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
        description = "Create giant powerful orb in hand";
        cooldown = 3;
        manaCost = 4;
        range = 5;
        damage = 5;
        element = "fire";
        icon = null;
    }

    public override void UseAbility()
    {
        base.UseAbility();
        Combat combat = Combat.instance;
        Unit castingUnit = combat.attacker;
        castingUnit.PlayFireballAnim();
    }
}

