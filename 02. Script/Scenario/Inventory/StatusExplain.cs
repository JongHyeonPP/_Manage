using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;
using JetBrains.Annotations;
public class StatusExplain : MonoBehaviour
{
    public TMP_Text statusName;
    public TMP_Text statusExplain;
    public void SetHpExplain(float _maxHp, float _hp)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "ü��" : "Hp";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"<size=+5>{_maxHp:F0}</size>�� ü�� ��\n<size=+5>{_hp:F0}</size>�� ���ҽ��ϴ�." :
            $"Out of <size=+5>{_maxHp:F0}</size> health points,\n<size=+5>{_hp:F0}</size> are remaining.";
    }

    public void SetAbilityExplain(float _ability)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "�ɷ�" : "Ability";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"��ų�� �⺻������\n<size=+5>{_ability:F0}</size>��ŭ �������ϴ�." :
            $"Skills and default attack\nbecome <size=+5>{_ability:F0}</size> stronger.";
    }

    public void SetResistExplain(float _resist)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "���׷�" : "Resist";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"�޴� ���ذ� <size=+5>{_resist:F1}%</size>\n��ŭ �����մϴ�." :
            $"The damage taken is\nreduced by <size=+5>{_resist:F1}%</size>";
    }

    public void SetSpeedExplain(float _speed)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "�ӵ�" : "Speed";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"���� �ӵ��� <size=+5>{_speed:F1}%</size>\n�������ϴ�." :
            $"Casting speed\nincreases by <size=+5>{_speed:F1}%</size>";
    }
    public void SetExplain(string _name, string _explain)
    {
        statusName.text = _name;
        statusExplain.text = _explain;
    }
}