using DefaultCollection;
using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TalentSlot_Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image talentIcon;
    [SerializeField]TalentClass currentTalent;
    StatusExplain statusExplain;
    private void Start()
    {
        statusExplain = ItemManager.itemManager.inventoryUi.statusExplain;
    }
    internal void SetTalentSlot(TalentClass _talent)
    {
        currentTalent = _talent;
        talentIcon.sprite = _talent.sprite;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        statusExplain.transform.parent = transform.parent;
        statusExplain.gameObject.SetActive(true);
        statusExplain.transform.localPosition = transform.localPosition + Vector3.right * 20f;

        string name = currentTalent.name[GameManager.language];
        string levelText = currentTalent.effectLevel + 1 + ((GameManager.language == Language.Ko) ? "·¹º§" : "Lv");
        statusExplain.SetExplain($"{name} ({levelText})", currentTalent.GetExplain());

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        statusExplain.gameObject.SetActive(false);
    }


}
