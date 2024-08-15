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

    public void SynchronizeCharacterData(CharacterData _data)
    {
        skillInBattles.Clear();
        maxHp = maxHpInBattle = _data.maxHp;
        ability = _data.ability;
        speed = _data.speed;
        resist = _data.resist;
        Hp = _data.hp;
        job = _data.jobClass;
        //Talent
        List<List<SkillEffect>> effectsList = new();
        foreach (TalentClass talent in _data.talents)
        {
            List<SkillEffect> effects = new();
            foreach (TalentEffect x in talent.effects)
            {
                PassiveEffect skillEffect = new PassiveEffect(1, true, x.value[talent.effectLevel], x.type, EffectRange.Self, ValueBase.Const, false, 0f, -99f, 1f);
                effects.Add(skillEffect);
            }
            effectsList.Add(effects);
            skillInBattles.Add(new Skill(string.Empty, default, -99, effectsList, false, false, null, null, null).GetInBattle(0));
        }
        //Job
        if (job.jobSkill != null)
            skillInBattles.Add(job.jobSkill.GetInBattle(0));
        //Skill
        for (int i = 0; i < _data.skillAsItems.Length; i++)
        {
            SkillAsItem asItem = _data.skillAsItems[i];
            if (asItem != null)
            {
                Skill skill = LoadManager.loadManager.skillsDict[asItem.itemId];
                skillInBattles.Add(skill.GetInBattle((int)(asItem.itemGrade)));
            }
        }

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
        name = _data.jobClass.name;
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
            GameManager.battleScenario.GameOver();//게임 오버
        }

    }

    public override void SetAnimParam()
    {

    }
}