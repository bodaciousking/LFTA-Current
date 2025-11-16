using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class MapPreview : MonoBehaviour
{
    public static MapPreview instance;
    public GameObject previewObj;
    public GameObject repObj, fileObj;
    public GameObject allMapsPanel;
    public string mapCode;
    public MapInfo currentMapInfo;
    TeamPreview tP;
    HowToSaveLoadAnArrayWithCompression hts;

    private void Awake()
    {
        instance = this;
    }
    public void DisplayFilesAsMaps()
    {
        string path = Application.persistentDataPath + "/Maps/";

        foreach (string file in Directory.GetFiles(path))
        {
            //Debug.Log(Path.GetFileName(file));
            GameObject newObj = Instantiate(fileObj, GameObject.Find("SavedMapsPanel").transform);
            TextMeshProUGUI mapNameText = newObj.GetComponentInChildren<TextMeshProUGUI>();
            mapNameText.text = Path.GetFileName(file);
            FileInfoHolder fileInfoScript = newObj.GetComponent<FileInfoHolder>();

            string inputString = File.ReadAllText(file);

            MapInfo.MapData fileMapData = JsonUtility.FromJson<MapInfo.MapData>(inputString);

            fileInfoScript.spawnsString = fileMapData.spawnLocationLists;
            fileInfoScript.mapString = fileMapData.sectData;
        } 
    }

    public void PreviewMap(string sectMapCode, string spawnCode)
    {
        mapCode = sectMapCode;
        sectMapCode.Trim();
        spawnCode.Trim();
        BasicSect[,,] sectMap = hts.LoadAnArrayFromString(sectMapCode);
        bool[][,,] spawnMaps = hts.LoadListFromString(spawnCode);
        Vector3 mapSize = new Vector3(sectMap.GetLength(0), sectMap.GetLength(1), sectMap.GetLength(2));

        currentMapInfo = MapInfo.instance;
        currentMapInfo.mapSize = mapSize;
        currentMapInfo.spawnLocationLists = spawnMaps.ToList();
        currentMapInfo.basicSectArray = sectMap;

        string holderName = "MapPreview(Clone)";
        if (GameObject.Find(holderName))
            DestroyImmediate(GameObject.Find(holderName));

        GameObject holderObj = Instantiate(previewObj, GameObject.Find("MapDisplay").transform);
        GridLayoutGroup myLayout = holderObj.GetComponent<GridLayoutGroup>();
        float width = holderObj.GetComponent<RectTransform>().rect.width;
        float height = holderObj.GetComponent<RectTransform>().rect.height;
        Vector2 newSize = new Vector2(width / mapSize.z, height / mapSize.x);
        if (mapSize.x > mapSize.z)
        {
            myLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            myLayout.constraintCount = (int)mapSize.x;
        }
        else
        {
            myLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            myLayout.constraintCount = (int)mapSize.z;
        }
        myLayout.cellSize = newSize;

        GameObject[,] previewsArray = new GameObject[(int)mapSize.x, (int)mapSize.z];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                GameObject newObj = Instantiate(repObj, transform.position, Quaternion.identity, holderObj.transform);
                Image objImage = newObj.GetComponent<Image>();
                for (int i = 0; i < currentMapInfo.spawnLocationLists.Count; i++)
                {
                    if (currentMapInfo.spawnLocationLists[i][x, 0, z])
                    {
                        objImage.color = Color.black;
                    }
                }
                if(sectMap[x,0,z] == null)
                {
                    objImage.color = Color.clear;
                }
                previewsArray[x, z] = newObj;
            
                for (int y = 1; y < mapSize.y; y++)
                {
                    if ((sectMap[x, y, z] != null))
                    {
                        previewsArray[x, z].GetComponent<Image>().color = Color.grey;
                    }
                }
            }
        }

        tP.SpawnTeamsSlots();
    }

    // Start is called before the first frame update
    void Start()
    {
        tP = TeamPreview.instance;
        hts = HowToSaveLoadAnArrayWithCompression.instance;
        DisplayFilesAsMaps();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
