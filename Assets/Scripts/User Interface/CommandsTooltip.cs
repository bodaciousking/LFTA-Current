using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void DisplayAbilityButtons()
    {
        foreach (GameObject button in uiM.buttons)
        {
            button.SetActive(false);
        }
        for (int i = 0; i < unitSelect.selectedUnit.abilityList.Count; i++)
        {
            Button abilityButton = Instantiate(abilityButtonPrefab, commands.transform);
            TextMeshProUGUI tmp = abilityButton.GetComponentInChildren<TextMeshProUGUI>();
            Ability currentAbility = unitSelect.selectedUnit.abilityList[i];
            tmp.text = currentAbility.name;

            abilityButton.onClick.RemoveAllListeners();

            if (currentAbility.targetMethod == Ability.TargetMode.SingleLineTarget)
            {
                UnityEngine.Events.UnityAction action1 = () => { SingleTargetLine(currentAbility); };
                abilityButton.onClick.AddListener(action1); 
            }
            if (currentAbility.targetMethod == Ability.TargetMode.SingleAoETarget)
            {
                UnityEngine.Events.UnityAction action1 = () => { SingleAoETarget(currentAbility); };
                abilityButton.onClick.AddListener(action1);
            }
            if (currentAbility.targetMethod == Ability.TargetMode.MultiLineTarget)
            {
                UnityEngine.Events.UnityAction action1 = () => { MultiTargetLine(currentAbility); };
                abilityButton.onClick.AddListener(action1);
            }

        }
    }

    public void SingleTargetLine(Ability ability)
    {
        HideCommands();
        unitSelect.targetting = true;
        combat.attacker = uS.selectedUnit;
        combat.castedAbility = ability;
        pf.StartFindLineRange(ability, unitSelect.selectedUnit);
    }

    public void MultiTargetLine(Ability ability)
    {
        HideCommands();
        unitSelect.targetting = true;
        combat.attacker = uS.selectedUnit;
        combat.castedAbility = ability;
        pf.StartFindMultiTargetLineRange(ability, unitSelect.selectedUnit);
    }

    public void SingleAoETarget(Ability ability)
    {
        HideCommands();
        unitSelect.targetting = true;
        combat.attacker = uS.selectedUnit;
        combat.castedAbility = ability;
        pf.StartFindAoERange(ability, unitSelect.selectedUnit);
    }

    public void RemoveAbilityButtons()
    {
        foreach (GameObject button in GameObject.FindGameObjectsWithTag("AbilityButton"))
        {
            Destroy(button);
        }
    }

    public void OpenAbilityTargetting()
    {
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

    public void OpenMovement()
    {
        grid.ResetGrid();
        grid.ResetTileGrid();
        chooseMove = true;
        chooseAttack = false;

        unitSelect.targetting = true;
        unitSelect.moving = true;

        pf.StartFindMoveRange(unitSelect.selectedUnit.transform.position, unitSelect.selectedUnit);
        HideCommands();

    }

    public void CloseMovement()
    {
        Destroy(GameObject.Find("DisplaySquares"));
        Destroy(GameObject.Find("AttackDisplaySquares"));
        Destroy(GameObject.Find("AbilityDisplaySquares"));
        Destroy(GameObject.Find("BlastZoneSquares"));
        //RemoveAbilityButtons();
        chooseAttack = false;
        chooseMove = false;
        unitSelect.moving = false;
    }

    public void Defend()
    {
        unitSelect.selectedUnit.Defend();
        HideCommands();
    }

    public void OpenAttack()
    {
        RemoveAbilityButtons();
        chooseAttack = !chooseAttack;
        chooseMove = false;
        unitSelect.targetting = true;
        unitSelect.moving = false;
        grid.ResetGrid();
        grid.ResetTileGrid();

        pf.StartFindAttackRange(unitSelect.selectedUnit.transform.position, unitSelect.selectedUnit);
    }
}