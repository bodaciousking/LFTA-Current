using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoading : MonoBehaviour
{
    public static MapLoading instance;
    Init_Placement iP;
    TeamManager tM;
    MapInfo mI;
    Grid grid;
    TeamDisplay tD;
    HowToSaveLoadAnArrayWithCompression hts;

    public Transform tilePrefab, obstaclePrefab;

    public List<GameObject> graphicsList = new List<GameObject>();

    public Tile[,,] tileGrid;
    List<Coord> allTileCoords;
    MapFunctions mF;

    //public string mapCode, spawnCode;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        hts = HowToSaveLoadAnArrayWithCompression.instance;
        mF = MapFunctions.instance;
        //DecodeAndLoad(mapCode, spawnCode);
    }

    public void DecodeAndLoad(string mapCode, string spawnMapCode)
    {
        BasicSect[,,] loadedMap = hts.LoadAnArrayFromString(mapCode);

        bool[][,,] spawnLocations = hts.LoadListFromString(spawnMapCode);
        //TestLoadMap(loadedMap);

        LoadMap(loadedMap, spawnLocations);
    }

    public void TestLoadMap(BasicSect[,,] bSMap)
    {
        Vector3 mapSize = new Vector3(bSMap.GetLength(0), bSMap.GetLength(1), bSMap.GetLength(2));
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    if (bSMap[x, y, z] != null)
                    {
                        BasicSect bS = bSMap[x, y, z];
                        Instantiate(graphicsList[bS.gR], mF.CoordToPosition(x, y, z, mapSize), Quaternion.identity);
                    }
                }
            }
        }
    }


    public void LoadMap(BasicSect[,,] fullMap, bool[][,,] spawnLocationsLists)
    {
        InitializeComponents();
        Vector3 mapSize = new Vector3(fullMap.GetLength(0), fullMap.GetLength(1), fullMap.GetLength(2));
        mI.mapSize = mapSize;

        tileGrid = new Tile[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];

        allTileCoords = new List<Coord>();
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

        Coord mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2, (int)mapSize.z/2);
        Coord[,,] coordMap = new Coord[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];
        coordMap[mapCenter.x, mapCenter.y, mapCenter.z] = mapCenter;


        string holderName = "Loaded Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        List<Coord> openTiles = new List<Coord>();


        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    if (fullMap[x, y, z] != null)
                    {
                        BasicSect bS = fullMap[x, y, z];
                        //Instantiate(graphicsList[bS.gR], mF.CoordToPosition(x, y, z, mapSize), Quaternion.identity);

                        Vector3 tilePosition = mF.CoordToPosition(x, y, z, mapSize);
                        Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                        newTile.parent = mapHolder;
                        Tile tileScript = newTile.GetComponent<Tile>();
                        Color sectColor = JsonUtility.FromJson<Color>(bS.colorString);
                        newTile.GetComponent<Renderer>().material.color = sectColor;
                        foreach (Transform item in newTile.transform)
                        {
                            if (item.GetComponent<Renderer>())
                            {
                                item.GetComponent<Renderer>().material.color = sectColor;
                            }
                        }
                        Coord openTileCoord = new Coord(x, y, z);
                        tileScript.tilePos = mF.CoordToVector3(openTileCoord);
                        tileGrid[x, y, z] = tileScript;

                        openTiles.Add(openTileCoord);
                        coordMap[openTileCoord.x, openTileCoord.y, openTileCoord.z] = openTileCoord;
                    }
                }
            }
        }

        mI.tileGrid = tileGrid;
        grid.CreateGrid();

        //Spawning Teams
        iP.SpawnTeams();


        tD.OrganizeTeams();

        Camera.main.GetComponent<CameraControll>().target.position = mF.CoordToPosition(mapCenter.x, mapCenter.y, mapCenter.z, mapSize);
    }

    void InitializeComponents()
    {
        mF = MapFunctions.instance;
        iP = Init_Placement.instance;
        tM = TeamManager.instance;
        mI = MapInfo.instance;
        grid = Grid.instance;
        hts = HowToSaveLoadAnArrayWithCompression.instance;
        tD = TeamDisplay.instance;
    }
}
