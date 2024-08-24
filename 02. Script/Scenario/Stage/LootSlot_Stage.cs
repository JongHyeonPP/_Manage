using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootSlot_Stage : MonoBehaviour
{
    public Image imageGrade;
    public Image imageIcon;
    public TMP_Text textName;
    public void SetContent(Sprite _sprite, string _text, ItemGrade _itemGrade)
    {
        imageIcon.sprite = _sprite;
        textName.text = _text;
        switch (_itemGrade)
        {
            case ItemGrade.Normal:
                imageGrade.sprite = ItemManager.itemManager.item_Normal;
                break;
            case ItemGrade.Rare:
                imageGrade.sprite = ItemManager.itemManager.item_Rare;
                break;
            case ItemGrade.Unique:
                imageGrade.sprite = ItemManager.itemManager.item_Unique;
                break;
            case ItemGrade.None:
                imageGrade.sprite = ItemManager.itemManager.item_None;
                break;
        }
    }
}