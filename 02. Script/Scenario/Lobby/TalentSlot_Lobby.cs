using DefaultCollection;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using EnumCollection;
using System;
public class TalentSlot_Lobby : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        if (_talent.ableLevel == -1)
        {
            imageIcon.transform.localScale = Vector3.one * 0.6f;
            textLevel.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            textLevel.transform.parent.gameObject.SetActive(true);
            imageIcon.transform.localScale = Vector3.one;
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
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        talentExplainUi.rectTransform.parent = transform;
        talentExplainUi.rectTransform.localPosition = new Vector2(0f, 50f);
        talentExplainUi.gameObject.SetActive(true);
        string title = currentTalent.name[GameManager.language];
        if (currentTalent.ableLevel != -1)
        {
            string levelStr = (GameManager.language == Language.Ko) ? "·¹º§" : "Level";
            title += $" <size=120%>({levelStr} {currentTalent.effectLevel + 1})";
        }
        talentExplainUi.SetTalentExplain(title ,currentTalent.GetExplain());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        talentExplainUi.gameObject.SetActive(false);
    }


}
