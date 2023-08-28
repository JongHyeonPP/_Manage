using UnityEngine;

static internal class GUI_InvenUnitTools
{
    static internal void InitInvenGUI(this GUI_Inventory inven)
    {

    }

    static internal void RefreshInvenGUI(this GUI_Inventory inven)
    {
        InvenDataSet _InvenData = inven._InvenData;

        foreach (InvenUnit item in _InvenData.data)
        {
            if (item._mySlave == null)
            {
                GUI_ItemUnit temp = UnityEngine.Object.Instantiate(inven.prefab_InvenUnitGUI, inven.parentTrans);
                _InvenData.data.Debug_AddByData(temp, item);
                temp.UpdateObjectGUI(inven.UnitDataTable);
            }
        }


        foreach (InvenUnit item in _InvenData.data)
        {
            item._mySlave.transform.SetParent(inven.parentTrans);
        }

        if (inven._fillterIndex == 3)
        {
            foreach (InvenUnit item in _InvenData.data)
            {
                item._mySlave.transform.SetParent(inven.dataParents);
                item._mySlave.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (InvenUnit item in _InvenData.data)
            {
                if (item._type != inven._fillterIndex)
                {
                    item._mySlave.transform.SetParent(inven.parentTrans);
                    item._mySlave.SetActive(false);
                }
                else
                {
                    item._mySlave.transform.SetParent(inven.dataParents);
                    item._mySlave.SetActive(true);
                }
            }
        }
    }

    static internal void SetFilter(this GUI_Inventory inven,int input)
    {
   
        int temp = inven._fillterIndex;
        inven.filterList[input].interactable = false;
        inven._fillterIndex = input;
        inven.filterList[temp].interactable = true;
    }

    static internal void SwapFilter(this GUI_Inventory inven, int input)
    {

        int temp = inven._fillterIndex;
        inven.filterList[input].interactable = false;
        inven._fillterIndex = input;
        inven.filterList[temp].interactable = true;
    }
}
