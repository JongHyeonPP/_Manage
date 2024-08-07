using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using ItemCollection;
using System.Threading.Tasks;
using DefaultCollection;

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
    public int gridIndex;
    public Dictionary<EffectType, float> PermEffects { get; private set; }//º¸·ù
    public SkillAsItem[] skillAsIItems;
    public int[] exp;
    public WeaponClass weapon;
    public CharacterHierarchy characterHierarchy;
    public CharacterInBattle characterAtBattle { get; set; }
    public List<TalentClass> talents;

    internal void InitCharacterData(string _docId, string _jobId, float _maxHp, float _hp, float _ability,
        float _resist, float _speed, int _gridIndex, SkillAsItem[] _skillasItems, int[] _exp, WeaponClass _weapon,
        List<TalentClass> _talents)
    {
        docId = _docId;
        jobClass = LoadManager.loadManager.jobsDict[_jobId];
        maxHp = _maxHp;
        hp = _hp;
        ability = _ability;
        resist = _resist;
        speed = _speed;
        gridIndex = _gridIndex;
        skillAsIItems = _skillasItems;
        exp = _exp;
        characterHierarchy = transform.GetChild(0).GetComponent<CharacterHierarchy>();
        weapon = _weapon;
        if (_jobId != "000")
            characterHierarchy.SetJobSprite(jobClass);
        talents = _talents;
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
    public async Task SetCharacterAtDbAsync()
    {
        Dictionary<string, object> setDict = new Dictionary<string, object>
        {
            { "WeaponId", weapon.itemId },
            { "JobId", jobClass.jobId},
            {"Exp", exp }
        };
        if (skillAsIItems[0] != null)
        {
            string id = skillAsIItems[0].itemId + ":::" + skillAsIItems[0].itemGrade;
            setDict.Add("Skill_0", id);
        }
        if (skillAsIItems[1] != null)
        {
            string id = skillAsIItems[1].itemId + ":::" + skillAsIItems[1].itemGrade;
            setDict.Add("Skill_1", id);
        }
        await DataManager.dataManager.SetDocumentData(setDict, $"Progress/{GameManager.gameManager.Uid}/Characters",docId);
    }
}