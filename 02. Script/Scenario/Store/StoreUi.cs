using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreUi : MonoBehaviour
{
    private List<InventorySlot_Store> inventorySlots;
    [SerializeField] Transform parentSlot;
    public List<SelectButton> selectButtons = new();
    private SelectButton currentSelectButton;
    public InventorySlot_Store draggingSlot;
    public Image imageSell;
    private void Awake()
    {
        imageSell.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        SetnventorySlots();
        SelectButtonSelect(selectButtons[0]);
    }
    private void SetnventorySlots()
    {
        inventorySlots = new();
        List<InventorySlot> originSlots = ItemManager.itemManager.inventoryUi.inventorySlots;
        for (int i = 0; i < ItemManager.inventorySize; i++)
        {
            InventorySlot_Store slot = parentSlot.GetChild(i).GetComponent<InventorySlot_Store>();
            inventorySlots.Add(slot);
            InventorySlot originSlot = originSlots[i];
            slot.SetSlot(originSlot);
            slot.slotIndex = i;
        }
    }


    public void SelectButtonSelect(SelectButton _selectButton)
    {
        currentSelectButton = _selectButton;
        foreach (SelectButton sb in selectButtons)
        {
            sb.ActiveHighlight(sb == _selectButton);
        }
        ItemType type = _selectButton.type;
        foreach (InventorySlot_Store slot in inventorySlots)
        {
            bool isActive;
            if (type == ItemType.All)
                isActive = true;
            else
            {
                if (slot.connectedSlot.ci == null)
                    isActive = true;
                else
                    isActive = slot.connectedSlot.ci.item.itemType == type;
            }
            slot.SetSelected(isActive);
        }
    }
}
