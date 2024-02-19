using System;
using System.Collections.Generic;
using UnityEngine;

public class GUI_IngridiSlotManager : MonoBehaviour
{
    [SerializeField] internal GUI_IngridiSlotValues _values;

    [ContextMenu("CookFunc_onIngridiment")]
    public void CookFunc_onIngridiment()
    {
        List<RBD_IngridimentSlot> _slots = _values.RBD_Slots;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i]._GUI_ItemUnit == null)
                continue;
            GameObject target = _slots[i]._GUI_ItemUnit.gameObject;
            Destroy(target);
        }

        Event_Reset();
    }


    [ContextMenu("Event_Reset")]
    public void Event_Reset()
    {
        List<RBD_IngridimentSlot> _slots = _values.RBD_Slots;

        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetDefault();
        }
    }
}

[Serializable]
internal struct GUI_IngridiSlotValues
{
    [SerializeField] internal List<RBD_IngridimentSlot> RBD_Slots;

    internal bool checkCurr()
    {
        for (int i = 0; i < RBD_Slots.Count; i++)
        {
            if (RBD_Slots[i].myGUI_Slot.value < 0)
                return false;
        }

        return true;
    }

    internal int IsDisAvabibleValue(int data)
    {
        for (int i = 0; i < RBD_Slots.Count; i++)
        {
            if (RBD_Slots[i].myGUI_Slot.value == data)
                return i;
        }

        return -1;
    }


    internal int[] GetCookSlotsItemIndex()
    {
        int[] temp = new int[RBD_Slots.Count];

        for (int i = 0; i < temp.Length; i++)
        {
            if (RBD_Slots[i].myGUI_Inven == null)
            {
                temp[i] = -1;
            }
            else
            {
                temp[i] = RBD_Slots[i].myGUI_Inven._index;
            }

            Debug.Log(i + " / " + temp[i]);
        }

        return temp;
    }
}