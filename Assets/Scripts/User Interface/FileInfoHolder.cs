using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileInfoHolder : MonoBehaviour
{
    public string mapSectString, spawnsString;
    MapPreview mapPreview = MapPreview.instance;
    TeamPreview teamPreview = TeamPreview.instance;
    public TeamManager.Team myTeam;

    public void ClickDown()
    {
        if(mapSectString.Length > 5)
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
        mapPreview.PreviewMap(mapSectString, spawnsString);
    }

    public void AddTeam()
    {
        teamPreview.AddTeam(myTeam);
    }
}
