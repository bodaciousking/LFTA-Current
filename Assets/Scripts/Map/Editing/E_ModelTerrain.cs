using UnityEngine;

public class E_ModelTerrain : MonoBehaviour
{

    public static E_ModelTerrain instance;

    EditingUI eUI;
    E_Colors eC;
    MapFunctions mF;
    MapInfo mI;
    MapEditing mE;

    public int graphicsReference = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many instances of ModelTerrain! " + instance.gameObject.name);
            Destroy(instance.gameObject);
            instance = this;
            return;
        }
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eUI = EditingUI.instance;
        eC = E_Colors.instance;
        mF = MapFunctions.instance;
        mI = MapInfo.instance;
        mE = MapEditing.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Sect CreateNewSect(Coord info)
    {
        string colorString = JsonUtility.ToJson(eC.modelColor);
        BasicSect newBS = new BasicSect(info.x, info.y, info.z, graphicsReference, colorString);

        Sect newSect = new Sect(mF.CoordToPosition(info.x, info.y, info.z, mI.mapSize), mF.CoordToVector3(info), mF.sectModel, info, newBS);
        newSect.graphicsReference = graphicsReference;
        newSect.graphics = mF.CreateSectGraphic(newSect);
        newSect.graphics.transform.parent = mE.mapHolder;
        newSect.ChangeColor(eC.modelColor);

        EditorTile tileScript;
        if (newSect.graphics.GetComponent<EditorTile>() != null)
            tileScript = newSect.graphics.GetComponent<EditorTile>();
        else
            tileScript = newSect.graphics.GetComponentInChildren<EditorTile>();

        tileScript.mySect = newSect;
        tileScript.mE = mE;
        tileScript.mF = mF;


        mE.basicSectArray[(int)newSect.simpleLocation.x, (int)newSect.simpleLocation.y, (int)newSect.simpleLocation.z] = newBS;
        mE.sectArray[(int)newSect.simpleLocation.x, (int)newSect.simpleLocation.y, (int)newSect.simpleLocation.z] = newSect;

        return (newSect);
    }

    public void EraseSect(Coord info)
    {
        mE.basicSectArray[info.x, info.y, info.z] = null;
        mE.sectArray[info.x, info.y, info.z] = null;


    }
    public void DestroyPreviewSect()
    {
        foreach (Transform item in mE.previewSectsHolder)
        {
            Destroy(item.gameObject);
        }
    
    }

    public void PreviewNewSect(Coord prevCoord, MapEditing.HitFace faceHit)
    {
        foreach (Transform item in mE.previewSectsHolder)
        {
            Destroy(item.gameObject);
        }
        if (faceHit == MapEditing.HitFace.Up)
        {
            if (prevCoord.y + 1 <= mI.mapSize.y - 1)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y + 1, prevCoord.z] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x, prevCoord.y + 1, prevCoord.z];
                    CreatePreview(coord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.Down)
        {
            if (prevCoord.y - 1 >= 0)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y - 1, prevCoord.z] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x, prevCoord.y - 1, prevCoord.z];
                    CreatePreview(coord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.North)
        {
            if (prevCoord.z + 1 < mI.mapSize.z - 1)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y, prevCoord.z + 1] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x, prevCoord.y, prevCoord.z + 1];
                    CreatePreview(coord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.South)
        {
            if (prevCoord.z - 1 >= 0)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y, prevCoord.z - 1] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x, prevCoord.y, prevCoord.z - 1];
                    CreatePreview(coord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.East)
        {
            if (prevCoord.x + 1 < mI.mapSize.x - 1)
            {
                if (mE.basicSectArray[prevCoord.x + 1, prevCoord.y, prevCoord.z] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x + 1, prevCoord.y, prevCoord.z];
                    CreatePreview(coord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.West)
        {
            if (prevCoord.x - 1 >= 0)
            {
                if (mE.basicSectArray[prevCoord.x - 1, prevCoord.y, prevCoord.z] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x - 1, prevCoord.y, prevCoord.z];
                    CreatePreview(coord);
                }
            }
        }

    }

    public void CreatePreview(Coord coord)
    {
        GameObject preview = Instantiate(mF.sectModel, mF.CoordToPosition(coord.x, coord.y, coord.z, mI.mapSize), Quaternion.identity, mE.previewSectsHolder);
        Destroy(preview.GetComponent<EditorTile>());
        Destroy(preview.GetComponentInChildren<EditorTile>());
        preview.layer = 2;
        preview.transform.localScale *= 0.9f;

        foreach (Transform item in preview.transform)
        {
            item.gameObject.layer = 2;
        }
        foreach (Transform item in preview.transform)
        {
            item.GetComponentInChildren<Renderer>().material = mE.hoverMat;
        }
    }
    public void AddNewSect(Coord prevCoord, MapEditing.HitFace faceHit)
    {
        if (faceHit == MapEditing.HitFace.Up)
        {
            if (prevCoord.y + 1 <= mI.mapSize.y - 1)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y + 1, prevCoord.z] == null)
                {
                    Coord newCoord = mE.allTileCoords[prevCoord.x, prevCoord.y + 1, prevCoord.z];
                    CreateNewSect(newCoord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.Down)
        {
            Debug.Log("A");
            if (prevCoord.y - 1 >= 0)
            {
                Debug.Log("B");
                if (mE.basicSectArray[prevCoord.x, prevCoord.y - 1, prevCoord.z] == null)
                {
                    Coord newCoord = mE.allTileCoords[prevCoord.x, prevCoord.y - 1, prevCoord.z];
                    CreateNewSect(newCoord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.North)
        {
            if (prevCoord.z + 1 < mI.mapSize.z - 1)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y, prevCoord.z + 1] == null)
                {
                    Coord newCoord = mE.allTileCoords[prevCoord.x, prevCoord.y, prevCoord.z + 1];
                    CreateNewSect(newCoord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.South)
        {
            if (prevCoord.z - 1 >= 0)
            {
                if (mE.basicSectArray[prevCoord.x, prevCoord.y, prevCoord.z - 1] == null)
                {
                    Coord coord = mE.allTileCoords[prevCoord.x, prevCoord.y, prevCoord.z - 1];
                    CreateNewSect(coord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.East)
        {
            if (prevCoord.x + 1 < mI.mapSize.x - 1)
            {
                if (mE.basicSectArray[prevCoord.x + 1, prevCoord.y, prevCoord.z] == null)
                {
                    Coord newCoord = mE.allTileCoords[prevCoord.x + 1, prevCoord.y, prevCoord.z];
                    CreateNewSect(newCoord);
                }
            }
        }
        else if (faceHit == MapEditing.HitFace.West)
        {
            if (prevCoord.x - 1 >= 0)
            {
                if (mE.basicSectArray[prevCoord.x - 1, prevCoord.y, prevCoord.z] == null)
                {
                    Coord newCoord = mE.allTileCoords[prevCoord.x - 1, prevCoord.y, prevCoord.z];
                    CreateNewSect(newCoord);
                }
            }
        }

    }
}
