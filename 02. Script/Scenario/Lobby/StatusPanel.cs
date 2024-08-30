using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;
public class StatusPanel : SlotBase
{
    private RecruitUi recruitUi;
    private TalentExplainUi talentExplainUi;
    public StatusType statusType;
    private void Start()
    {
        recruitUi = GameManager.lobbyScenario.recruitUi;
        talentExplainUi = recruitUi.talentExplainUi;
    }
    public void OnEnterPointer()
    {
        HighlightOn();
        talentExplainUi.rectTransform.parent = transform;
        talentExplainUi.rectTransform.localPosition = new Vector2(0f, 30f);
        talentExplainUi.rectTransform.sizeDelta = new Vector2(400f, talentExplainUi.rectTransform.sizeDelta.y);
        talentExplainUi.gameObject.SetActive(true);
        switch (statusType)
        {
            case StatusType.Hp:
                talentExplainUi.SetTalentExplain(GameManager.language==Language.Ko?"ü��":"Hp", GameManager.language == Language.Ko ? "���������� �ִ� ü��" : "Maximum health in battle");
                break;
            case StatusType.Ability:
                talentExplainUi.SetTalentExplain(GameManager.language == Language.Ko ? "�ɷ�" : "Ability", GameManager.language == Language.Ko ? "��ų�� ������ �ִ� ��ġ" : "Stat that affects the skill");
                break;
            case StatusType.Resist:
                talentExplainUi.SetTalentExplain(GameManager.language == Language.Ko ? "���׷�" : "Resist", GameManager.language == Language.Ko ? "�����Լ� �޴� ���� ���ҷ�" : "Damage reduction from enemies");
                break;
            case StatusType.Speed:
                talentExplainUi.SetTalentExplain(GameManager.language == Language.Ko ? "�ӵ�" : "Speed", GameManager.language == Language.Ko ? "��ų�� ���� �ӵ�" : "Cast speed of the skill");
                break;
        }
        
    }
    public void OnExitPointer()
    {
        talentExplainUi.gameObject.SetActive(false);
        HighlightOff();
    }
}
