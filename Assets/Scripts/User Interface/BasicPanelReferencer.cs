using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BasicPanelReferencer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public GameObject myFighter;
    public int myNum;
    TeamCreation teamCreation;
    public bool adding;
    public bool hero;
    Image myImage;

    private void Start()
    {
        teamCreation = TeamCreation.instance;
        myImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (!adding)
        {
            myImage.color = teamCreation.currentTeam.teamColor;
        }
    }

    private void OnMouseDown()
    {
    }

    private void OnMouseEnter()
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (adding)
        {
            teamCreation.ShowOffUnit(myFighter);
            if (hero)
            {
                teamCreation.showingHero = true;
            }
            else
                teamCreation.showingHero = false;
        }
        if (!adding)
            teamCreation.RemoveUnit(myFighter);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }
}
