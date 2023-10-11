using EnumCollection;
using StructCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FriendlyScript : CharacterBase
{
    public static readonly Color TARGET_COLOR = new(0f, 0f, 1f, 0.5f);
    public int job;
    public bool isAct = false;
    public new string name;
    public List<TalentStruct> talents;
    public List<Image> skillImages = new();
    RectTransform rectTransfrom;
    private void Awake()
    {
        rectTransfrom = GetComponent<RectTransform>();
    }
    public Dictionary<T_Type, float> talentEffects { get; private set; } = new()
    {
        { T_Type.AttAscend, 0f },
        { T_Type.DefAscend, 0f },
        { T_Type.AttDescend, 0f },
        { T_Type.DefDescend, 0f },
        { T_Type.Critical, 0f },
        { T_Type.HealAscend, 0f },
        { T_Type.BuffAscend, 0f},
        { T_Type.DebuffAscend, 0f}
    };
    public void CreateFriendly(float _maxHp, float _ability, float _resist)
    {
        maxHp = _maxHp;
        ability = _ability;
        resist = _resist;
    }
    public void InitFriendly( List<Skill> _skills,List<TalentStruct> _talents, ObjectGrid _grid, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _job)
    {
        InitCharacter(_skills, _maxHp, _hp, _ability, _resist, _speed);
        if (talents != null)
        {
            talents = _talents;
            ApplyTalent();
        }
        ability = _ability;
        job = _job;
        IsEnemy = false;
        grid = _grid;
        grid.owner = this;
    }

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
            foreach (T_Effect x1 in x0.effects)
            {
                switch (x1.type)
                {
                    case T_Type.FameAscend:
                        GameManager.gameManager.fameAscend += x1.value;
                        break;
                    case T_Type.GoldAscend:
                        GameManager.gameManager.goldAscend += x1.value;
                        break;
                    default:
                        talentEffects[x1.type]+=x1.value;
                        break;
                }
            }
        }
    }
}