using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firen : Unit
{
    public List<GameObject> abilityObjects = new List<GameObject>();
    public class Firenball : Fireball
    {
        public Firenball()
        {
            name = "Fireball";
            description = "Throw flame";
            element = "fire";
            range = 6;
            cooldown = 3;
            damage = 12;
        }
        public override void UseAbility(Unit caster)
        {
            base.UseAbility(caster);
        }
    }
    public override void Start()
    {
        base.Start();
        Firenball firenBall = new Firenball();
        abilityList.Add(firenBall);
        Explosion firenSplosion = new Explosion();
        abilityList.Add(firenSplosion);
    }

    public void PlayExplosionAnim()
    {
        anim.Play("Explosion");
    }

    public override void AnimationTrigger(string label)
    {
        base.AnimationTrigger(label);

        Combat combat = Combat.instance;
        if (label == "Explosion")
        {
            GameObject explosionObject = Instantiate(abilityObjects[0], this.transform.position + Vector3.up, Quaternion.identity);
            Destroy(explosionObject, 2f);
            combat.MultiImpact();
            combat.SpawnFlames();
            return;
        }
        else if (label == "Throw Fireball")
        {
            combat.BasicRangedAttack();
        }
    }

}


