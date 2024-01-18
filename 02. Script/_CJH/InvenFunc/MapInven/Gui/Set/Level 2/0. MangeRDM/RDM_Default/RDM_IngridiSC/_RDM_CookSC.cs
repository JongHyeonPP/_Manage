using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class _RDM_CookSC
{
    internal static void SetIngridiment_byInvenSlot(this RDM_CookSC _inven, SlotGUI_InvenSlot _src, RBD_IngridimentSlot _dst)
    {
        GUI_ItemUnit _itemGUI = _src._itemGUI;

        _dst.myGUI.value = _itemGUI._myData.index;
        _dst._GUI_ItemUnit = _itemGUI;
        ItemUnit _itemUnit = _src._itemGUI._myData;
        Image _Values_GUI = _src._itemGUI.getImageGUI();
        _dst.myGUI.SetImg(_Values_GUI.sprite, _Values_GUI.color);
        _src._itemGUI.SetSizeAuto();
        return;
    }
}