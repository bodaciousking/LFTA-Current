using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelPopulator : MonoBehaviour
{
    public TextMeshProUGUI unitName, health, mana;
    public Image profilePic;
    UnitSelection uS;

    private void Start()
    {
        uS = GameObject.Find("GameManager").GetComponent<UnitSelection>();
    }

    private void Update()
    {
        Unit selectedUnit;
        if (uS.hoverUnit)
        {
            selectedUnit = uS.hoverUnit;
        }
        else
        selectedUnit = uS.selectedUnit;

        if (selectedUnit)
        {
            unitName.text = (selectedUnit.unitName);
            health.text = ("Hp " + selectedUnit.health.x + "/" + selectedUnit.health.y);
            mana.text = ("Mp " + selectedUnit.mana.x + "/" + selectedUnit.mana.y);
            profilePic.sprite = selectedUnit.profile;
        }
    }
}
