using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamPiano : MonoBehaviour
{
    public TeamManager.Team myTeam;
    public GameObject unitKey;
    public TextMeshProUGUI tmp;
    //LobbyManager lobbyManager = LobbyManager.Instance;
    TeamPreview tp;
    TeamCreation tC = TeamCreation.instance;
    AllUnitsLibrary aUL = AllUnitsLibrary.instance;

    public void Refresh()
    {
        myTeam.units.Clear();
        Transform keyHolder = transform.Find("KeyHolder");
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = myTeam.teamName;
        this.name = myTeam.teamName;
        foreach (Transform item in keyHolder)
        {
            Destroy(item);
        }
        if (myTeam.unitNums.Length > 0)
            for (int i = 0; i < myTeam.unitNums.Length; i++)
            {
                if (myTeam.unitNums[i] <= 11)
                {
                    myTeam.units.Add(aUL.heroList[myTeam.unitNums[i]]);
                    //Debug.Log("Added to the team: " + myTeam.units[i].name);
                }
                else if (myTeam.unitNums[i] > 11)
                {
                    myTeam.units.Add(aUL.gruntList[myTeam.unitNums[i] - 12]);

                    //Debug.Log("Added to the team: " + myTeam.units[i].name);
                }
                GameObject key = Instantiate(unitKey, keyHolder);
                TeamKey tk = key.GetComponent<TeamKey>();
                tk.childImage.sprite = myTeam.units[i].GetComponent<Unit>().profile;
                tk.bgImage.color = myTeam.teamColor;
            }

    }

    public void RemoveMyTeam()
    {
        tp = TeamPreview.instance;
        tp.RemoveTeam(myTeam);
        Destroy(this.gameObject);
    }
    public void TryRemoveTeam()
    {
       /* if (lobbyManager.playingOnline)
        // Only send the teamName (unique identifier) to the server
        {
            string teamName = myTeam.teamName;
            lobbyManager.RemoveTeamFromLobbyRpc(teamName);
        }
        else 
            RemoveMyTeam();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
