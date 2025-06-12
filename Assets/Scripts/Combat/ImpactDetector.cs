using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactDetector : MonoBehaviour
{
    Unit myUnit;
    private void Awake()
    {
        myUnit = GetComponentInParent<Unit>();
    }
    public void AnimationEvent (string label)
    {
        myUnit.AnimationTrigger(label);
    }
}
