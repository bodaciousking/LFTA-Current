using UnityEngine;

public class B_Spawns : MonoBehaviour
{
    E_TeamSpawns e_ts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        e_ts = E_TeamSpawns.instance;
    }

    public void AddTeam()
    {
        e_ts.AddNewTeamPointList();
    }

    public void RemoveTeam()
    {
        e_ts.RemoveTeamPointList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
