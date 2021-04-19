using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MapEditing : MonoBehaviour
{
    public static MapEditing instance;
    MapFunctions mF;
    MapLoading mL;
    MapInfo mI;
    EditingUI eUI;
    HowToSaveLoadAnArrayWithCompression hts;
    public Transform teamPanel;
    TMP_InputField x, y, z;
    bool mapsizeChosen;

    public Transform tilePrefab, obstaclePrefab;
    public int teamToAddTo;
    int graphicsReference = 0;

    public Vector3 chosenMapSize;
    public Vector3 brushSize = Vector3.one;
    TMP_InputField mapNameField;
    public string mapName;
    public GameObject finalizeButton;
    Transform mapHolder;

    BasicSect[,,] basicSectArray;
    Sect[,,] sectArray;

    float outlinePercent = 0.9f;

    private void Start()
    {
        mF = MapFunctions.instance;
        mL = MapLoading.instance;
        eUI = EditingUI.instance;
        mI = MapInfo.instance;
        x = GameObject.Find("XField").GetComponent<TMP_InputField>();
        y = GameObject.Find("YField").GetComponent<TMP_InputField>();
        z = GameObject.Find("ZField").GetComponent<TMP_InputField>();
        mapNameField = GameObject.Find("NameField").GetComponent<TMP_InputField>();
        hts = HowToSaveLoadAnArrayWithCompression.instance;

        string holderName = "Base Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        graphicsReference = 1;
    }

    private void Update()
    {
        if (x.text != null && y.text != null && z.text != null)
        {
            chosenMapSize = new Vector3(int.Parse(x.text), int.Parse(y.text), int.Parse(z.text));
            if (!mapsizeChosen && mapNameField.text != "")
            {
                finalizeButton.SetActive(true);
            }
        }
        else
            finalizeButton.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
    }

    public void AddNewTeamPointList()
    {
        bool[,,] newTeamList = new bool[(int)chosenMapSize.x,(int)chosenMapSize.y, (int)chosenMapSize.z];
        mI.spawnLocationLists.Add(newTeamList);

        Transform objectParent = GameObject.Find("TeamPanel").transform;
        GameObject newPanel = Instantiate(teamPanel, objectParent).gameObject;
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
        action = Choice.ChoosingSpawnSpots;
    }

    public bool ToggleSpawnPoint(Coord CoordToAdd)
    {
        bool success = false;
        if (CoordToAdd.y + 1 < chosenMapSize.y)
        {
            if (basicSectArray[CoordToAdd.x, CoordToAdd.y + 1, CoordToAdd.z] == null)
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

    public enum Choice
    {
        Placing,
        ChoosingSpawnSpots,
        Recolor,
    }

    public Choice action;


    public void ChooseSpawns()
    {
        action = Choice.ChoosingSpawnSpots;
    }

    public void ChooseObstacles()
    {
        action = Choice.Placing;
    }

    public void ChooseRecolor()
    {
        action = Choice.Recolor;
    }

    public void CreateBaseMap()
    {
        mapsizeChosen = true;
        mapName = mapNameField.text;
        Vector3 mapSize = chosenMapSize;
        mI.mapSize = mapSize;

        List<Coord> allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                { 
                    allTileCoords.Add(new Coord(x, y, z));
                }
            }
        }

        Coord mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2, (int)mapSize.z / 2);
        Coord[,,] coordMap = new Coord[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];
        coordMap[mapCenter.x, mapCenter.y, mapCenter.z] = mapCenter;

        basicSectArray = new BasicSect[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];
        sectArray = new Sect[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];

        List<Coord> openTiles = new List<Coord>();


        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                for (int y = 0; y <= 0; y++)
                {
                    Coord thisCoord = new Coord(x, y, z);
                    Sect newSect = CreateNewSect(thisCoord, true);

                    Coord openTileCoord = thisCoord;
                    openTiles.Add(openTileCoord);
                    coordMap[openTileCoord.x, openTileCoord.y, openTileCoord.z] = openTileCoord;

                }
            }
        }
    }
    public Sect CreateNewSect(Coord info, bool addRandomColor)
    {
        string colorString = JsonUtility.ToJson(eUI.modelColor);
        BasicSect newBS = new BasicSect(info.x, info.y, info.z, graphicsReference, colorString);

        Sect newSect = new Sect(mF.CoordToPosition(info.x, info.y, info.z, chosenMapSize), mF.CoordToVector3(info),mF.sectModel,info, newBS);
        newSect.graphicsReference = graphicsReference;
        newSect.graphics = mF.CreateSectGraphic(newSect);
        newSect.graphics.transform.localScale = Vector3.one * outlinePercent;
        newSect.graphics.transform.parent = mapHolder;
        newSect.ChangeColor(eUI.modelColor);

        EditorTile tileScript = newSect.graphics.GetComponent<EditorTile>();
        tileScript.mySect = newSect;
        tileScript.mE = this;
        tileScript.mF = mF;


        basicSectArray[(int)newSect.simpleLocation.x, (int)newSect.simpleLocation.y, (int)newSect.simpleLocation.z] = newBS;
        sectArray[(int)newSect.simpleLocation.x, (int)newSect.simpleLocation.y, (int)newSect.simpleLocation.z] = newSect;

        return (newSect);
    }
    public void AddNewSect(Coord prevCoord)
    {
        if (prevCoord.y + 1 <= chosenMapSize.y - 1)
        {
            if (basicSectArray[prevCoord.x, prevCoord.y + 1, prevCoord.z] == null)
            {
                Coord newCoord = new Coord(prevCoord.x, prevCoord.y + 1, prevCoord.z);
                CreateNewSect(newCoord, false);
            }
        }
    }
    public void ChangeXBrushSize(int change)
    {
        brushSize.x = brushSize.x + change;
        if (brushSize.x < 0)
            brushSize = new Vector3(0, brushSize.y, brushSize.z);
    }
    public void ChangeYBrushSize(int change)
    {
        brushSize.y = brushSize.y + change;
        if (brushSize.y < 0)
            brushSize = new Vector3(brushSize.x, 0, brushSize.z);
    }
    public void ChangeZBrushSize(int change)
    {
        brushSize.z = brushSize.z + change;
        if (brushSize.z < 0)
            brushSize = new Vector3(brushSize.x, brushSize.y, 0);
    }

    public void Paint(Sect originSect)
    {
        Debug.Log(originSect.simpleLocation);
        for (int i = 0; i <= brushSize.x; i++)
        {
            for (int j = 0; j <= brushSize.y; j++)
            {
                for (int k = 0; k <= brushSize.z; k++)
                {
                    if ((int)originSect.simpleLocation.x + i > chosenMapSize.x-1 || (int)originSect.simpleLocation.y + j > chosenMapSize.y-1 || (int)originSect.simpleLocation.z + k > chosenMapSize.z-1)
                    {
                        continue;
                    }
                    else if (basicSectArray[(int)originSect.simpleLocation.x + i, (int)originSect.simpleLocation.y + j, (int)originSect.simpleLocation.z + k] != null)
                    {
                        Sect sect = sectArray[(int)originSect.simpleLocation.x + i, (int)originSect.simpleLocation.y + j, (int)originSect.simpleLocation.z + k];
                        sect.ChangeColor(eUI.modelColor);
                    }
                }
            }
        }

    }


    public void ExportMap()
    {
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Maps/"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Maps/");
        }

        string mapDataSpawnListString = hts.GenerateStringFromList(mI.spawnLocationLists);

        string sectDataString = hts.GenerateAStringFromSect(basicSectArray);

        MapInfo.MapData myMapData = new MapInfo.MapData(mapName, sectDataString, mapDataSpawnListString, mI.mapSize);

        string mapDataJsonString = JsonUtility.ToJson(myMapData);

        string path = Application.persistentDataPath + "/Maps/" + mapName;
        string filestring = mapDataJsonString;
        File.WriteAllText(path, filestring);

        //Debug Reading Strings
        //Debug.Log(mapDataJsonString);
        //string inputString = File.ReadAllText(path);

        MenuNavigation mN = MenuNavigation.instance;
        mN.GoToMenuScene();
    }


}
