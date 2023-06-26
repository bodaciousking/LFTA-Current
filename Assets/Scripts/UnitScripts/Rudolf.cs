using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudolf : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Fireball shuriken = new Fireball();
        shuriken.name = "Shuriken Toss";
        shuriken.description = "Throw ninja star";
        shuriken.element = null;
        shuriken.range = 6;
        shuriken.damage = 8;
        abilityList.Add(shuriken);

        GapClose doubleSlice = new GapClose();
        doubleSlice.name = "Double Slice";
        doubleSlice.description = "Nothin personnel, kid";
        doubleSlice.element = null;
        doubleSlice.range = 6;
        doubleSlice.damage = 11;
        abilityList.Add(doubleSlice);
    }
    public override void CastAbility1()
    {
        base.CastAbility1();
        Combat combat = Combat.instance;
        combat.BasicRangedAttack();
    }

    public override void CastAbility2()
    {
        Debug.Log("b");
        base.CastAbility2();
        Combat combat = Combat.instance;
        combat.attacker = this;
        combat.BasicGapCloser();
    }
}
