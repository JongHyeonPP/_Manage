using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;
using JetBrains.Annotations;
using UnityEngine.UI;
public class StatusExplain : MonoBehaviour
{
    private RectTransform rectTransform;
    public TMP_Text statusName;
    public TMP_Text statusExplain;
    public RectTransform imageLine;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetHpExplain(float _maxHp, float _hp)
    {
        statusName.text = "<b><color=#FF4545>" + ((GameManager.language == Language.Ko) ? "체력" : "Hp");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"<b><color=#FF4545><size=120%>{_maxHp:F0}</size></color></b>의 체력 중\n<b><color=#FF8A45><size=120%>{_hp:F0}</size></color></b>이 남았습니다." :
            $"Out of <b><color=#FF4545><size=120%>{_maxHp:F0}</size></color></b> health points,\n<b><color=#FF8A45><size=120%>{_hp:F0}</size></color></b> are remaining.";
        SetSize(250f);
    }

    public void SetAbilityExplain(float _ability)
    {
        statusName.text = "<b><color=#FAF75C>" + ((GameManager.language == Language.Ko) ? "능력" : "Ability");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"스킬과 기본공격이\n<b><color=#FAF75C><size=120%>{_ability:F0}</size></color></b>만큼 강해집니다." :
            $"Skills and default attack\nbecome <b><color=#FAF75C><size=120%>{_ability:F0}</size></color></b> stronger.";
        SetSize(250f);
    }

    public void SetResistExplain(float _resist)
    {
        float calcedResist = (1f - BattleScenario.CalcResist(_resist)) * 100f;
        statusName.text = "<b><color=#45FF63>" + ((GameManager.language == Language.Ko) ? "저항력" : "Resist");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"받는 피해가 <b><color=#45FF63><size=120%>{calcedResist:F1}%</size></color></b>\n만큼 감소합니다." :
            $"The damage taken is\nreduced by <b><color=#45FF63><size=120%>{calcedResist:F1}%</size></color></b>";
        SetSize(250f);
    }

    public void SetSpeedExplain(float _speed)
    {
        statusName.text = "<b><color=#1429E6>" + ((GameManager.language == Language.Ko) ? "속도" : "Speed");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"시전 속도가 <b><color=#1429E6><size=120%>{_speed:F1}배</size></color></b>\n빨라집니다." :
            $"The casting speed becomes <b><color=#1429E6><size=120%>{_speed:F1} times</size></color></b> faster.";
        SetSize(250f);
    }
    public void SetExplain(string _name, string _explain)
    {
        statusName.text = _name;
        statusExplain.text = _explain;
        SetSize(330f);
    }
    private void SetSize(float _width)
    {
        rectTransform.sizeDelta = new Vector2(_width, Mathf.Max(150f, statusExplain.preferredHeight + 100f));//width 설정 된 이후 preferredHeight 써야 돼서 두 번
        rectTransform.sizeDelta = new Vector2(_width, Mathf.Max(150f, statusExplain.preferredHeight + 100f));
        imageLine.sizeDelta = new Vector2(_width*0.8f, imageLine.sizeDelta.y);
    }
}