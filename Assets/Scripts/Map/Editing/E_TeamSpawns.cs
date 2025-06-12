using System;
using UnityEngine;
using static MapEditing;

public class E_TeamSpawns : MonoBehaviour
{
    MapInfo mI;
    MapEditing mE;
    public Transform teamPanel;
    public static E_TeamSpawns instance;

    public int teamToAddTo;

    private void Start()
    {
        mI = MapInfo.instance;
        mE = MapEditing.instance;   
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many Terrain editors!");
        }
        instance = this;
    }


    public void AddNewTeamPointList()
    {
        bool[,,] newTeamList = new bool[(int)mI.mapSize.x, (int)mI.mapSize.y, (int)mI.mapSize.z];
        mI.spawnLocationLists.Add(newTeamList);

        Transform objectParent = GameObject.Find("TeamPanel").transform;
        GameObject newPanel = Instantiate(teamPanel.gameObject, objectParent).gameObject;
        EditorSpawnPanel eSP = newPanel.GetComponent<EditorSpawnPanel>();
        eSP.myTeam = (mI.spawnLocationLists.Count - 1);
        eSP.myList = newTeamList;
        teamToAddTo = eSP.myTeam;
    }

    public void RemoveTeamPointList()
    {
        mI.spawnLocationLists.Remove(mI.spawnLocationLists[teamToAddTo]);
    }


    public void SelectTeam(int i)
    {
        teamToAddTo = i;
        mE.action = Choice.ChoosingSpawnSpots;
    }

    public bool ToggleSpawnPoint(Coord CoordToAdd)
    {
        bool success = false;
        if (CoordToAdd.y + 1 < mI.mapSize.y)
        {
            if (mE.basicSectArray[CoordToAdd.x, CoordToAdd.y + 1, CoordToAdd.z] == null)
            {
                success = true;
            }
        }
        else
        { success = true; }

        if (success)
            mI.spawnLocationLists[teamToAddTo][(int)CoordToAdd.x, (int)CoordToAdd.y, (int)CoordToAdd.z] = !mI.spawnLocationLists[teamToAddTo][(int)CoordToAdd.x, (int)CoordToAdd.y, (int)CoordToAdd.z];

        return success;
    }

}
