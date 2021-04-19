using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamCreation : MonoBehaviour
{
    public GameObject unitKey, smallPiano, smallKey, teamsHolderObj;
    public static TeamCreation instance;
    public List<GameObject> heroList = new List<GameObject>();
    public List<GameObject> gruntList = new List<GameObject>();
    public List<GameObject> smallPianoObjects = new List<GameObject>();
    public List<int> unitIntList = new List<int>();
    public TeamManager.Team currentTeam;
    Transform teamsHolder;
    Transform teamDisplayer;


    public GameObject showoff;
    Image profile;
    TextMeshProUGUI name, hp, mp, description;
    TextMeshProUGUI inputField;
    public bool showingHero;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many Team Displays!");
            return;
        }
        instance = this;
    }
    void Start()
    {
        GameObject myObj = GameObject.Find("Canvas");
        AllUnitsLibrary aul = myObj.GetComponent<AllUnitsLibrary>();
        aul.gruntList = gruntList;
        aul.heroList = heroList;
        DisplayHeroes();
        DisplayGrunts();
        currentTeam = CreateTeam();
        DisplayCurrentTeam();

        inputField = GameObject.Find("OmegaText").GetComponent<TextMeshProUGUI>();
        
        string holderName = "AllTeamsPanel";
        //teamsHolder = GameObject.Find(holderName).transform;

        name = GameObject.Find("UnitName").GetComponent<TextMeshProUGUI>();
        hp = GameObject.Find("HP").GetComponent<TextMeshProUGUI>();
        mp = GameObject.Find("MP").GetComponent<TextMeshProUGUI>();
        description = GameObject.Find("Description").GetComponent<TextMeshProUGUI>();
        profile = GameObject.Find("ProfilePic").GetComponent<Image>();
    }

    public void ShowOffUnit(GameObject unit)
    {
        showoff = unit;
        Unit unitScript = unit.GetComponent<Unit>();
        profile.sprite = unitScript.profile;
        name.text = unitScript.unitName;
        hp.text = unitScript.health.y.ToString();
        mp.text = unitScript.mana.y.ToString();
        description.text = unitScript.unitDescription;
    }

    public void DisplayHeroes()
    {
        for (int i = 0; i < heroList.Count; i++)
        {
            Transform holder = GameObject.Find("Heroes").transform;
            GameObject newUnitDisplay = Instantiate(unitKey, holder);

            Image backgroundImage = newUnitDisplay.GetComponent<Image>();
            Image panelImage = newUnitDisplay.GetComponentsInChildren<Image>()[1];

            BasicPanelReferencer bpr = newUnitDisplay.GetComponent<BasicPanelReferencer>();
            bpr.myNum = i;
            bpr.myFighter = heroList[i];
            bpr.adding = true;
            bpr.hero = true;

            panelImage.sprite = heroList[i].GetComponent<Unit>().profile;
            //backgroundImage.color = tM.teams[i].teamColor;
        
        }
    }

    public void DisplayGrunts()
    {
        for (int i = 0; i < gruntList.Count; i++)
        {
            Transform holder = GameObject.Find("Grunts").transform;
            GameObject newUnitDisplay = Instantiate(unitKey, holder);

            Image backgroundImage = newUnitDisplay.GetComponent<Image>();
            Image panelImage = newUnitDisplay.GetComponentsInChildren<Image>()[1];

            BasicPanelReferencer bpr = newUnitDisplay.GetComponent<BasicPanelReferencer>();
            bpr.myFighter = gruntList[i];
            bpr.myNum = heroList.Count + i;
            bpr.adding = true;
            bpr.hero = false;

            panelImage.sprite = gruntList[i].GetComponent<Unit>().profile;
            //backgroundImage.color = tM.teams[i].teamColor;

        }
    }

    public TeamManager.Team CreateTeam()
    {
        string newTeamName = "Team 0";
        Color newTeamColor = new Color();
        newTeamColor = Random.ColorHSV();

        TeamManager.Team newTeam = new TeamManager.Team(newTeamName, newTeamColor);
        
        teamDisplayer = Instantiate(smallPiano, teamsHolder).transform;
        smallPianoObjects.Add(teamDisplayer.gameObject);

        return newTeam;
    }


    public void DisplayCurrentTeam()
    {
        string holderName = "CurrentTeamDisplay";
        Transform teamHolder = GameObject.Find(holderName).transform;
        foreach (Transform child in teamHolder)
        {
            foreach (Transform child2 in child)
            {
                Destroy(child2.gameObject);
            }
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentTeam.units.Count; i++)
        {
            GameObject newUnitDisplay = Instantiate(unitKey, teamHolder);

            Image backgroundImage = newUnitDisplay.GetComponent<Image>();
            Image panelImage = newUnitDisplay.GetComponentsInChildren<Image>()[1];

            BasicPanelReferencer bpr = newUnitDisplay.GetComponent<BasicPanelReferencer>();
            bpr.myFighter = currentTeam.units[i];
            bpr.adding = false;

            panelImage.sprite = currentTeam.units[i].GetComponent<Unit>().profile;
            backgroundImage.color = currentTeam.teamColor;
        }
    }

    public void AddUnit()
    {
        if (showingHero)
        {
            if (!currentTeam.units.Contains(showoff))
            {
                currentTeam.units.Add(showoff);
                unitIntList.Add(showoff.GetComponent<Unit>().unitID);
                DisplayCurrentTeam();
            }
            else
                Debug.Log("That hero already exists!");
        }
        else
        {
            currentTeam.units.Add(showoff);
            DisplayCurrentTeam();
        }
    }
    public void RemoveUnit(GameObject unit)
    {
        currentTeam.units.Remove(unit);
        DisplayCurrentTeam();
    }

    public void ConvertUnitListToNums()
    {
        currentTeam.unitNums = new int[currentTeam.units.Count];
        for (int i = 0; i < currentTeam.units.Count; i++)
        {
            currentTeam.unitNums[i] = currentTeam.units[i].GetComponent<Unit>().unitID;
        }
    }

    public void ExportTeam()
    {
        ConvertUnitListToNums();
        string teamString = JsonUtility.ToJson(currentTeam);

        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Teams/"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Teams/");
        }
        string path = Application.persistentDataPath + "/Teams/" + currentTeam.teamName;
        File.WriteAllText(path, teamString);


        Debug.Log("Exported a file at " + path);
        Debug.Log("File text: " + teamString);

    }

    private void Update()
    {
        currentTeam.teamName = inputField.text;
    }
}
