using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : SlotBase
{
    public TMP_Text textPokerNum;
    public TMP_Text textAmount;
    public Image imageIngredient;
    public CountableItem ci;
    CookUi cookUi;
    public IngredientScrollView parentScrollView;
    private void Start()
    {
        cookUi = GameManager.storeScenario.cookUi;
    }
    public void SetCountableItem(CountableItem _ci)
    {
        ci = _ci;
        IngredientClass ingredient = (IngredientClass)ci.item;
        textPokerNum.text = ingredient.pokerNum.ToString();
        textPokerNum.color = ingredient.GetPokerNumColor();
        textAmount.text = ci.amount.ToString();
        imageIngredient.sprite = ingredient.sprite;
    }
    public void OnButtonClicked()
    {
        if (!cookUi.OnIngredientSlotClicked(ci.item))
            return;
        cookUi.textName.text = string.Empty;
        ci.amount--;
        if (ci.amount == 0)
        {
            gameObject.SetActive(false);
            parentScrollView.ModifySizeDelta(false);
            SetHighLightAlphaZero();
        }
        else
        {
            textAmount.text = ci.amount.ToString();
        }
    }
    public void OnPointerEnter()
    {
        HighlightOn();
    }
    public void OnPointerExit()
    {
        HighlightOff();
    }
}
