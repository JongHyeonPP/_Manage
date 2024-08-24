using EnumCollection;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoreUi : MonoBehaviour
{
    private List<InventorySlot_Store> inventorySlots;
    [SerializeField] Transform parentSlot;
    public Image imageSell;
    public Image raycastBlock;
    public List<SelectButton> selectButtons = new();
    private SelectButton currentSelectButton;
    public InventorySlot_Store draggingSlot;
    public bool sellReady;
    [SerializeField] SellUi sellUi;
    [SerializeField] BuyConfirmUi buyConfirmUi;
    public List<GoodsPanel> goodsPanels;
    private void Awake()
    {
        imageSell.gameObject.SetActive(false);
        sellUi.gameObject.SetActive(false);
        buyConfirmUi.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        SetInventorySlots();
        SelectButtonSelect(selectButtons[0]);
        GameManager.gameManager.buttonInventory.enabled = GameManager.gameManager.buttonSetting.enabled = false;
        GameManager.storeScenario.raycastBlock.SetActive(true);
        raycastBlock.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        GameManager.gameManager.buttonInventory.enabled = GameManager.gameManager.buttonSetting.enabled = true;
        sellUi.gameObject.SetActive(false);
        buyConfirmUi.gameObject.SetActive(false);
        GameManager.storeScenario.raycastBlock.SetActive(false);

    }
    private void SetInventorySlots()
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
    public void OnPointerEnterOnImageSell()
    {
        sellReady = true;

    }
    public void OnPointerExitOnImageSell()
    {
        sellReady = false;
    }
    public void SetSellUi(InventorySlot_Store _slot)
    {
        sellUi.SetInventorySlot(_slot);
        sellUi.gameObject.SetActive(true);
    }

    public void SetNewGoods()
    {
        for (int i = 0; i < 3; i++)
        {
            goodsPanels[i].SetNewGoods();
        }
    }
    public void OnGoodsSlotClick(GoodsSlot _goodsSlot)
    {
        buyConfirmUi.SetItemPrice(_goodsSlot);
        buyConfirmUi.gameObject.SetActive(true);
    }

    public InventorySlot_Store GetExistSlot(Item _item)
    {
        return inventorySlots.Where(data => data.connectedSlot.ci != null).Where(data => data.connectedSlot.ci.item.itemId == _item.itemId).FirstOrDefault();
    }
    public void ConnectEmptySlot(Item _item)
    {
        InventorySlot_Store emptySlot = inventorySlots.Where(data => data.connectedSlot.ci == null).FirstOrDefault();
        if (emptySlot != null)
        {
            InventorySlot connectedSlot = ItemManager.itemManager.inventoryUi.inventorySlots[emptySlot.connectedSlot.slotIndex];
            connectedSlot.SetSlot(new(_item));
            emptySlot.SetSlot(connectedSlot);
        }
    }

    internal void SetExistingSlot(Dictionary<string, object> goodsDoc)
    {
        foreach (KeyValuePair<string, object> kvp in goodsDoc)
        {
            GoodsPanel targetGoodsPanel = null;
            switch (kvp.Key)
            {
                case "Weapon":
                    targetGoodsPanel = goodsPanels[0];
                    break;
                case "Skill":
                    targetGoodsPanel = goodsPanels[1];
                    break;
                case "Premium":
                    targetGoodsPanel = goodsPanels[2];
                    break;
            }
            List<object> goodsDataList = kvp.Value as List<object>;
            targetGoodsPanel.SetExistingGoods(goodsDataList);
        }
    }
}
