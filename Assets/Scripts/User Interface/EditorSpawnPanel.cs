using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EditorSpawnPanel : MonoBehaviour
{
    public TextMeshProUGUI teamNameDisplay;
    public int myTeam;
    MapEditing mE;
    public bool[,,] myList;

    private void Start()
    {
        mE = MapEditing.instance;
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
