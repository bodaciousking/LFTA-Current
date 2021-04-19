using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;
    public List<Team> teams = new List<Team>();
    public Team winningTeam;
    Victory v;
    TeamDisplay tD;
    [SerializeField]
    int numTeams;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many team managers!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        tD = TeamDisplay.instance;
        TestFunction();

        v = GetComponent<Victory>();
    }

    private void Update()
    {
        numTeams = teams.Count;
    }
    public void OnUnitDied()
    {
        //Is the game over?
        if (!v.gameOver)
        {
            int losingTeams = 0;
            for (int i = 0; i < teams.Count; i++)
            {
                if (DidLose(teams[i]))
                {
                    losingTeams++;
                }
            }
            if (losingTeams == teams.Count - 1)
            {
                for (int i = 0; i < teams.Count; i++)
                {
                    if (DidLose(teams[i]) == false)
                    {
                        winningTeam = teams[i];
                        v.WinnerDecided();

                    }
                }
            }
        }
    }

    public bool DidLose(Team team)
    {
        bool lost = true;
        foreach (GameObject f in team.units)
        {
            Unit unit = f.GetComponent<Unit>();
            if (unit.alive == true)
            {
                lost = false;
                break;
            }
        }
        return lost;
    }

    public void CreateTeam()
    {
        string newTeamName = "Team " + (teams.Count);
        Color newTeamColor = new Color();
        newTeamColor = Random.ColorHSV();

        Team newTeam = new Team(newTeamName, newTeamColor);
        newTeam.teamID = teams.Count;
        teams.Add(newTeam);
    }


    public struct Team
    {
        public string teamName;
        public int[] unitNums;
        public Color teamColor;
        public List<GameObject> units;
        public int teamID;
        public int teamCost;

        public Team(string _teamName, Color _teamColor)
        {
            teamName = _teamName;
            teamColor = _teamColor;
            teamID = 0;
            units = new List<GameObject>();
            unitNums = new int[0];
            teamCost = 0;
        }

    }

    public void TestFunction()
    { }
}