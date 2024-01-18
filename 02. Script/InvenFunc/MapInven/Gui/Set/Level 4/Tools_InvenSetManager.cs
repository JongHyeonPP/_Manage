using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class Tools_InvenSetManager
{
    static internal GUI_ItemUnit GetGUI_byItemData(this List<UnitDataTable> _dataSet, List<int> _itemData,Transform _trans)
    {
        int itemType_0 = _itemData[0]; int itemType_1 = _itemData[1]; int itemType_2 = _itemData[2];

        GUI_ItemUnit obj = Object.Instantiate(_dataSet[itemType_0].prefab_InvenUnitGUI, _trans);
        obj._GUI.img.sprite = _dataSet[itemType_0].spriteList[itemType_1].table[itemType_2];
        obj._GUI.img.color = _dataSet[itemType_0].spriteList[itemType_1].myColor;
        return obj;
    }

    static internal SlotGUI_InvenSlot GetSlotGUI_byAddr(this GUI_InvenSetManager _inven, List<int> _addrData)
    {
        int equipIndex = _addrData[0];
        int invenIndex = _addrData[1];

        return _inven.myInvenSet[equipIndex].MySlotList[invenIndex];
    }

    static internal SlotGUI_InvenSlot GetSlotGUI_byMin(this GUI_InvenSetManager _inven)
    {
        for (int i = 0; i < _inven.myInvenSet[0].MySlotList.Count; i++)
        {
            if (_inven.myInvenSet[0].MySlotList[i]._itemGUI == null)
                return _inven.myInvenSet[0].MySlotList[i];
        }

        return null;
    }

    static internal void SetItemData_byData(this SlotGUI_InvenSlot _inven, ItemUnit _item)
    {
        _inven._itemGUI._myData = _item;
        if (_item == null)
        {
            Object.Destroy(_inven._itemGUI.gameObject);
            _inven._itemGUI = null;
        }    

        return;
    }
    static internal void SetItemData_byData(this SlotGUI_ShopGoods _inven, ItemUnit _item)
    {
        _inven._itemGUI._myData = _item;

        return;
    }
}