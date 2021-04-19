using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    UnitSelection uS;
    Camera mainCam;
    public List<GameObject> buttons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        uS = UnitSelection.instance;
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (uS.selectedUnit)
        {
            float distance = Vector3.Distance(mainCam.transform.position, uS.selectedUnit.transform.position);

            transform.position = Camera.main.WorldToScreenPoint(uS.selectedUnit.transform.Find("UI Orient").transform.position);

            //Debug.Log(distance);
            if (distance < 7.5f && distance > 6.5f)
            foreach (GameObject button in buttons)
            {
                button.transform.localScale = Vector3.one * distance/8;
            }
        }
    }
}
