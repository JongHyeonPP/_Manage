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
            $"<size=+5>{_maxHp:F0}</size>의 체력 중\n<size=+5>{_hp:F0}</size>이 남았습니다." :
            $"Out of <size=+5>{_maxHp:F0}</size> health points,\n<size=+5>{_hp:F0}</size> are remaining.";
    }

    public void SetAbilityExplain(float _ability)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "능력" : "Ability";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"스킬과 기본공격이\n<size=+5>{_ability:F0}</size>만큼 강해집니다." :
            $"Skills and default attack\nbecome <size=+5>{_ability:F0}</size> stronger.";
    }

    public void SetResistExplain(float _resist)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "저항력" : "Resist";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"받는 피해가 <size=+5>{_resist:F1}%</size>\n만큼 감소합니다." :
            $"The damage taken is\nreduced by <size=+5>{_resist:F1}%</size>";
    }

    public void SetSpeedExplain(float _speed)
    {
        statusName.text = (GameManager.language == Language.Ko) ? "속도" : "Speed";
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"시전 속도가 <size=+5>{_speed:F1}%</size>\n빨라집니다." :
            $"Casting speed\nincreases by <size=+5>{_speed:F1}%</size>";
    }
    public void SetExplain(string _name, string _explain)
    {
        statusName.text = _name;
        statusExplain.text = _explain;
    }
}