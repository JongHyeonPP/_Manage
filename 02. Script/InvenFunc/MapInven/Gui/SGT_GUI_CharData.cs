using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SGT_GUI_CharData : MonoBehaviour
{
    private static SGT_GUI_CharData dataSGT;
    public List<CharUnit> dataList = new();

    public void InitSGT(ref SGT_GUI_CharData _localData)
    {
        if (dataSGT == null)
        {
            DontDestroyOnLoad(_localData);
            dataSGT = _localData;
        }
        else
        {
            if (dataSGT == _localData)
            {
                return;
            }

            GameObject temp = _localData.gameObject;
            _localData = dataSGT;
            Destroy(temp);
        }
    }

    static public SGT_GUI_CharData getSGT()
    {
        return dataSGT;
    }
}

[Serializable]
public class CharUnit
{
    static int _index = 0;
    public int index = -1;
    public string name;
    public Vector3Int stat;

    public void InitData_Random(List<UnitDataTable> _dt, SlotGUI_InvenSlot _slot)
    {
        initData_Index();

        void initData_Index()
        {
            index = _index++;
            name = "Rand_" + index;
        }
    }
}