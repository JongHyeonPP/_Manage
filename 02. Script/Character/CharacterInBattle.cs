using EnumCollection;
using BattleCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LobbyCollection;
using DefaultCollection;
using ItemCollection;

public class CharacterInBattle : BaseInBattle
{
    public static readonly Color TARGET_COLOR = new(0f, 0f, 1f, 0.5f);
    public bool isAct = false;
    public new string name;
    public List<TalentClass> talents;

    public void SynchronizeCharacterData(CharacterData _data)
    {
        skillInBattles.Clear();
        maxHp = maxHpInBattle = _data.maxHp;
        ability = _data.ability;
        speed = _data.speed;
        resist = _data.resist;
        Hp = _data.hp;
        for (int i = 0; i < _data.skillAsIItems.Length; i++)
        {
            SkillAsItem asItem = _data.skillAsIItems[i];
            if (asItem != null)
            {
                Skill skill = LoadManager.loadManager.skillsDict[asItem.itemId];
                skillInBattles.Add(skill.GetInBattle((int)(asItem.itemGrade)));
            }
        }
        job = _data.jobClass;
        if (job.jobSkill != null)
            skillInBattles.Add(job.jobSkill.GetInBattle(0));

        weapon = _data.weapon;

        grid = BattleScenario.CharacterGrids[_data.gridIndex];
        MoveToTargetGrid(grid, true);
        grid.owner = this;
        if (Hp == 0)
        {
            Hp = 1f;
            gameObject.SetActive(true);
            isDead = false;
        }
    }
    public void InitCharacter(CharacterData _data, GridObject _grid)
    {
        _data.characterAtBattle = this;
        IsEnemy = false;
        InitBase(_grid);
        

        transform.localScale = Vector3.one;
        transform.GetChild(0).localScale = Vector3.one * 100;
        transform.localPosition = Vector3.zero;
        skillTargetTransform = Instantiate(new GameObject("SkillTarget"), transform.GetChild(0)).transform;
        skillTargetTransform.localPosition = new Vector3(-0.4f, 0.6f, 0);
        skillTargetTransform.localScale = new Vector3(0.6f, 0.6f, 0);
        rootTargetTransform.localScale = new Vector3(0.6f, 0.6f, 0);
        rootTargetTransform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        skillTargetTransform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
    }
    [ContextMenu("DeadTest")]
    public override void OnDead()
    {
        GameManager.battleScenario.StartCoroutine(OnDead_Base());
        bool gameOverFlag = false;
        foreach (var x in BattleScenario.characters)
            if (x&&!x.isDead)
                gameOverFlag = true;
        if (!gameOverFlag)
        {
            GameManager.gameManager.GameOver();//���� ����
        }

    }

    public override void SetAnimParam()
    {

    }
}