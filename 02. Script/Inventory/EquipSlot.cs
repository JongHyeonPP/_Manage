using EnumCollection;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipSlot : SlotBase, IPointerEnterHandler, IPointerExitHandler
{
    public Item item { get; private set; }
    public ItemType itemType { get; set; }
    public Image imageItem;
    public int index;
    public GameObject objectCategori;
    public TMP_Text textCategori;
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventorySlot draggingSlot = ItemManager.itemManager.draggingSlot;
        if (item != null)
        {
            ItemManager.itemManager.inventoryUi.SetTooltipAtInventory(transform.parent, transform.localPosition + new Vector3(0f, 60f, 0f), item);
        }
        if (!draggingSlot || draggingSlot.ci.item.itemType != itemType)
            return;
        ItemManager.itemManager.targetEquipSlot = null;
        HightlightOn();

        ItemManager.itemManager.targetEquipSlot = this;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HightlightOff();
        ItemManager.itemManager.inventoryUi.tooltip.gameObject.SetActive(false);
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
                SkillCategori categori = ((Skill)_item).categori;
                ItemManager.itemManager.SetCategoriCharAtText(categori, textCategori);

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
}
