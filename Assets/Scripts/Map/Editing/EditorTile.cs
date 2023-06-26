using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EditorTile : MonoBehaviour
{
    [HideInInspector]
    public MapEditing mE;
    public Sect mySect;

    public MapFunctions mF;
    public Material myMat;
    public Material hoverMat;
    bool isSpawnPoint = false;
    public int spawnPointTeam = -1;
    [HideInInspector]
    public TextMeshPro text;
    public GameObject myTextObj;

    private void Start()
    {
        if(GetComponent<Renderer>())
            myMat = GetComponent<Renderer>().material;
        else
            myMat = GetComponentInChildren<Renderer>().material;

        text = myTextObj.GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        text.text = spawnPointTeam.ToString();
        if (spawnPointTeam == -1)
        {
            myTextObj.SetActive(false);
        }
        else
            myTextObj.SetActive(true);
    }

    private void OnMouseEnter()
    {
    }


    private void OnMouseExit()
    {
    }

    private void OnMouseDown()
    {
        
    }


}
