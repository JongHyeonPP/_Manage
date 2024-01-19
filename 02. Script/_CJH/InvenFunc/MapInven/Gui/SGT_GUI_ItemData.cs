using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SGT_GUI_ItemData : MonoBehaviour
{
    private static SGT_GUI_ItemData dataSGT;
    public int currGold;
    public List<ItemUnit> itemUnits = new();
    [SerializeField] internal List<UnitDataTable_toGUI> spriteDataSet = new();

    public void InitSGT(ref SGT_GUI_ItemData _localData)
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

    public void AddItemUnit_byPurchase(ItemUnit data)
    {
        itemUnits.Add(data);
    }

    static public SGT_GUI_ItemData GetSGT()
    {
        return dataSGT;
    }

    static internal List<string> GetCharInvenSGT(int _CharIndex)
    {
        List<ItemUnit> _itemUnits = dataSGT.itemUnits;
        List<string> charEquipped= new();
        for (int i = 0; i < _itemUnits.Count; i++)
        {
            if (_itemUnits[i].invenAddr[0] == _CharIndex)
            {
                string ItemCode = ConvString_toString(_itemUnits[i].itemData);
                charEquipped.Add(ItemCode);
            }
        }

        return charEquipped;
        

        string ConvString_toString(List<int> target)
        {
            string rtnStr = "";

            switch (target[1])
            {
                case 0:
                    rtnStr += "Power_";
                    break;
                case 1:
                    rtnStr += "Sustain_";
                    break;
                case 2:
                    rtnStr += "Util_";
                    break;
                default:
                    break;
            }
            rtnStr += target[2];
            return rtnStr;
        }
    }
}

[Serializable]
public class ItemUnit
{
    static int _index = 0;
    public int index = -1;
    public string itemName;

    // case, case2 ,level
    public List<int> itemData = new();
    public List<int> invenAddr = new();
    public int GoldValue;

    internal void InitData_Random_Slot(List<UnitDataTable_toGUI> _dt, SlotGUI_InvenSlot _slot)
    {
        initData_Index();
        initData_ItemData();
        GoldValue = UnityEngine.Random.Range(50, 150);
        invenAddr = _slot.myAddr;

        void initData_Index()
        {
            index = _index++;
            itemName = "Rand_" + index;
        }

        void initData_ItemData()
        {
            int itemCase_0 = UnityEngine.Random.Range(0, _dt.Count);
            itemData.Add(itemCase_0);
            int itemType_1 = UnityEngine.Random.Range(0, _dt[itemCase_0].spriteList.Count);
            itemData.Add(itemType_1);
            int itemLevel_2 = UnityEngine.Random.Range(0, _dt[itemCase_0].spriteList[itemType_1].table.Count);
            itemData.Add(itemLevel_2);
        }
    }

    internal void InitData_Random(List<UnitDataTable_toGUI> _dt)
    {
        initData_Index();
        initData_ItemData();
        GoldValue = UnityEngine.Random.Range(50, 150);

        void initData_Index()
        {
            index = _index++;
            itemName = "RandGoods_" + index;
        }

        void initData_ItemData()
        {
            int itemCase_0 = UnityEngine.Random.Range(0, _dt.Count);
            itemData.Add(itemCase_0);
            int itemType_1 = UnityEngine.Random.Range(0, _dt[itemCase_0].spriteList.Count);
            itemData.Add(itemType_1);
            int itemLevel_2 = UnityEngine.Random.Range(0, _dt[itemCase_0].spriteList[itemType_1].table.Count);
            itemData.Add(itemLevel_2);
        }
    }

    public void SetIndex_byPurchase()
    {
        initData_Index();
        itemName = "Purchase - " + index;

        void initData_Index()
        {
            index = _index++;
            itemName = "Rand_" + index;
        }
    }
}

[Serializable]
internal class UnitDataTable_toGUI
{
    public GUI_ItemUnit prefab_InvenUnitGUI;
        
    [SerializeField] public List<MySpriteTable> spriteList;

    [Serializable]
    public class MySpriteTable
    {
        public Color myColor;
        public List<Sprite> table;
    }
}
