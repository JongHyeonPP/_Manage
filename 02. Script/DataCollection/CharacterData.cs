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
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public float speed;
    public int index;
    public Dictionary<EffectType, float> PermEffects { get; private set; }//보류
    public string[] skillNames = new string[2];
    public void SetPermEffects(EffectType _effectType, float _value)
    {
        if (!PermEffects.ContainsKey(_effectType))
        {
            PermEffects.Add(_effectType, new());
        }
        PermEffects[_effectType] += _value;
    }
    public CharacterData(string _docId, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _index, string[] _skillNames)
    {
        docId = _docId;
        maxHp = _maxHp;
        hp = _hp;
        ability = _ability;
        resist = _resist;
        speed = _speed;
        index = _index;
        skillNames = _skillNames;
    }
    public string ChangeSkill(int _index, string _skillName)
    {
        //몇 번째에 무슨 스킬을 넣겠다고 했을 때, 원래 착용 중이던 스킬 Return
        bool wasNull = false;//원래 비어있는 스킬 슬롯이었는지 체크
        if (skillNames[_index] == string.Empty)
            wasNull = true;
        string returnValue = skillNames[_index];
        skillNames[_index] = _skillName;
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
                GameManager.gameManager.InitFriendlyObject(docId, skillNames, BattleScenario.FriendlyGrids[index]);
                
            }
        }
        return returnValue;
    }
}