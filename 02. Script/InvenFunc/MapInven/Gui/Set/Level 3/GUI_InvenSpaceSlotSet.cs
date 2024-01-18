using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUI_InvenSpaceSlotSet : MonoBehaviour
{
    public List<int> myAddr = new();
    public List<SlotGUI_InvenSlot> MySlotList = new List<SlotGUI_InvenSlot>();
    public int index = -1;

    public void _DEBUG(List<int> _data)
    {
        myAddr = new();
        for (int i = 0; i < _data.Count; i++)
        {
            myAddr.Add(_data[i]);
        }
        myAddr.Add(index);

        MySlotList = GetComponentsInChildren<SlotGUI_InvenSlot>().ToList();
        for (int i = 0; i < MySlotList.Count; i++)
        {
            MySlotList[i]._DEBUG(i,myAddr);
        }
    }
}
