using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubLootSlot : SlotBase
{
    public Image imageLoot;
    public TMP_Text textAmount;
    public TMP_Text textPokerNum;
    public Item curItem;
    public void SetSubLoot(IngredientClass _ingredient, int _amount)
    {
        imageLoot.sprite = _ingredient.sprite;
        textAmount.text = _amount .ToString();
        textPokerNum.text = _ingredient.pokerNum.ToString();
        textPokerNum.color = _ingredient.GetPokerNumColor();
        curItem = _ingredient;
    }
    public void OnPointerEnterSlot()
    {
        HighlightOn();
        ItemTooltip tooltip = GameManager.battleScenario.battleTooltip;
        tooltip.transform.parent = transform;
        tooltip.rectTransform.anchorMin = new Vector2(0.5f, 0f);
        tooltip.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        tooltip.rectTransform.pivot = new Vector2(0.5f, 0f);
        tooltip.transform.localPosition = new Vector2(0f, 50f);
        tooltip.SetTooltipInfo(curItem);
        tooltip.gameObject.SetActive(true);
    }
    public void OnPointerExitSlot()
    {
        HighlightOff();
        GameManager.battleScenario.battleTooltip.gameObject.SetActive(false);
    }
}
