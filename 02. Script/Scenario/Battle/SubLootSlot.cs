using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubLootSlot : MonoBehaviour
{
    public Image imageLoot;
    public TMP_Text textAmount;
    public TMP_Text textPokerNum;
    public void SetSubLoot(IngredientClass _ingredient, int _amount)
    {
        imageLoot.sprite = _ingredient.sprite;
        textAmount.text = _amount .ToString();
        textPokerNum.text = _ingredient.pokerNum.ToString();
        textPokerNum.color = _ingredient.GetPokerNumColor();
    }
}
