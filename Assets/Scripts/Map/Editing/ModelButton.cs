using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelButton : MonoBehaviour
{
    public GameObject myModel;
    public Image previewImage;
    Image myImage;
    MapFunctions mF;

    private void Start()
    {
        mF = MapFunctions.instance;
        myImage = GetComponent<Image>();
        myImage.sprite = previewImage.sprite;
    }
    public void SelectModel()
    {
        mF.sectModel = myModel;
    }
}
