using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelButton : MonoBehaviour
{
    public GameObject myModel;
    public Sprite previewSprite;
    public int myReference;
    MapFunctions mF;
    MapEditing mE;

    private void Start()
    {
        mF = MapFunctions.instance;
        mE = MapEditing.instance;
    }
    public void Init()
    {
        GetComponent<Image>().sprite = previewSprite;
    }
    public void SelectModel()
    {
        mF.sectModel = myModel;
        mE.ChoosePlacing();
        mE.graphicsReference = myReference;
    }
    private void Update()
    {
        
    }
}
