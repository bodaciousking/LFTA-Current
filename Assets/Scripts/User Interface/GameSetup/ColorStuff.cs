using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorStuff : MonoBehaviour
{
    TeamCreation tC;
    EditingUI eUI;
    Image myImage;

    public GameObject picker, roster;
    // Start is called before the first frame update
    void Start()
    {
        tC = TeamCreation.instance;
        eUI = EditingUI.instance;
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tC)
        {
            myImage.color = tC.currentTeam.teamColor;
        }
        else if (eUI)
        {
            myImage.color = eUI.modelColor;
        }
    }

    public void ToggleColorPicker()
    {
        picker.SetActive(!picker.activeSelf);
        roster.SetActive(!roster.activeSelf);
    }
}
