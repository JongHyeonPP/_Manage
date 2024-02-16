using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class CharacterData
{
    public string docId;
    public string jobId;
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public float speed;
    public int index;
    public Dictionary<EffectType, float> PermEffects { get; private set; }//보류
    public string[] skillNames = new string[2];
    public string weaponId;
    public string weaponCur;//무기가 변경되었는지 알기 위한 배열 
    public void SetPermEffects(EffectType _effectType, float _value)
    {
        if (!PermEffects.ContainsKey(_effectType))
        {
            PermEffects.Add(_effectType, new());
        }
        PermEffects[_effectType] += _value;
    }
    public CharacterData(string _docId, string _jobId, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _index, string[] _skillNames, string _weaponName)
    {
        docId = _docId;
        jobId = _jobId;
        maxHp = _maxHp;
        hp = _hp;
        ability = _ability;
        resist = _resist;
        speed = _speed;
        index = _index;
        skillNames = _skillNames;
        weaponId = _weaponName;
    }
    public string ChangeSkill(int _index, string _skillName)//스킬 착용하고 착용하고 있던 스킬 리턴
    {
        //몇 번째에 무슨 스킬을 넣겠다고 했을 때, 원래 착용 중이던 스킬 Return
        bool wasNull = false;//원래 비어있는 스킬 슬롯이었는지 체크
        if (skillNames[_index] == string.Empty)
            wasNull = true;
        string returnValue = skillNames[_index];
        skillNames[_index] = _skillName;//핵심
        if (wasNull)
        {
            //넣으면서 두 개가 충족되었는지 체크
            int num = 0;
            foreach (string x in skillNames)
            {
                if (x != string.Empty)
                {
                    num++;
                }
            }
            if (num == 2)
            {
                //프리팹 바꿔줘야함
                GameManager.battleScenario.DestoyByDocId(docId);
                jobId = GameManager.gameManager.GetJobId(skillNames);
                GameManager.gameManager.InitFriendlyObject(docId, jobId, BattleScenario.FriendlyGrids[index]);
            }
        }
        return returnValue;
    }
    public string ChangeWeapon(string _weaponId)//무기를 장착하고 무기 id 리스트 리턴
    {
        //type에 따른 한 손, 양 손 판정
        //weaponsName만 접근
        weaponId = _weaponId;
        return weaponId;
    }
    public Tuple<string, float> GetSkillExplainAndCoolTime(int _skillIndex)
    {
        string skillName = skillNames[_skillIndex];
        if (skillName == string.Empty)
            return null;
        Skill skill = GameManager.LocalizeSkill(skillNames[_skillIndex]);
        string explain = skill.explain[GameManager.language].
            Replace("{Value_0}", skill.effects[0].value.ToString() + (skill.effects[0].isConst ? string.Empty:"Ab")).
            Replace("{Count_0}", skill.effects[0].count.ToString()).
            Replace("{Vamp_0}", skill.effects[0].vamp.ToString());
        if (skill.effects.Count >= 2)
        {
            explain = explain.
            Replace("{Value_1}", skill.effects[1].value.ToString() + (skill.effects[1].isConst ? string.Empty : "Ab")).
            Replace("{Count_1}", skill.effects[1].count.ToString()).
            Replace("{Vamp_1}", skill.effects[1].vamp.ToString());
        }
        if (skill.effects.Count >= 3)
        {
            explain = explain.
            Replace("{Value_2}", skill.effects[2].value.ToString() + (skill.effects[2].isConst ? string.Empty : "Ab")).
            Replace("{Count_2}", skill.effects[2].count.ToString()).
            Replace("{Vamp_2}", skill.effects[2].vamp.ToString());
        }
        
        bool isAllPassive = true;
        foreach (var x in skill.effects)
        {
            if (!x.isPassive)
            {
                isAllPassive = false;
                break;
            }
        }
        float cooltime = isAllPassive?-1:skill.cooltime;
        return new Tuple<string, float>(explain, cooltime);
    }
}