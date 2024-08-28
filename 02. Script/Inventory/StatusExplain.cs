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
        statusName.text = "<b><color=#FF4545>" + ((GameManager.language == Language.Ko) ? "ü��" : "Hp");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"<b><color=#FF4545><size=120%>{_maxHp:F0}</size></color></b>�� ü�� ��\n<b><color=#FF8A45><size=120%>{_hp:F0}</size></color></b>�� ���ҽ��ϴ�." :
            $"Out of <b><color=#FF4545><size=120%>{_maxHp:F0}</size></color></b> health points,\n<b><color=#FF8A45><size=120%>{_hp:F0}</size></color></b> are remaining.";
        SetSize(250f);
    }

    public void SetAbilityExplain(float _ability)
    {
        statusName.text = "<b><color=#FAF75C>" + ((GameManager.language == Language.Ko) ? "�ɷ�" : "Ability");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"��ų�� �⺻������\n<b><color=#FAF75C><size=120%>{_ability:F0}</size></color></b>��ŭ �������ϴ�." :
            $"Skills and default attack\nbecome <b><color=#FAF75C><size=120%>{_ability:F0}</size></color></b> stronger.";
        SetSize(250f);
    }

    public void SetResistExplain(float _resist)
    {
        float calcedResist = (1f - BattleScenario.CalcResist(_resist)) * 100f;
        statusName.text = "<b><color=#45FF63>" + ((GameManager.language == Language.Ko) ? "���׷�" : "Resist");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"�޴� ���ذ� <b><color=#45FF63><size=120%>{calcedResist:F1}%</size></color></b>\n��ŭ �����մϴ�." :
            $"The damage taken is\nreduced by <b><color=#45FF63><size=120%>{calcedResist:F1}%</size></color></b>";
        SetSize(250f);
    }

    public void SetSpeedExplain(float _speed)
    {
        statusName.text = "<b><color=#1429E6>" + ((GameManager.language == Language.Ko) ? "�ӵ�" : "Speed");
        statusExplain.text = (GameManager.language == Language.Ko) ?
            $"���� �ӵ��� <b><color=#1429E6><size=120%>{_speed:F1}��</size></color></b>\n�������ϴ�." :
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
        rectTransform.sizeDelta = new Vector2(_width, Mathf.Max(150f, statusExplain.preferredHeight + 100f));//width ���� �� ���� preferredHeight ��� �ż� �� ��
        rectTransform.sizeDelta = new Vector2(_width, Mathf.Max(150f, statusExplain.preferredHeight + 100f));
        imageLine.sizeDelta = new Vector2(_width*0.8f, imageLine.sizeDelta.y);
    }
}