using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class _MoveItemFunc
{
    // ¾î·Æ³×
    internal static Color ItemToItem_CheckColor(this iRoot_DDO_Manager _inven, SlotGUI_InvenSlot _src, SlotGUI_InvenSlot _dst)
    {
        if (_inven.Comp_ItemToItem(_src, _dst))
        {
            if (CheckItemMaxLevel(_src._itemGUI._myData))
            {
                return Color.blue;
            }
            else
                Debug.Log("??");
        }

        return Color.white;
    }

    internal static void ItemToItem_EventDDO(this iRoot_DDO_Manager _inven, SlotGUI_InvenSlot _src, SlotGUI_InvenSlot _dst)
    {
        if (_inven.Comp_ItemToItem(_src, _dst))
        {
            if (CheckItemMaxLevel(_src._itemGUI._myData))
            {
                _inven.FusionFunc(_src, _dst);
                return;
            }
            else
                Debug.Log("??");
        }

        _inven.MoveFunc(_src, _dst);
        return;
    }

    internal static bool Comp_ItemToItem(this iRoot_DDO_Manager _inven, SlotGUI_InvenSlot _src, SlotGUI_InvenSlot _dst)
    {
        if ((_src._itemGUI && _dst._itemGUI) == false)
            return false;

        ItemUnit _srcData = _src._itemGUI._myData;
        ItemUnit _dstData = _dst._itemGUI._myData;

        for (int i = 0; i < _srcData.itemData.Count; i++)
        {
            if(_srcData.itemData[i] != _dstData.itemData[i]) return false; 
        }
        return true;
    }

    static void MoveFunc(this iRoot_DDO_Manager _inven, SlotGUI_InvenSlot _src, SlotGUI_InvenSlot _dst)
    {
        GUI_ItemUnit _srcGUI = _src._itemGUI;
        GUI_ItemUnit _dstGUI = _dst._itemGUI;
        _dst.SetItemGUI(_srcGUI);
        _src.SetItemGUI(_dstGUI);
        return;
    }

    static void FusionFunc(this iRoot_DDO_Manager _inven, SlotGUI_InvenSlot _src, SlotGUI_InvenSlot _dst)
    {
        _inven.GetInvenSGT().itemUnits.Remove(_src._itemGUI._myData);
        _src.SetItemData_byData(null);

        ItemUnit targetData = _dst._itemGUI._myData; targetData.itemData[2]++;
        GUI_ItemUnit newGUI = _inven.GetInvenSGT().spriteDataSet.GetGUI_byItemData(targetData.itemData, _src.GetTransform_ItemGUI());
        _dst.SetItemData_byData(null);
        _dst.SetGUI_byItemGUI(newGUI);
        _dst.SetItemData_byData(targetData);

        return;
    }

    static bool CheckItemMaxLevel(ItemUnit target) 
    {
        return target.itemData[2] < 2;
    }
}
