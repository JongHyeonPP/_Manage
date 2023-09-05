using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_InvenEquipSlot : MonoBehaviour, GUI_ItemSlot_Interface
{
    public InvenUnit _myData;
    public Transform _trans;
    public GUI_ItemUnit _myItem;
    public int _index;
    public int _equipped = 1;
    public GUI_ItemUnit get_GUI_ItemUnit()
    {
        return _myItem;
    }

    public void ApplyDataToGridGUI(GUI_ItemUnit dataGUI)
    {
        if (dataGUI == null)
        {
            _myItem = null;
            _myData = InvenUnit.getNullData();
            Debug.Log("im null");
            return;
        }

        Debug.Log("TODO : equip slot change");
        _myItem = dataGUI;
        _myData = dataGUI._myData;

        dataGUI._trans.parent = _trans;
        dataGUI.SetSizeAuto();
        dataGUI._myData.Update_gridPos(_index);
        dataGUI._myData.Update_equipped(_equipped);
    }
}
