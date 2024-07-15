using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using ItemCollection;
using System.Threading.Tasks;

[Serializable]
public class CharacterData:MonoBehaviour
{
    public string docId;
    public JobClass jobClass;
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public float speed;
    public int index;
    public Dictionary<EffectType, float> PermEffects { get; private set; }//º¸·ù
    public Skill[] skills = new Skill[2];
    public WeaponClass weapon;
    public CharacterHierarchy characterHierarchy;
    public CharacterInBattle characterAtBattle;
    internal void InitCharacterData(string _docId, string _jobId, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _index, Skill[] _skills, WeaponClass _weapon)
    {
        docId = _docId;
        jobClass = LoadManager.loadManager.jobsDict[_jobId];
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
            characterHierarchy.SetJobSprite(jobClass);
    }
    public void SetPermEffects(EffectType _effectType, float _value)
    {
        if (!PermEffects.ContainsKey(_effectType))
        {
            PermEffects.Add(_effectType, new());
        }
        PermEffects[_effectType] += _value;
    }
    public void ChangeWeapon(WeaponClass _newWeapon)
    {
        ItemManager.itemManager.inventoryUi.ch.SetWeaponSprite(_newWeapon);
        weapon = _newWeapon;
        characterHierarchy.SetWeaponSprite(_newWeapon);
        
    }
    public async Task SetEquipJobAtDbAsync()
    {
        Dictionary<string, object> setDict = new Dictionary<string, object>
        {
            { "WeaponId", weapon.itemId },
            { "JobId", jobClass.jobId}
        };
        if (skills[0] != null)
            setDict.Add("Skill_0", skills[0].itemId);
        if (skills[1]!=null)
            setDict.Add("Skill_1", skills[1].itemId);
        await DataManager.dataManager.SetDocumentData(setDict, $"Progress/{GameManager.gameManager.Uid}/Characters",docId);
    }
}