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
        statusName.text = (GameManager.language == Language.Ko) ? "체력" : "Hp";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"<b><color=#0096FF><size=120%>{_maxHp:F0}</size></color></b>의 체력 중\n<b><color=#0096FF><size=120%>{_hp:F0}</size></color></b>이 남았습니다." :
            $"Out of <b><color=#0096FF><size=120%>{_maxHp:F0}</size></color></b> health points,\n<b><color=#0096FF><size=120%>{_hp:F0}</size></color></b> are remaining.";
    }

    public void SetAbilityExplain(float _ability)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "능력" : "Ability";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"스킬과 기본공격이\n<b><color=#0096FF><size=120%>{_ability:F0}</size></color></b>만큼 강해집니다." :
            $"Skills and default attack\nbecome <b><color=#0096FF><size=120%>{_ability:F0}</size></color></b> stronger.";
    }

    public void SetResistExplain(float _resist)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "저항력" : "Resist";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"받는 피해가 <b><color=#0096FF><size=120%>{_resist:F1}%</size></color></b>\n만큼 감소합니다." :
            $"The damage taken is\nreduced by <b><color=#0096FF><size=120%>{_resist:F1}%</size></color></b>";
    }

    public void SetSpeedExplain(float _speed)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "속도" : "Speed";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"시전 속도가 <b><color=#0096FF><size=120%>{_speed:F1}%</size></color></b>\n빨라집니다." :
            $"Casting speed\nincreases by <b><color=#0096FF><size=120%>{_speed:F1}%</size></color></b>";
    }
    public void SetExplain(string _name, string _explain)
    {
        statusName.text = _name;
        statusExplain.text = _explain;
    }
}