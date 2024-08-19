using EnumCollection;
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
    public int sellPrice;
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textReturn;
    [SerializeField] TMP_Text textConfirm;
    [SerializeField] TMP_Text textTotalPrice;
    [SerializeField] TMP_InputField inputFieldAmount;

    
    public void SetInventorySlot(InventorySlot_Store _slot)
    {
        slot = _slot;
        amount = slot.connectedSlot.ci.amount;
        float price = 0;
        Item item = slot.connectedSlot.ci.item;
        price = StoreScenario.GetGoodsPrice(item);
        sellPrice = Mathf.RoundToInt(price * 0.7f);
        ModifyValueToInputField();
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
    public async void OnConfirmButtonClick()
    {
        if (amount == 0)
            return;
        gameObject.SetActive(false);
        slot.SetAmountResult(amount);

        GameManager.gameManager.ChangeGold(sellPrice * amount);
        await ItemManager.itemManager.SetInventoryAtDb();
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
        textTotalPrice.text = (amount * sellPrice).ToString("F0");
    }
}
