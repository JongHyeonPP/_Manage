using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class CombatProgress : MonoBehaviour
{
    public GameCharactor[,] allyBoard = new GameCharactor[3,3];
    public List<SkillCode> temp;
    //public Dictionary<string, List<SkillEffectDelegate>> Skill_Dictionary;
    public void OnCharacterDeath(GameCharactor character)
    {
        Debug.Log(character.name + "is dead");
    }

    public void test1(string str)
    {

        Skill_Set.Init_EffectSet();
        Skill_Set.Add_Skill(str);
    }

    public void test2(string str)
    {
        Skill_Set.Cast_Skill(str);
    }
}
[Serializable]
public class SkillCode
{
    public delegate void SkillEffectDelegate(GameCharactor caster, GameCharactor target);

    public string m_Name;
    public List<SkillEffectDelegate> m_Skills;
    public string skill_Code;
}

public static class Skill_Set
{
    public delegate void SkillEffectDelegate(GameCharactor caster, GameCharactor target);
    static public Dictionary<string, List<SkillEffectDelegate>> Skill_Dictionary = new Dictionary<string, List<SkillEffectDelegate>>();
    static public Dictionary<string, SkillEffectDelegate> Effect_Dictionary = new Dictionary<string, SkillEffectDelegate>();

    static public void Init_EffectSet()
    {
        Effect_Dictionary.Add("atk", Skill_Attack);
    }


    static public void Add_Skill(string codeSet, string skillName = "tempSkill")
    {
        string[] codeAry = codeSet.Split('/');

        List<SkillEffectDelegate> tempList = new List<SkillEffectDelegate>();
        foreach (string code in codeAry)
        {
            Debug.Log(code);
            tempList.Add(Effect_Dictionary[code]);
        }

        Skill_Dictionary.Add((string)codeSet, tempList);
    }

    static public void Cast_Skill(string s_name)
    {
        foreach (SkillEffectDelegate effect in Skill_Dictionary[s_name])
        {
            effect(null, null);
        }
    }

    static SkillEffectDelegate attack = Skill_Attack;
    static public void Skill_Attack(GameCharactor caster, GameCharactor target)
    {
        target.TakeDamage(caster.power);
    }
    /*
    static public void Skill_Attack(GameCharactor caster, GameCharactor target)
    {
        target.TakeDamage(caster.power);
    }*/
}