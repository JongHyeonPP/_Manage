using DefaultCollection;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using EnumCollection;
public class TalentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RecruitUi recruitUi;
    public Image imageIcon;
    public TMP_Text textLevel;
    private TalentClass currentTalent;
    private TalentExplainUi talentExplainUi;
    private void Start()
    {
        recruitUi = GameManager.lobbyScenario.recruitUi;
        talentExplainUi = recruitUi.talentExplainUi;
        talentExplainUi.gameObject.SetActive(false);
    }
    public void SetCurrentTalent(TalentClass _talent)
    {
        currentTalent = _talent;
        imageIcon.sprite = _talent.sprite;
        switch (_talent.effectLevel)
        {
            default:
                textLevel.text = "I";
                break;
            case 1:
                textLevel.text = "II";
                break;
            case 2:
                textLevel.text = "III";
                break;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        talentExplainUi.rectTransform.parent = transform;
        talentExplainUi.rectTransform.localPosition = new Vector2(0f, 50f);
        talentExplainUi.gameObject.SetActive(true);
        string levelStr = (GameManager.language == Language.Ko) ? "·¹º§" : "Level";
        string title = $"{currentTalent.name[GameManager.language]} <size=120%>({levelStr} {currentTalent.effectLevel + 1})";
        talentExplainUi.SetTalentExplain(title ,currentTalent.GetExplain());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        talentExplainUi.gameObject.SetActive(false);
    }
}
