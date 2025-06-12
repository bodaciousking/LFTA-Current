using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EditorSpawnPanel : MonoBehaviour
{
    public TextMeshProUGUI teamNameDisplay;
    public int myTeam;
    E_TeamSpawns mE;
    public bool[,,] myList;

    private void Start()
    {
        mE = E_TeamSpawns.instance;
    }

    public void SelectMe()
    {
        mE.SelectTeam(myTeam);
    }

    private void Update()
    {
        teamNameDisplay.text = "Team " + myTeam.ToString();
        
    }
}
