using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellUi : MonoBehaviour
{
    private InventorySlot_Store slot;
    private int amount;

    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textReturn;
    [SerializeField] TMP_Text textConfirm;
    [SerializeField] TMP_InputField inputFieldAmount;
    
    public void SetInventorySlot(InventorySlot_Store _slot)
    {
        slot = _slot;
        amount = slot.connectedSlot.ci.amount;
        inputFieldAmount.text = amount.ToString();
    }
    public void OnPlusButtonClick()
    {
        amount++;
        ModifyValueToInputField();
    }
    public void OnMinusButtonClick()
    {
        amount--;
        ModifyValueToInputField();
    }
    public void OnValueChanged()
    {
        if (inputFieldAmount.text == string.Empty)
        {
            amount = 0;
        }
        else
        {
            if (int.TryParse(inputFieldAmount.text, out int result))
            {
                amount = result;
            }
            ModifyValueToInputField();
        }
    }
    public void OnReturnButtonClick()
    {
        gameObject.SetActive(false);
    }
    public void OnConfirmButtonClick()
    {
        if (amount == 0)
            return;
        gameObject.SetActive(false);
        slot.SetAmountResult(amount);
        float price = 0;
        Item item = slot.connectedSlot.ci.item;
        switch (item.itemType)
        {
            case EnumCollection.ItemType.Weapon:
                switch (item.itemGrade)
                {
                    case EnumCollection.ItemGrade.Normal:
                        break;
                    case EnumCollection.ItemGrade.Rare:
                        break;
                    case EnumCollection.ItemGrade.Unique:
                        break;
                    case EnumCollection.ItemGrade.None:
                        break;
                }
                break;
            case EnumCollection.ItemType.Skill:
                switch (item.itemGrade)
                {
                    case EnumCollection.ItemGrade.Normal:
                        break;
                    case EnumCollection.ItemGrade.Rare:
                        break;
                    case EnumCollection.ItemGrade.Unique:
                        break;
                }
                break;
            case EnumCollection.ItemType.Food:
                var food = (FoodClass)item;
                switch (food.pokerCombination)
                {
                    case EnumCollection.PokerCombination.NoCard:
                        break;
                    case EnumCollection.PokerCombination.HighCard:
                        break;
                    case EnumCollection.PokerCombination.OnePair:
                        break;
                    case EnumCollection.PokerCombination.TwoPair:
                        break;
                    case EnumCollection.PokerCombination.ThreeOfAKind:
                        break;
                    case EnumCollection.PokerCombination.Straight:
                        break;
                    case EnumCollection.PokerCombination.Flush:
                        break;
                    case EnumCollection.PokerCombination.FullHouse:
                        break;
                    case EnumCollection.PokerCombination.FourOfAKind:
                        break;
                    case EnumCollection.PokerCombination.StraightFlush:
                        break;
                }
                break;
            case EnumCollection.ItemType.Ingredient:
                price = StoreScenario.ingredientPrice;
                break;
        }
    }
    private void ModifyValueToInputField()
    {
        if (amount < 0)
            amount = 0;
        if (amount > slot.connectedSlot.ci.amount)
        {
            amount = slot.connectedSlot.ci.amount;
        }
        inputFieldAmount.text = amount.ToString();
    }
}
