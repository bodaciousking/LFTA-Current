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


