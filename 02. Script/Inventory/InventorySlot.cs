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
    public int slotIndex;
    public void SetSlot(CountableItem _ci)
    {
        imageItem.gameObject.SetActive(true);

        ci = _ci;
        Material gradeMat = null;
        switch (_ci.item.itemGrade)
        {
            default:
                break;
            case ItemGrade.Normal:
                gradeMat = ItemManager.itemManager.itemMat_Normal;
                break;
            case ItemGrade.Rare:
                gradeMat = ItemManager.itemManager.itemMat_Rare;
                break;
            case ItemGrade.Unique:
                gradeMat = ItemManager.itemManager.itemMat_Unique;
                break;
        }
        imageGrade.material = gradeMat;
        Sprite itemSprite;
        switch (ci.item.itemType)
        {
            default:
                itemSprite = ci.item.sprite;
                imageItem.transform.localScale = ci.item.scale;
                break;
            case ItemType.Skill:
                switch (ci.item.itemId.Split("_")[0])
                {
                    default:
                        itemSprite = ItemManager.itemManager.book_P;
                        break;
                    case "Util":
                        itemSprite = ItemManager.itemManager.book_U;
                        break;
                    case "Sustain":
                        itemSprite = ItemManager.itemManager.book_S;
                        break;
                }
                imageItem.transform.localScale = Vector2.one * 0.6f;
                break;

        }

        bool isIngredient = ci.item.itemType == ItemType.Ingredient;
        imageItem.gameObject.SetActive(!isIngredient);
        imageGrade.gameObject.SetActive(!isIngredient);

        imageItem.sprite = itemSprite;
        if (ci.amount == 1)
            textNum.gameObject.SetActive(false);
        else
        {
            textNum.gameObject.SetActive(true);
            textNum.text = ci.amount.ToString();
        }
    }

    public void ClearSlot()
    {
        ci = null;
        imageGrade.gameObject.SetActive(false);
        imageItem.gameObject.SetActive(false);
        textNum.gameObject.SetActive(false);
    }
}
