using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Inventory : MonoBehaviour
{
    [SerializeField] internal UnitDataTable UnitDataTable;
    
    [SerializeField]
    public InvenDataSet _InvenData;

    public List<GUI_ItemUnit> myInvenSpace;

    public GUI_ItemUnit prefab_InvenUnitGUI;

    public List<Button> filterList;
    public int _fillterIndex = 0;
    public List<Button> sortList;
    public int sortIndex = 0;
    public Transform dataParents, parentTrans;
    public GameObject dataPrefab;

    public void SetFillter()
    {
        SwapFilterQueue();
        this.RefreshInvenGUI();

        void SwapFilterQueue()
        {
            int temp = sortIndex++;
            sortIndex %= sortList.Count;
            sortList[sortIndex].gameObject.SetActive(true);
            sortList[temp].gameObject.SetActive(false);
        }
    }

    public void EventSetFilter(int input)
    {
        this.SetFilter(input);
        this.RefreshInvenGUI();
    }

    public void addRandom()
    {
        GUI_ItemUnit temp = Instantiate(prefab_InvenUnitGUI, dataParents);
        _InvenData.data.Debug_AddRandom(temp);
        temp.UpdateObjectGUI(UnitDataTable);

        this.RefreshInvenGUI();
    }


    public void sortByType()
    {
        _InvenData.data.SortByType(); this.RefreshInvenGUI();
    }

    public void sortByLevel()
    {
        _InvenData.data.SortByLevel(); this.RefreshInvenGUI();
    }

    public void sortByIndex()
    {
        _InvenData.data.SortByIndex(); this.RefreshInvenGUI();
    }
}

[Serializable]
internal class UnitDataTable{
    [SerializeField]
    public List<MySpriteTable> spriteList;

    public Sprite getSprite(int _case, int _index)
    {
        return spriteList[0].table[_index];
    }
    public Color getColor(int _case, int _index)
    {
        return spriteList[_case].myColor;
    }

    [Serializable]
    internal class MySpriteTable
    {
        public Color myColor;
        public List<Sprite> table;
    }
}

