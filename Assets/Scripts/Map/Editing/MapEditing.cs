using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class MapEditing : MonoBehaviour
{
    public static MapEditing instance;
    MapFunctions mF;
    MapLoading mL;
    MapInfo mI;
    EditingUI eUI;
    HowToSaveLoadAnArrayWithCompression hts;

    TMP_InputField x, y, z;
    bool mapsizeChosen;
    public float mapBottom;

    public Transform previewSectsHolder;
    public Coord[,,] allTileCoords;

    E_TeamSpawns e_ts;
    E_ModelTerrain e_mt;
    E_Colors e_c;

    public Choice action;

    public Transform tilePrefab;

    EditorTile hitTile;
    EditorTile previousTile;
    HitFace faceHit;

    TMP_InputField mapNameField;
    public string mapName;
    public GameObject finalizeButton;
    public Material hoverMat;
    public Transform mapHolder;

    public BasicSect[,,] basicSectArray;
    public Sect[,,] sectArray;

    private void Start()
    {
        mF = MapFunctions.instance;
        mL = MapLoading.instance;
        eUI = EditingUI.instance;
        mI = MapInfo.instance;
        e_mt = E_ModelTerrain.instance;
        e_c = E_Colors.instance;
        e_ts = E_TeamSpawns.instance;

        x = GameObject.Find("XField").GetComponent<TMP_InputField>();
        y = GameObject.Find("YField").GetComponent<TMP_InputField>();
        z = GameObject.Find("ZField").GetComponent<TMP_InputField>();
        mapNameField = GameObject.Find("NameField").GetComponent<TMP_InputField>();

        hts = HowToSaveLoadAnArrayWithCompression.instance;

        previewSectsHolder = GameObject.Find("PreviewSectsHolder").transform;

        string holderName = "Base Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        
    }

    private void Update()
    {
        if (x.text != null && y.text != null && z.text != null)
        {
            mI.mapSize = new Vector3(int.Parse(x.text), int.Parse(y.text), int.Parse(z.text));
            if (!mapsizeChosen && mapNameField.text != "")
            {
                finalizeButton.SetActive(true);
            }
        }
        else
            finalizeButton.SetActive(false);

        if (action != Choice.Placing && previewSectsHolder.childCount > 0)
            e_mt.DestroyPreviewSect();

        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 8;
        if (Physics.Raycast(ray, out hit, 5000, layerMask))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (hit.collider.gameObject.GetComponent<EditorTile>())
                    hitTile = hit.collider.gameObject.GetComponent<EditorTile>();
                else
                    hitTile = hit.collider.gameObject.GetComponentInChildren<EditorTile>();

                if (action == MapEditing.Choice.Placing)
                {
                    previousTile = null;

                    faceHit = GetHitFace(hit);
                    if (previousTile != null)
                    {
                        if (previousTile != hitTile)
                            e_mt.PreviewNewSect(hitTile.mySect.coord, faceHit);
                    }
                    else if (previousTile == null)
                        e_mt.PreviewNewSect(hitTile.mySect.coord, faceHit);

                    previousTile = hitTile;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (action == MapEditing.Choice.Placing && hitTile.spawnPointTeam == -1)
                    {
                        e_mt.AddNewSect(hitTile.mySect.coord, faceHit);
                    }

                    else if (action == MapEditing.Choice.ChoosingSpawnSpots && (hitTile.spawnPointTeam == -1 || hitTile.spawnPointTeam == e_ts.teamToAddTo))
                    {
                        if (e_ts.ToggleSpawnPoint(hitTile.mySect.coord))
                        {
                            if (hitTile.spawnPointTeam == -1)
                                hitTile.spawnPointTeam = e_ts.teamToAddTo;
                            else if (hitTile.spawnPointTeam == e_ts.teamToAddTo)
                                hitTile.spawnPointTeam = -1;
                        }
                    }
                    else if (action == MapEditing.Choice.Recolor)
                    {
                        e_c.Paint(hitTile.mySect);
                        hitTile.myMat.color = e_c.modelColor;
                    }

                    else if (action == MapEditing.Choice.Erase)
                    {
                        e_mt.EraseSect(hitTile.mySect.coord);
                        Destroy(hitTile.gameObject);
                    }
                }
            }
            else
                e_mt.DestroyPreviewSect();
        }
        else
            e_mt.DestroyPreviewSect();
    }
    public enum HitFace
    {
        None,
        Up,
        Down,
        East,
        West,
        North,
        South
    }

    public HitFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;

        if (incomingVec == new Vector3(0, -1, -1))
        {
            return HitFace.South;
        }

        if (incomingVec == new Vector3(0, -1, 1))
        {
            return HitFace.North;
        }

        if (incomingVec == new Vector3(0, 0, 0))
        {
            return HitFace.Up;
        }

        if (incomingVec == new Vector3(0, -2, 0))
        {
            return HitFace.Down;
        }

        if (incomingVec == new Vector3(-1, -1, 0))
        {
            return HitFace.West;
        }

        if (incomingVec == new Vector3(1, -1, 0))
        {
            return HitFace.East;
        }

        return HitFace.None;
    }


    private void Awake()
    {
        instance = this;
    }

    public enum Choice
    {
        Placing,
        ChoosingSpawnSpots,
        Recolor,
        Erase,
    }



    public void ChooseSpawns()
    {
        action = Choice.ChoosingSpawnSpots;
    }
    public void ChoosePlacing()
    {
        action = Choice.Placing;
    }
    public void ChooseRecolor()
    {
        action = Choice.Recolor;
    }
    public void ChooseErase()
    {
        action = Choice.Erase;
    }

    public void CreateBaseMap()
    {
        mapsizeChosen = true;
        bool firstCheck = false;
        mapName = mapNameField.text;
        Vector3 mapSize = mI.mapSize;
        mI.mapSize = mapSize;

        allTileCoords = new Coord[(int)mI.mapSize.x, (int)mI.mapSize.y, (int)mI.mapSize.z];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Coord newCoord = new Coord(x, y, z);
                    allTileCoords[x, y, z] = newCoord;
                }
            }
        }

        Coord mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2, (int)mapSize.z / 2);
        Vector3 centerPos = mF.CoordToPosition(mapCenter.x, mapCenter.y, mapCenter.z, mapSize);
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
                    Sect newSect = e_mt.CreateNewSect(thisCoord);

                    Coord openTileCoord = thisCoord;
                    openTiles.Add(openTileCoord);
                    coordMap[openTileCoord.x, openTileCoord.y, openTileCoord.z] = openTileCoord; 

                    if (!firstCheck)
                    {
                        firstCheck = true;
                        mapBottom = mF.CoordToPosition(openTileCoord.x, openTileCoord.y, openTileCoord.z, mI.mapSize).y;
                        CameraControll cC = Camera.main.GetComponent<CameraControll>();
                        cC.minYLocation = (int)mapBottom;
                        cC.target.position = new Vector3(centerPos.x, (int)mapBottom, centerPos.z);
                    }
                }
            }
        }
        eUI.SetOptionsButtons(true);
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
