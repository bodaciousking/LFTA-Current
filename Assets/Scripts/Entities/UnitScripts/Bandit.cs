using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bandit;

public class Bandit : Unit
{
    SpriteRenderer sR;
    public override void Start()
    {
        sR = GetComponentInChildren<SpriteRenderer>();
        base.Start();
    }
    public override void AnimationTrigger(string label)
    {
        base.AnimationTrigger(label);
        Combat combat = Combat.instance;
    }
}
