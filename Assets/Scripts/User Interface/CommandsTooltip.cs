using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class CommandsTooltip : MonoBehaviour
{
    private UnitSelection unitSelect;
    public static CommandsTooltip instance;
    MapInfo mI;
    Pathfinding pf;
    Grid grid;
    UnitSelection uS;
    public GameObject commands;
    public Button abilityButtonPrefab;
    public Ability usedAbility;
    UIMover uiM;
    Combat combat;
    TargetDisplay tD;
    TargettingActual tA;
    public bool chooseMove = false, chooseAttack = false;

    void Start()
    {
        unitSelect = GameObject.Find("GameManager").GetComponent<UnitSelection>();
        pf = Pathfinding.instance;
        mI = MapInfo.instance;
        grid = Grid.instance;
        uS = UnitSelection.instance;
        uiM = commands.GetComponent<UIMover>();
        combat = Combat.instance;
        tD = TargetDisplay.instance;
        tA = TargettingActual.instance;

        chooseMove = false;
        chooseAttack = false;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many CommandsTooltips!");
            return;
        }
        instance = this;
    }

    void Update()
    {
        if (!chooseMove && !chooseAttack)
        {
            unitSelect.targetting = false;
            unitSelect.moving = false;
        }

        if (uS.selectedUnit)
        {
            if(uS.selectedUnit.canAct == false)
            {
                HideCommands();
            }
        }
    }

    public void StartTargetting(Ability ability)
    {
        CloseMovement();
        HideCommands();
        unitSelect.targetting = true;
        combat.attacker = uS.selectedUnit;
        combat.castedAbility = ability;
        List<Tile> tilesInRange = tD.GetTilesInRange(combat.attacker.currentTile, ability.range);
        tD.Display(tilesInRange);
        tA.ValidateTargets(tilesInRange);
    }

    public void RemoveAbilityButtons()
    {
        foreach (GameObject button in GameObject.FindGameObjectsWithTag("AbilityButton"))
        {
            Destroy(button);
        }
    }

    public void DisplayCommands()
    {
        commands.transform.position = Camera.main.WorldToScreenPoint(uS.selectedUnit.transform.Find("UI Orient").transform.position);
        commands.SetActive(true);
        RemoveAbilityButtons();
        foreach (GameObject button in uiM.buttons)
        {
            button.SetActive(true);
        }
        for (int i = 0; i < unitSelect.selectedUnit.abilityList.Count; i++)
        {
            Button abilityButton = Instantiate(abilityButtonPrefab, commands.transform);
            TextMeshProUGUI tmp = abilityButton.GetComponentInChildren<TextMeshProUGUI>();
            Ability currentAbility = unitSelect.selectedUnit.abilityList[i];
            tmp.text = currentAbility.name;

            abilityButton.onClick.RemoveAllListeners();

            UnityEngine.Events.UnityAction action1 = () => { StartTargetting(currentAbility); };
          
            UnityEngine.Events.UnityAction action2 = () => { tD.DestroyMoveSquares(); };

            abilityButton.onClick.AddListener(action2);
            abilityButton.onClick.AddListener(action1);
        }
    }

    public void HideCommands()
    {
        commands.SetActive(false);
    }

    public void CancelMovement()
    {
        CloseMovement();
        uS.selectedUnit.CancelMove();
        grid.ResetGrid();
        grid.ResetTileGrid();
    }

    public void OpenAttack()
    {
        chooseAttack = !chooseAttack;
        chooseMove = false;
        unitSelect.targetting = true;
        unitSelect.moving = false;

        List<Tile> tilesInRange = tA.GetTilesInRange(unitSelect.selectedUnit.currentTile, unitSelect.selectedUnit.attackRange);

        tA.ValidateTargets(tilesInRange);

        tD.Display(tilesInRange);
    }

    public void OpenMovement()
    {
        grid.ResetGrid();
        grid.ResetTileGrid();
        chooseMove = true;
        chooseAttack = false;

        unitSelect.targetting = true;
        unitSelect.moving = true;

        List<Tile> tilesInRange = tA.GetTilesInRange(unitSelect.selectedUnit.currentTile, unitSelect.selectedUnit.moveRange);
       
        tA.ValidateTargets(tilesInRange);

        tD.Display(tilesInRange);

        HideCommands();

    }
    public void CloseMovement()
    {
        tD.DestroyMoveSquares();
        grid.ResetTileGrid();
        chooseAttack = false;
        chooseMove = false;
        unitSelect.moving = false;
    }

    public void Defend()
    {
        unitSelect.selectedUnit.Defend();
        HideCommands();
    }
}