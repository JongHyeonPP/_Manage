using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VolumeComponent;

public class GUI_Inventory : MonoBehaviour
{
    [SerializeField] internal UnitDataTable UnitDataTable;
    [SerializeField] public InvenDataSet _InvenData;

    public List<GUI_ItemUnit> myItemList;
    public List<GUI_InvenGrid> gridList;
    //public Transform dataParents;

    public void AddNew()
    {
        GUI_InvenGrid target = this.GetGridSpaceInInven();
        if (target == null)
            return;

        InvenUnit temp = _InvenData.data.Debug_AddRandom();
        GUI_ItemUnit my = Instantiate(UnitDataTable.prefab_InvenUnitGUI, UnitDataTable.parent).GetComponent<GUI_ItemUnit>();
        this.tmp_SetItemToGUI(my, target);
        this.ApplyInvenUnitGUI(my, temp);

        //myItemList.Add(my);

        //this.RefreshInvenGUI();
    }
}

[Serializable]
internal class UnitDataTable{
    public GUI_ItemUnit prefab_InvenUnitGUI;
    public Transform parent;
    [SerializeField] public List<MySpriteTable> spriteList;

    [Serializable] 
    internal class MySpriteTable
    {
        public Color myColor;
        public List<Sprite> table;
    }
}

[Serializable]
public class InvenDataSet
{
    [SerializeField]
    public List<InvenUnit> data;
}