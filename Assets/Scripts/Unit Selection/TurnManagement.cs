using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManagement : MonoBehaviour
{
    public static TurnManagement instance;
    public int teamTurn = 0, numTeams = 0;
    TeamManager tM;
    CommandsTooltip cT;
    UnitSelection unitSelection;
    TeamManager.Team currentTeam;
    public Image turnDisplayObj;
    public GameObject endTurnButton;
    public bool autoEndTurn = true;
    Victory v;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many turn managers!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        tM = TeamManager.instance;
        cT = CommandsTooltip.instance;
        v = GetComponent<Victory>();
        unitSelection = GetComponent<UnitSelection>();
    }

    private void Update()
    {
        if (!v.gameOver)
        {
            currentTeam = tM.teams[teamTurn];
            turnDisplayObj.color = tM.teams[teamTurn].teamColor;
            numTeams = tM.teams.Count;
        }
        if (unitSelection.selectedUnit)
        {
            if (unitSelection.selectedUnit.midMove)
            {
                endTurnButton.SetActive(false);
            }
            else
                endTurnButton.SetActive(true);
        }
        else
            endTurnButton.SetActive(true);
        if (v.gameOver)
        {
            endTurnButton.SetActive(false);
        }
    }

    public bool AllUnitsMoved()
    {
        int movedUnits = 0;
        for (int i = 0; i < currentTeam.units.Count; i++)
        {
            if(currentTeam.units[i].GetComponent<Unit>().canAct == false)
            {
                movedUnits++;
            }
        }
        if (movedUnits == currentTeam.units.Count)
        {
            if(autoEndTurn)
                StartCoroutine("EndTurn");

            return true;
        }
        else
        {
            return false;
        }
    }

    void SetupTeams(int _numTeams)
    {
    }

    public void BeginEndTurn()
    {
        //StartCoroutine("EndTurn");
        BeginTurn();
    }
  

    IEnumerator EndTurn()
    {
        endTurnButton.SetActive(false);

        yield return new WaitForSeconds(1f);
        {
            endTurnButton.SetActive(true);
            BeginTurn();
        }
    }

    public void BeginTurn()
    {
        cT.HideCommands();
        if (!v.gameOver)
        {
            teamTurn++;
            if (teamTurn >= numTeams)
            {
                teamTurn = 0;
            }
            if (tM.DidLose(tM.teams[teamTurn]))
            {
                teamTurn++;
                BeginTurn();
            }


            currentTeam = tM.teams[teamTurn];
            for (int i = 0; i < currentTeam.units.Count; i++)
            {
                currentTeam.units[i].GetComponent<Unit>().ReadyUnit();
            }
        }
    }
}
