using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemCollection;
using EnumCollection;
using System;

public class InventorySlot : MonoBehaviour
{
    public CountableItem ci { get; private set; }

    public Image imageGrade;
    public Image imageItem;
    public TMP_Text textNum;

    public void SetSlot(CountableItem _ci)
    {
        ci = _ci;
        Material gradeMat = null;
        switch (_ci.item.itemGrade)
        {
            default:
                break;
            case ItemGrade.Normal:
                gradeMat = ItemManager.itemManager.nameMat_Normal;
                break;
            case ItemGrade.Rare:
                gradeMat = ItemManager.itemManager.nameMat_Rare;
                break;
            case ItemGrade.Unique:
                gradeMat = ItemManager.itemManager.nameMat_Unique;
                break;
        }
        imageGrade.material = gradeMat;
        imageItem.sprite = _ci.item.sprite;
        if (_ci.amount == 1)
            textNum.gameObject.SetActive(false);
        else
        {
            textNum.gameObject.SetActive(true);
            textNum.text = _ci.amount.ToString();
        }
    }
}
