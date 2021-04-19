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
    public GameObject commands, movebutton, cancelButton;
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
            unitSelect.attacking = false;
            unitSelect.hoverObj.SetActive(false);
        }

        if (uS.selectedUnit)
        {
            if (uS.selectedUnit.moved)
            {
                movebutton.SetActive(false);
                cancelButton.SetActive(true);
            }
            else
            {
                cancelButton.SetActive(false);
            }
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
                abilityButton.onClick.AddListener(() => ProjectileTarget(currentAbility));
            if (currentAbility.targetMethod == Ability.TargetMode.SingleAoETarget)
                abilityButton.onClick.AddListener(() => SingleAoETarget(currentAbility));
            if (currentAbility.targetMethod == Ability.TargetMode.MultiLineTarget)
                abilityButton.onClick.AddListener(() => MultiLineTarget(currentAbility));

        }
    }

    public void ProjectileTarget(Ability ability)
    {
        HideCommands();
        unitSelect.targetting = true;
        combat.attacker = uS.selectedUnit;
        combat.castedAbility = ability;
        pf.StartFindProjectileRange(ability, unitSelect.selectedUnit);
        
    }

    public void MultiLineTarget(Ability ability)
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
        movebutton.SetActive(true);
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
        unitSelect.hoverObj.SetActive(true);
        unitSelect.hoverObj.transform.position = unitSelect.selectedUnit.transform.position;

        unitSelect.attackHoverObj.SetActive(false);

        pf.StartFindRange(unitSelect.selectedUnit.transform.position, (int)unitSelect.selectedUnit.movePoints, unitSelect.selectedUnit.attackRange, unitSelect.selectedUnit.team);
        HideCommands();
    }

    public void CloseMovement()
    {
        Destroy(GameObject.Find("DisplaySquares"));
        Destroy(GameObject.Find("AttackDisplaySquares"));
        Destroy(GameObject.Find("AbilityDisplaySquares"));
        Destroy(GameObject.Find("BlastZoneSquares"));
        RemoveAbilityButtons();
        chooseAttack = false;
        chooseMove = false;
        unitSelect.hoverObj.SetActive(false);
        unitSelect.attackHoverObj.SetActive(false);
        unitSelect.moving = false;
        unitSelect.attacking = false;
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
        unitSelect.attacking = true;
        unitSelect.hoverObj.SetActive(false);
        unitSelect.attackHoverObj.SetActive(true);
        unitSelect.hoverObj.transform.position = unitSelect.selectedUnit.transform.position;
        unitSelect.attackHoverObj.transform.position = unitSelect.selectedUnit.transform.position;

        grid.ResetGrid();
        grid.ResetTileGrid();

        pf.StartFindAttackRange(unitSelect.selectedUnit.transform.position, unitSelect.selectedUnit.attackRange, unitSelect.selectedUnit.team);
        HideCommands();
    }
}