using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBGChanger : MonoBehaviour
{
    Image myImage;
    Unit displayedUnit;
    UnitSelection uS;
    TeamManager tM;

    private void Start()
    {
        tM = TeamManager.instance;
        uS = UnitSelection.instance;
        myImage = GetComponent<Image>();
    }
    private void Update()
    {
        if (uS.hoverUnit)
        {
            displayedUnit = uS.hoverUnit;
        }
        else if (uS.selectedUnit)
        {
            displayedUnit = uS.selectedUnit;
        }
        else
            displayedUnit = null;

        if(displayedUnit)
        myImage.color = tM.teams[displayedUnit.team].teamColor;
    }
}
