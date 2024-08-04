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
            $"<b><color=#0096FF><size=120%>{_maxHp:F0}</size></color></b>�� ü�� ��\n<b><color=#0096FF><size=120%>{_hp:F0}</size></color></b>�� ���ҽ��ϴ�." :
            $"Out of <b><color=#0096FF><size=120%>{_maxHp:F0}</size></color></b> health points,\n<b><color=#0096FF><size=120%>{_hp:F0}</size></color></b> are remaining.";
    }

    public void SetAbilityExplain(float _ability)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "�ɷ�" : "Ability";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"��ų�� �⺻������\n<b><color=#0096FF><size=120%>{_ability:F0}</size></color></b>��ŭ �������ϴ�." :
            $"Skills and default attack\nbecome <b><color=#0096FF><size=120%>{_ability:F0}</size></color></b> stronger.";
    }

    public void SetResistExplain(float _resist)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "���׷�" : "Resist";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"�޴� ���ذ� <b><color=#0096FF><size=120%>{_resist:F1}%</size></color></b>\n��ŭ �����մϴ�." :
            $"The damage taken is\nreduced by <b><color=#0096FF><size=120%>{_resist:F1}%</size></color></b>";
    }

    public void SetSpeedExplain(float _speed)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "�ӵ�" : "Speed";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"���� �ӵ��� <b><color=#0096FF><size=120%>{_speed:F1}%</size></color></b>\n�������ϴ�." :
            $"Casting speed\nincreases by <b><color=#0096FF><size=120%>{_speed:F1}%</size></color></b>";
    }
    public void SetExplain(string _name, string _explain)
    {
        statusName.text = _name;
        statusExplain.text = _explain;
    }
}