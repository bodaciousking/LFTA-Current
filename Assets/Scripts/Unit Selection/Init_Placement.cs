using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init_Placement : MonoBehaviour
{
    public List<GameObject> randomPlaceFighters = new List<GameObject>();
    public GameObject shadowLight;

    MapFunctions mF;
    TeamManager tM;
    TeamDisplay tD;
    Grid grid;
    public GameObject shadowsHolder;
    public static Init_Placement instance;

    private void Awake()
    {
        if(instance!= null)
        {
            Debug.Log("Too many IPs!");
            return;
        }
        instance = this;
        tD = TeamDisplay.instance;

    }
    private void Start()
    {
        mF = MapFunctions.instance;
        grid = Grid.instance;
    }

    public void SpawnTeams()
    {
        MapInfo mapInfo = MapInfo.instance;
        tM = TeamManager.instance;
        for (int i = 0; i < mapInfo.teams.Count; i++)
        {
            tM.teams.Add(SpawnTeam(mapInfo.teams[i], mapInfo.spawnLocationLists[i], i));
        }
    }
    public TeamManager.Team SpawnTeam(TeamManager.Team team, bool[,,] spawnLocations, int teamNum)
    {
        TeamManager.Team newTeam = new TeamManager.Team();
        newTeam.units = new List<GameObject>();
        newTeam.teamName = team.teamName;
        newTeam.teamID = teamNum;
        newTeam.teamColor = team.teamColor;
        newTeam.teamCost = team.teamCost;

        grid = Grid.instance;
        mF = MapFunctions.instance;

        List<Vector3> spawnPositions = new List<Vector3>();
        Vector3 mapSize = new Vector3(spawnLocations.GetLength(0), spawnLocations.GetLength(1), spawnLocations.GetLength(2));

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    if (spawnLocations[x, y, z])
                    {
                        spawnPositions.Add(mF.CoordToPosition(x, y, z, mapSize));
                    }
                }
            }
        }

        if (team.units.Count > spawnPositions.Count)
        {
            Debug.Log("Error! Team size is too big for this map! Not enough locations found to spawn in!");
            return team;
        }
        GameObject unitsHolder = new GameObject(team.teamName + " Units");
        for (int i = 0; i < team.units.Count; i++)
        {
            GameObject newFighter = SpawnFighter(team.units[i], spawnPositions[i], teamNum);
            newFighter.transform.parent = unitsHolder.transform;
            newTeam.units.Add(newFighter);
        }

        return newTeam;
    }

    public GameObject SpawnFighter(GameObject fighter, Vector3 spawnLocation, int teamNum)
    {
        GameObject newFighter = Instantiate(fighter, spawnLocation, Quaternion.identity);

        GameObject sL = Instantiate(shadowLight, shadowsHolder.transform);
        MoveWithGameObject mW = sL.GetComponent<MoveWithGameObject>();
        mW.owner = newFighter;
        
        Unit currentUnit = newFighter.GetComponent<Unit>();
        currentUnit.currentTile = grid.TileFromWorldPoint(spawnLocation);
        currentUnit.currentTile.occupied = true;
        currentUnit.currentTile.occupyingUnit = currentUnit;
        currentUnit.location = currentUnit.currentTile.tilePos;
        currentUnit.team = teamNum;

        return newFighter;
    }
    
}
