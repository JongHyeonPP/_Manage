using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_IngridiSlotManager : MonoBehaviour
{
    public List<GUI_IngridiSlot> slots;


    [ContextMenu("ReadCurrIngridiment")]
    void ReadCurrIngridiment()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetDefault();
        }   
    }

    [ContextMenu("sad")]
    void sad()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetDefault();
        }
        Debug.Log(checkCurr());
    }
    
    bool checkCurr()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].value < 0)
                return false;
        }
        return true;
    }
}
