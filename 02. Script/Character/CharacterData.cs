using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using ItemCollection;

[Serializable]
public class CharacterData:MonoBehaviour
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
    public Skill[] skills = new Skill[2];
    public WeaponClass weapon;
    public CharacterHierarchy characterHierarchy;
    public CharacterInBattle characterAtBattle;
    internal void InitCharacterData(string _docId, string _jobId, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _index, Skill[] _skills, WeaponClass _weapon)
    {
        docId = _docId;
        jobId = _jobId;
        maxHp = _maxHp;
        hp = _hp;
        ability = _ability;
        resist = _resist;
        speed = _speed;
        index = _index;
        skills = _skills;
        characterHierarchy = transform.GetChild(0).GetComponent<CharacterHierarchy>();
        weapon = _weapon;
        if (_jobId != "000")
            characterHierarchy.SetJob(_jobId);
    }
    public void SetPermEffects(EffectType _effectType, float _value)
    {
        if (!PermEffects.ContainsKey(_effectType))
        {
            PermEffects.Add(_effectType, new());
        }
        PermEffects[_effectType] += _value;
    }
    public WeaponClass ChangeWeapon(WeaponClass _newWeapon)//무기를 장착하고 해제된 무기 리턴
    {
        WeaponClass curWeapon = weapon;
        weapon = _newWeapon;
        return curWeapon;
    }

    public void SetWeaponSprite() => characterHierarchy.SetWeaponSprite(weapon);
    [ContextMenu("JobChangeTest")]
    public void JobChangeTest()
    {
        characterHierarchy.SetJob("200");
    }
}