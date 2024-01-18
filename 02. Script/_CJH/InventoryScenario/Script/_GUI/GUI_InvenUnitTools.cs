using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

static internal class GUI_InvenUnitTools
{
    static internal void InitInvenGUI(this GUI_Inventory inven)
    {

    }

    static internal void RefreshInvenGUI(this GUI_Inventory inven)
    {
        return;
        List<GUI_ItemUnit> temp = new();
        for (int i = 0; i < inven.myItemList.Count; i++)
        {
            temp.Add(inven.myItemList[i]);
        }

        Transform rootParent = inven.gridList[0]._trans.parent;
        for (int i = 0; i < inven.gridList.Count; i++)
        {
            if (temp.Count <= i)
                break;
            else
            {
                inven.gridList[i].gameObject.SetActive(false);
                temp[i]._trans.SetParent(rootParent);
                temp[i]._trans.SetSiblingIndex(i);
            }
        }
        
    }
    static internal void tmp_SetItemToGUI(this GUI_Inventory inven, GUI_ItemUnit dataGUI, GUI_InvenGrid target)
    {
        target.ApplyDataToGridGUI(dataGUI);
    }

    static internal void ApplyInvenUnitGUI(this GUI_Inventory inven, GUI_ItemUnit item, InvenUnit data)
    {
        data._mySlave = item.gameObject;
        item._myData = data;

        item._img.color = inven.UnitDataTable.spriteList[data._type].myColor;
        item._img.sprite = inven.UnitDataTable.spriteList[data._type * 0].table[data._level];
        item._nameText.text = data._name;

    }

    static internal GUI_InvenGrid GetGridSpaceInInven(this GUI_Inventory inven)
    {
        for (int i = 0; i < inven.gridList.Count; i++)
        {
            if (inven.gridList[i]._myData == null)
                return inven.gridList[i];

            if (inven.gridList[i]._myData._name == "NULL")
                return inven.gridList[i];
        }

        Debug.Log("´Ù Ã¡´Ù");
        return null; 
    }
}
