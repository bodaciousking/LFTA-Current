using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class B_Models : MonoBehaviour
{
    public GameObject myModel;
    public Sprite previewSprite;
    public int myReference;
    MapFunctions mF;
    MapEditing mE;
    E_ModelTerrain e_mT;

    private void Start()
    {
        mF = MapFunctions.instance;
        mE = MapEditing.instance;
        e_mT = E_ModelTerrain.instance;
    }
    public void Init()
    {
        GetComponent<Image>().sprite = previewSprite;
    }
    public void SelectModel()
    {
        mF.sectModel = myModel;
        mE.ChoosePlacing();
        e_mT.graphicsReference = myReference;
    }
    private void Update()
    {
        
    }
}
