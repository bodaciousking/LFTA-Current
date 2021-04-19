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
    Material myMat;
    public Material hoverMat;
    bool isSpawnPoint = false;
    public int spawnPointTeam = -1;
    [HideInInspector]
    public TextMeshPro text;
    public GameObject myTextObj;

    private void Start()
    {
        myMat = GetComponent<Renderer>().material;
        text = myTextObj.GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            GetComponent<Renderer>().material = myMat;

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
        if (!EventSystem.current.IsPointerOverGameObject())
            HoverOver();
    }

    public void HoverOver()
    {
        GetComponent<Renderer>().material = hoverMat;
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material = myMat;
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (mE.action == MapEditing.Choice.Placing && spawnPointTeam == -1)
            {
                mE.AddNewSect(mySect.coord);
            }

            else if (mE.action == MapEditing.Choice.ChoosingSpawnSpots && (spawnPointTeam == -1 || spawnPointTeam == mE.teamToAddTo))
            {
                if (mE.ToggleSpawnPoint(mySect.coord))
                {
                    if (spawnPointTeam == -1)
                        spawnPointTeam = mE.teamToAddTo;
                    else if (spawnPointTeam == mE.teamToAddTo)
                        spawnPointTeam = -1;
                }
            }
            else if (mE.action == MapEditing.Choice.Recolor)
            {
                mE.Paint(mySect);
                myMat.color = EditingUI.instance.modelColor;
            }
        }
    }
}
