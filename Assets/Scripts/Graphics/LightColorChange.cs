using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorChange : MonoBehaviour
{
    Light mylight;
    TurnManagement tuM;
    TeamManager tM;
    MoveWithGameObject mwg;
    // Start is called before the first frame update
    void Start()
    {
        mylight = GetComponent<Light>();
        mwg = GetComponentInParent<MoveWithGameObject>();
        tM = TeamManager.instance;
        tuM = TurnManagement.instance;
    }

    // Update is called once per frame
    void Update()
    {
        mylight.color = tM.teams[mwg.owner.GetComponent<Unit>().team].teamColor;

    }
}
