using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamDisplay : MonoBehaviour
{
    TeamManager tM;
    public GameObject teamPiano, unitDisplay;
    public static TeamDisplay instance;
    public GameObject panelHolder;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Too many Team Displays!");
            return;
        }
        instance = this;
    }
    void Start()
    {
    }

    public void OrganizeTeams()
    {
        tM = TeamManager.instance;
        for (int i = 0; i < tM.teams.Count; i++)
        {
            RectTransform panelTransform = panelHolder.GetComponent<RectTransform>();
            float width = panelTransform.rect.width;
            float height = panelTransform.rect.height;
            GridLayoutGroup panelLayout = panelHolder.GetComponent<GridLayoutGroup>();
            Vector2 pianoSize = new Vector2(width / tM.teams.Count, height / tM.teams.Count);
            panelLayout.cellSize = pianoSize;

            GameObject newPiano = Instantiate(teamPiano, panelHolder.transform);
            RectTransform pianoTransform = newPiano.GetComponent<RectTransform>();
            float pWidth = pianoTransform.rect.width;
            float pHeight = pianoTransform.rect.height;
            GridLayoutGroup pianoLayout = newPiano.GetComponent<GridLayoutGroup>();
            float baseHeight = 40f;
            Vector2 keySize = new Vector2(pWidth / tM.teams[i].units.Count * 2, baseHeight);
            if (tM.teams[i].units.Count > 8)
            {
                float mult = tM.teams[i].units.Count / 8;
                pianoLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                pianoLayout.constraintCount = Mathf.CeilToInt(mult);
                keySize = new Vector2(pWidth / 8, baseHeight / Mathf.CeilToInt(mult));
            }
            float xClamp = Mathf.Clamp(keySize.x, 20, 30);
            float yClamp = Mathf.Clamp(keySize.y, 20, 30);
            keySize = new Vector2(xClamp, yClamp);
            if(keySize.x > keySize.y)
            {
                Vector2 oldSize = keySize;
                keySize = new Vector2(oldSize.y, oldSize.y);
            }
            if(keySize.x < keySize.y)
            {
                Vector2 oldSize = keySize;
                keySize = new Vector2(oldSize.x, oldSize.x);
            }
            pianoLayout.cellSize = keySize;

            for (int j = 0; j < tM.teams[i].units.Count; j++)
            {
                GameObject newUnitDisplay = Instantiate(unitDisplay, newPiano.transform);
                Image backgroundImage = newUnitDisplay.GetComponent<Image>();
                Image panelImage = newUnitDisplay.GetComponentsInChildren<Image>()[1];
                UnitPanelReferencer uPR = newUnitDisplay.GetComponentInChildren<UnitPanelReferencer>();
                uPR.myUnit = tM.teams[i].units[j].GetComponent<Unit>();

                panelImage.sprite = tM.teams[i].units[j].GetComponent<Unit>().profile;
                backgroundImage.color = tM.teams[i].teamColor;
            }
        }
    }
}
