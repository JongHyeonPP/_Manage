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
    public void InitFriendly(List<Skill> _skills,List<TalentStruct> _talents, int _gridIndex, float _maxHp, float _hp, float _ability, float _resist, int _job)
    {
        InitCharacter(_skills, GameManager.gameManager.FriendlyGrids[_gridIndex], _gridIndex, _maxHp, _hp, _ability, _resist);
        if (talents != null)
        {
            talents = _talents;
            ApplyTalent();
        }
        ability = _ability;
        job = _job;
    }

    public override void OnDead()
    {
        GameManager.gameManager.Friendlies.Remove(this);
        GameManager.battleScenario.regularEffect -= ActiveRegularEffect;
        if (GameManager.gameManager.Friendlies.Count == 0)
        {
            GameManager.battleScenario.GameOver();
        }
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