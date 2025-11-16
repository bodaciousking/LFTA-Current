using JetBrains.Annotations;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamPreview : MonoBehaviour
{
    MapPreview mapPreview;
    public GameObject teamDisplayObj, teamsPreviewer, unitDisplay, teamFileObj, savedTeamsPanel;
    public List<GameObject> slotObjects = new List<GameObject>();
    MapInfo mI;
    public static TeamPreview instance;
    public bool initialized;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Too many Team Preview's!");
            return;
        }
        instance = this;
    }

    public void SpawnTeamsSlots()
    {
        int teamSlots = mI.spawnLocationLists.Count;
        slotObjects.Clear();
        mI.teams.Clear();

        if (GameObject.Find("TeamsPreviewHolder(Clone)"))
            DestroyImmediate(GameObject.Find("TeamsPreviewHolder(Clone)"));

        GameObject teamsPreview = Instantiate(teamsPreviewer, GameObject.Find("AllTeamsPanel").transform);

        for (int i = 0; i < teamSlots; i++)
        {
            GameObject newTeamPanel = Instantiate(teamDisplayObj, teamsPreview.transform);
            newTeamPanel.name = "Slot " + i.ToString();

            TextMeshProUGUI textObj = newTeamPanel.GetComponentInChildren<TextMeshProUGUI>();
            textObj.text = "Slot " + i.ToString();

            GameObject button = newTeamPanel.transform.Find("Button").gameObject;
            button.SetActive(false);

            slotObjects.Add(newTeamPanel);
        }


    }

    public void UpdateTeamsSlots()
    {
        foreach (Transform item in GameObject.Find("TeamsPreviewHolder(Clone)").transform)
        {
            if (!item.name.Contains("Slot"))
                Destroy(item.gameObject);
        }
        for (int i = 0; i < mI.teams.Count; i++)
        {
            GameObject newPiano = Instantiate(teamDisplayObj, GameObject.Find("TeamsPreviewHolder(Clone)").transform);
            newPiano.transform.SetSiblingIndex(slotObjects[i].transform.GetSiblingIndex() + 1);
            newPiano.GetComponent<Image>().color = mI.teams[i].teamColor;

            TextMeshProUGUI textObj = newPiano.GetComponentInChildren<TextMeshProUGUI>();
            //textObj.text = "Slot " + i.ToString();

            slotObjects[i].SetActive(false);


            TeamPiano teamPianoScript = newPiano.GetComponent<TeamPiano>();
            teamPianoScript.myTeam = mI.teams[i];
            teamPianoScript.Refresh();
        }
        for (int i = 0; i < mI.spawnLocationLists.Count; i++)
        {
            if(i >= mI.teams.Count)
            {
                slotObjects[i].SetActive(true);
            }
        }
    }

    public void DisplayFilesAsTeams()
    {
        foreach(Transform child in GameObject.Find("SavedTeamsPanel").transform)
        {
            Destroy(child.gameObject);
        }

        string path = Application.persistentDataPath + "/Teams/";

        foreach (string file in Directory.GetFiles(path))
        {
            //Debug.Log(Path.GetFileName(file));
            GameObject newObj = Instantiate(teamFileObj, GameObject.Find("SavedTeamsPanel").transform);
            TextMeshProUGUI teamNameText = newObj.GetComponentInChildren<TextMeshProUGUI>();
            teamNameText.text = Path.GetFileName(file);
            FileInfoHolder fileInfoScript = newObj.GetComponent<FileInfoHolder>();

            string teamString = File.ReadAllText(file);

            TeamManager.Team newTeam = JsonUtility.FromJson<TeamManager.Team>(teamString);
            fileInfoScript.teamString = teamString;
            fileInfoScript.myTeam = newTeam;
            newObj.GetComponent<Image>().color = newTeam.teamColor;
        }
        
    }

    public void AddTeam(TeamManager.Team team)
    {
        if (mI.teams.Count == mI.spawnLocationLists.Count)
        { 
            Debug.Log("Map is full! Cannot add team. Remove a team first.");
            return;
        }
        mI.teams.Add(team);
        UpdateTeamsSlots();
    }

    public void RemoveTeam(TeamManager.Team team)
    {
        if (mI.teams.Contains(team))
        {
            mI.teams.Remove(team);
            Destroy(GameObject.Find(team.teamName));
            UpdateTeamsSlots();
        }
        else
            Debug.Log("Error! Trying to remove a team that does not exist!");
    }
    public void UpdateTeamsFromJsonList(IReadOnlyList<string> jsonList)
    {
        mI.teams.Clear();

        foreach (string teamJson in jsonList)
        {
            TeamManager.Team team = JsonUtility.FromJson<TeamManager.Team>(teamJson);
            mI.teams.Add(team);
        }

        UpdateTeamsSlots(); // Your existing method
    }
    void Start()
    {
        mapPreview = MapPreview.instance;
        mI = MapInfo.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
