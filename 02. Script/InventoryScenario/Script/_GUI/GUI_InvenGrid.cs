using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GUI_InvenGrid : MonoBehaviour, GUI_ItemSlot_Interface
{
    public InvenUnit _myData;
    public RBD_InvenNull _rbd;
    public Transform _trans;
    public GUI_ItemUnit _myItem;

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

        _myItem = dataGUI;
        _myData = dataGUI._myData;

        dataGUI._trans.parent = _trans;
        dataGUI.SetSizeAuto();
        dataGUI._myData.Update_gridPos(_rbd.gridIndex);
        dataGUI._myData.Update_equipped(0);
    }
}
