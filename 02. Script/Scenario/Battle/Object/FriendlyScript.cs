using EnumCollection;
using StructCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FriendlyScript : CharacterBase
{
    public JobClass job;
    public static readonly Color TARGET_COLOR = new(0f, 0f, 1f, 0.5f);
    public bool isAct = false;
    public new string name;
    public List<TalentStruct> talents;
    public void CreateFriendly(float _maxHp, float _ability, float _resist)
    {
        maxHp = _maxHp;
        ability = _ability;
        resist = _resist;
    }
    public void InitFriendly(JobClass _job, List<Skill> _skills,List<TalentStruct> _talents, ObjectGrid _grid, float _maxHp, float _hp, float _ability, float _resist, float _speed)
    {
        job = _job;
        switch (job.jobType)
        {
            case JobType.Tanker:
            case JobType.Warrior:
            case JobType.Crusader:
            case JobType.Thief:
                break;
            case JobType.Ranger:
                break;
            case JobType.Witch:
                break;
        }
        if (talents != null)
        {
            talents = _talents;
            ApplyTalent();
        }
        ability = _ability;
        IsEnemy = false;
        grid = _grid;
        grid.owner = this;
        InitCharacter(_skills, _maxHp, _hp, _ability, _resist, _speed);
    }
    [ContextMenu("MyMethod")]
    public override void OnDead()
    {
        OnDead_Base();
        bool gameOverFlag = false;
        foreach (var x in GameManager.Friendlies)
            if (!x.isDead)
                gameOverFlag = true;
        if (!gameOverFlag)
            GameManager.gameManager.GameOver();

    }



    private void ApplyTalent()
    {
        foreach (TalentStruct x0 in talents)
        {
            foreach (SkillEffect x1 in x0.effects)
            {
                switch (x1.type)
                {

                }
            }
        }
    }

    public override void SetAnimParam()
    {
        animator.SetFloat("NormalState", 0.5f);
        animator.SetFloat("SkillState", 0.5f);
    }
}