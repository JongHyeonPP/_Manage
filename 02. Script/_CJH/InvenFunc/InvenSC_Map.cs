using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenSC_Map : MonoBehaviour
{
    public SGT_GUI_ItemData invenData_SGT;
    public CJH_CharacterData charData_SGT;
    public GUI_InvenSetManager invenGUI_Manager;
    public List<ItemUnit> ItemList_Data;

    void Awake()
    {
        invenData_SGT.InitSGT(ref invenData_SGT);
        charData_SGT.InitSGT(ref charData_SGT);

        ItemList_Data = invenData_SGT.itemUnits;
    }

    private void Start()
    {
        setCharData_DEBUG();
        setGUI_bySGT();

        void setCharData_DEBUG()
        {
            /*
            Debug.Log("getCharData_DEBUG");

            if (_cm == null)
                _cm = FindObjectOfType<CharacterManager>();

            if (_cm == null)
            {
                Debug.Log("wognsdk cm sjgdjfk");
                return;
            }*/

            //_cm.SetData();
        }
    }

    public void AddItem_Debug()
    {
        SlotGUI_InvenSlot targetSlot = invenGUI_Manager.GetSlotGUI_byMin();

        if (!targetSlot)
        {
            Debug.Log("Inven is Full"); return;
        }

        ItemUnit itemData = new ItemUnit();
        itemData.InitData_Random_Slot(targetSlot);
        ItemList_Data.Add(itemData);

        addGUI_byData(itemData);
    }


    private void addGUI_byData(ItemUnit data)
    {
        SlotGUI_InvenSlot targetSlot = invenGUI_Manager.GetSlotGUI_byAddr(data.invenAddr);
        GUI_ItemUnit ins_ItemGUI = invenData_SGT.spriteDataSet.GetGUI_byItemData(data.itemData, invenGUI_Manager._InsTrans);

        targetSlot.SetGUI_byItemGUI(ins_ItemGUI);
        targetSlot.SetItemData_byData(data);
    }


    public void setGUI_bySGT()
    {  
        for (int i = 0; i < ItemList_Data.Count; i++)
        {
            GUI_ItemUnit sad = invenData_SGT.spriteDataSet.GetGUI_byItemData(ItemList_Data[i].itemData, invenGUI_Manager._InsTrans);
            SlotGUI_InvenSlot temp = invenGUI_Manager.GetSlotGUI_byAddr(ItemList_Data[i].invenAddr);
            temp.SetGUI_byItemGUI(sad);
            temp.SetItemData_byData(ItemList_Data[i]);
        }
    }
}