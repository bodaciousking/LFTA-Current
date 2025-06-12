using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class E_Colors : MonoBehaviour
{
    public static E_Colors instance;
    FlexibleColorPicker fCP;
    public Color modelColor;

    public MapInfo mI;
    public MapEditing mE;
    EditingUI eUI;

    public Vector3 brushSize = Vector3.zero;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many Color Editors!");
            return;
        }
        instance = this;
    }
    void Start()
    {
        fCP = GameObject.Find("p_colors").GetComponent<FlexibleColorPicker>();
        eUI = EditingUI.instance;
        mE = MapEditing.instance;
        mI = MapInfo.instance; 
    }
    void Update()
    {
        
    }
    public void ChangeBrushSize(Vector3 size)
    {
        brushSize = size;
    }

    public void ChangeXBrushSize(int change)
    {
        brushSize.x = brushSize.x + change - 1;
        if (brushSize.x < 0)
            brushSize = new Vector3(0, brushSize.y, brushSize.z);
    }
    public void ChangeYBrushSize(int change)
    {
        brushSize.y = brushSize.y + change - 1;
        if (brushSize.y < 0)
            brushSize = new Vector3(brushSize.x, 0, brushSize.z);
    }
    public void ChangeZBrushSize(int change)
    {
        brushSize.z = brushSize.z + change - 1;
        if (brushSize.z < 0)
            brushSize = new Vector3(brushSize.x, brushSize.y, 0);
    }

    public void Paint(Sect originSect)
    {
        List<Sect> sectsToPaint = GetNeighbours(originSect);
        for (int i = 0; i < sectsToPaint.Count; i++)
        {
            Sect sect = sectsToPaint[i];
            sect.ChangeColor(modelColor);
        }
    }


    public List<Sect> GetNeighbours(Sect node)
    {
        List<Sect> neighbours = new List<Sect>();
        neighbours.Add(node);
        for (int x = (int)-brushSize.x; x <= (int)brushSize.x; x++)
        {
            for (int y = (int)-brushSize.y; y <= (int)brushSize.y; y++)
            {
                for (int z = (int)-brushSize.z; z <= (int)brushSize.z; z++)
                {
                    //        if (x == 0 && y == 0 && z == 0 || y != 0 && x == 0 && z == 0 || x != 0 && z != 0)
                    //            continue;

                    int checkX = (int)node.simpleLocation.x + x;
                    int checkY = (int)node.simpleLocation.y + y;
                    int checkZ = (int)node.simpleLocation.z + z;

                    if (checkX >= 0 && checkX < mI.mapSize.x && checkY >= 0 && checkY < mI.mapSize.y && checkZ >= 0 && checkZ < mI.mapSize.z)
                    {
                        if (mE.sectArray[checkX, checkY, checkZ] != null)
                            neighbours.Add(mE.sectArray[checkX, checkY, checkZ]);
                    }
                }
            }
        }
        return neighbours;
    }
}
