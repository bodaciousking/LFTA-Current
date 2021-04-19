using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    TeamManager tM;
    TeamManager.Team winnerTeam;
    public GameObject teamPanel, unitDisplay, victoryDisplayObject, teamdisplayObject;
    public GameObject[] disableOnW;
    public bool gameOver = false;
    SoundPlayer victoryAudio;

    private void Start()
    {
        tM = TeamManager.instance;
        GameObject soundManager = GameObject.Find("2DSoundManager");
        victoryAudio = soundManager.GetComponent<SoundPlayer>();

    }

    public void WinnerDecided()
    {
        winnerTeam = tM.winningTeam;
        victoryDisplayObject.SetActive(true);
        teamdisplayObject.SetActive(true);
        for (int i = 0; i < disableOnW.Length; i++)
        {
            GameObject obj = disableOnW[i];
            obj.SetActive(false);
        }
        DisplayWinningTeam();
        gameOver = true;
        victoryAudio.PlayClip(0);
    }


    public void DisplayWinningTeam()
    {
            GameObject panelHolder = teamdisplayObject;
            GameObject newTeamPanel = Instantiate(teamPanel, panelHolder.transform);
            for (int j = 0; j < winnerTeam.units.Count; j++)
            {
                GameObject newUnitDisplay = Instantiate(unitDisplay, newTeamPanel.transform);
                Image backgroundImage = newUnitDisplay.GetComponent<Image>();
                Image panelImage = newUnitDisplay.GetComponentsInChildren<Image>()[1];
                UnitPanelReferencer uPR = newUnitDisplay.GetComponentInChildren<UnitPanelReferencer>();
                uPR.myUnit = winnerTeam.units[j].GetComponent<Unit>();

                panelImage.sprite = winnerTeam.units[j].GetComponent<Unit>().profile;
                backgroundImage.color = winnerTeam.teamColor;
            }
        
    }
}
