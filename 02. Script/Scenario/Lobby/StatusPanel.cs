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
                talentExplainUi.SetTalentExplain(GameManager.language==Language.Ko?"체력":"Hp", GameManager.language == Language.Ko ? "전투에서의 최대 체력" : "Maximum health in battle");
                break;
            case StatusType.Ability:
                talentExplainUi.SetTalentExplain(GameManager.language == Language.Ko ? "능력" : "Ability", GameManager.language == Language.Ko ? "스킬에 영향을 주는 수치" : "Stat that affects the skill");
                break;
            case StatusType.Resist:
                talentExplainUi.SetTalentExplain(GameManager.language == Language.Ko ? "저항력" : "Resist", GameManager.language == Language.Ko ? "적에게서 받는 피해 감소량" : "Damage reduction from enemies");
                break;
            case StatusType.Speed:
                talentExplainUi.SetTalentExplain(GameManager.language == Language.Ko ? "속도" : "Speed", GameManager.language == Language.Ko ? "스킬의 시전 속도" : "Cast speed of the skill");
                break;
        }
        
    }
    public void OnExitPointer()
    {
        talentExplainUi.gameObject.SetActive(false);
        HighlightOff();
    }
}
