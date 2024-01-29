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
    public Dictionary<EffectType, float> PermEffects { get; private set; }//����
    public string[] skillNames = new string[2];
    public string[] weaponIds = new string[2];
    public string[] weaponCurs = new string[2];//���Ⱑ ����Ǿ����� �˱� ���� �迭 
    public void SetPermEffects(EffectType _effectType, float _value)
    {
        if (!PermEffects.ContainsKey(_effectType))
        {
            PermEffects.Add(_effectType, new());
        }
        PermEffects[_effectType] += _value;
    }
    public CharacterData(string _docId, string _jobId, float _maxHp, float _hp, float _ability, float _resist, float _speed, int _index, string[] _skillNames, string[] _weaponNames)
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
        weaponIds = _weaponNames;
    }
    public string ChangeSkill(int _index, string _skillName)//��ų �����ϰ� �����ϰ� �ִ� ��ų ����
    {
        //�� ��°�� ���� ��ų�� �ְڴٰ� ���� ��, ���� ���� ���̴� ��ų Return
        bool wasNull = false;//���� ����ִ� ��ų �����̾����� üũ
        if (skillNames[_index] == string.Empty)
            wasNull = true;
        string returnValue = skillNames[_index];
        skillNames[_index] = _skillName;//�ٽ�
        if (wasNull)
        {
            //�����鼭 �� ���� �����Ǿ����� üũ
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
                //������ �ٲ������
                GameManager.battleScenario.DestoyByDocId(docId);
                jobId = GameManager.gameManager.GetJobId(skillNames);
                GameManager.gameManager.InitFriendlyObject(docId, jobId, BattleScenario.FriendlyGrids[index]);
            }
        }
        return returnValue;
    }
    public List<string> ChangeWeapon(int _index, string _weaponId)//���⸦ �����ϰ� ���� id ����Ʈ ����
    {
        //type�� ���� �� ��, �� �� ����
        //weaponsName�� ����
        List<string> returnValue = new();
        WeaponClass temp = LoadManager.loadManager.weaponDict[_weaponId];
        if (temp.type == WeaponType.Bow || temp.type == WeaponType.Magic)
        {
            returnValue.Add(weaponIds[0]);
            returnValue.Add(weaponIds[1]);
            weaponIds[0] = _weaponId;
            weaponIds[1] = string.Empty;
        }
        else
        {
            returnValue.Add(weaponIds[_index]);
            weaponIds[_index] = _weaponId;
        }
        return returnValue;
    }
}