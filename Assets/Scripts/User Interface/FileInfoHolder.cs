using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileInfoHolder : MonoBehaviour
{
    public string mapString, spawnsString, teamString;
    MapPreview mapPreview = MapPreview.instance;
    TeamPreview teamPreview = TeamPreview.instance;
    //LobbyManager lM;
    public TeamManager.Team myTeam;
    private void Start()
    {
        //lM = LobbyManager.Instance;
    }
    public void OnClick()
    {
        
    }
    public void ClickDown()
    {
        if (mapString.Length > 5)
        {
            SelectMap();
        }
        else
        {
            AddTeam();
        }
    }
    public void SelectMap()
    {
        //lM.RequestMapChangeRpc(mapString, spawnsString);
    }

    public void AddTeam()
    {
        //lM.AddTeamToLobbyRpc(teamString);

        // SINGLE PLAYER
        // teamPreview.AddTeam(myTeam);
    }
}
