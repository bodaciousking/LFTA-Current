using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitPanelReferencer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Unit myUnit;
    UnitSelection uS;

    private void Start()
    {
        uS = UnitSelection.instance;
    }

    private void OnMouseDown()
    {
    }

    private void OnMouseEnter()
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uS.hoverUnit = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (uS.selectedUnit)
            uS.DeselectUnit();
        uS.SelectUnit(myUnit);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uS.hoverUnit = myUnit;
    }
}
