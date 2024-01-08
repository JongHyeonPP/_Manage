using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public string docId;
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public float speed;
    public int index;
    public JobClass jobClass;
    public Dictionary<EffectType, float> PermEffects { get; private set; }//º¸·ù
    public List<Skill> skills;
    public void SetPermEffects(EffectType _effectType, float _value)
    {
        if (!PermEffects.ContainsKey(_effectType))
        {
            PermEffects.Add(_effectType, new());
        }
        PermEffects[_effectType] += _value;
    }
    public CharacterData(string _docId, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _index, string _jobId, List<Skill> _skills)
    {
        docId = _docId;
        maxHp = _maxHp;
        hp = _hp;
        ability = _ability;
        resist = _resist;
        speed = _speed;
        index = _index;
        jobClass = LoadManager.loadManager.jobsDict[_jobId];
        skills = _skills;
    }
}