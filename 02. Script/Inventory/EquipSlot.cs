using EnumCollection;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

public class EquipSlot : SlotBase, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public Item item { get; private set; }
    public ItemType itemType { get; set; }
    public Image imageItem;
    public int index;
    public GameObject objectCategori;
    public TMP_Text textLevel;
    public TMP_Text textName;
    public GameObject expBar;
    public Image imageFill;
    private new void Awake()
    {
        base.Awake();
        expBar.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (expBar.activeSelf)
        {
            ItemManager.itemManager.inventoryUi.SetTooltipAtInventory(transform, new Vector3(50f,30f), item);
            HighlightOn();
        }
        else
        {
            InventorySlot draggingSlot = ItemManager.itemManager.inventoryUi.draggingSlot;
            if (item != null)
            {
                ItemManager.itemManager.inventoryUi.SetTooltipAtInventory(transform, new Vector3(50f, 50f), item);
            }
            if (!draggingSlot || draggingSlot.ci.item.itemType != itemType)
                return;
            HighlightOn();
            ItemManager.itemManager.inventoryUi.targetEquipSlot = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightOff();
        ItemManager.itemManager.inventoryUi.tooltip.gameObject.SetActive(false);
        ItemManager.itemManager.inventoryUi.targetEquipSlot = null;
    }

    public void SetSlot(Item _item)
    {
        if (_item != null)
        {
            imageItem.gameObject.SetActive(true);
            item = _item;
            _item.SetSpriteToImage(imageItem);
            objectCategori.SetActive(_item.itemType == ItemType.Skill);
            if (_item.itemType == ItemType.Skill)
            {
                objectCategori.SetActive(true);
                switch (_item.itemGrade)
                {
                    case ItemGrade.Normal:
                        textLevel.text = "I";
                        break;
                    case ItemGrade.Rare:
                        textLevel.text = "II";
                        break;
                    case ItemGrade.Unique:
                        textLevel.text = "III";
                        break;
                }
            }
            else
            {
                objectCategori.SetActive(false);
            }
        }
        else
        {
            item = null;
            imageItem.gameObject.SetActive(false);
            objectCategori.SetActive(false);
        }
    }
    public void SetExp(float _exp)
    {
        imageFill.fillAmount = (float)_exp /ItemManager.itemManager.GetNeedExp(item.itemGrade);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (expBar.activeSelf)
        {
            ItemManager.itemManager.upgradeSkillUi.gameObject.SetActive(true);
            ItemManager.itemManager.upgradeSkillUi.InitUpgradeSkillUi((SkillAsItem)item, index);
        }
    }
}
