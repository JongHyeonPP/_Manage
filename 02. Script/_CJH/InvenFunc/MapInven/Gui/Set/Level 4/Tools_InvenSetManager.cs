using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class Tools_InvenSetManager
{
    static internal GUI_ItemUnit GetGUI_byItemData(this MyInvenSpriteDB _dataSet, List<int> _itemData,Transform _trans)
    {
        GUI_ItemUnit obj = Object.Instantiate(_dataSet.invenPrefab, _trans);
        obj.SetImageGUI_Sprite(_dataSet.GetSprite_byItemData(_itemData));

        string temp = "Lv ";
        if (true)
        {
            if (_itemData[0] == 0)
            {
                temp += _itemData[1];
            }
            else {
                temp += _itemData[_itemData.Count - 1];
            }
        }

        obj.SetNameText(temp);
        obj.SetSizeAuto(_trans);
        return obj;
    }

    static internal SlotGUI_InvenSlot GetSlotGUI_byAddr(this GUI_InvenSetManager _inven, List<int> _addrData)
    {
        int equipIndex = _addrData[0];
        int invenIndex = _addrData[1]; 
        Debug.Log(equipIndex + " / " + invenIndex);
        if (_inven.myInvenSet.Count <= equipIndex)
            return null;
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
            _inven.SetGUI_byItemGUI(null);
        }    

        return;
    }

    static internal void SetItemData_byData(this SlotGUI_ShopGoods _inven, ItemUnit _item)
    {
        _inven._itemGUI._myData = _item;

        return;
    }

    static internal void SetItemData_byData(this SlotGUI_CookResult _inven, ItemUnit _item)
    {
        _inven._itemGUI._myData = _item;

        return;
    }
}