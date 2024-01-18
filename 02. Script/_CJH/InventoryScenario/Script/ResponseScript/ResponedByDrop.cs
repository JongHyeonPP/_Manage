using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponedByDrop : MonoBehaviour
{
    public virtual void OnDragDrop(DragDropObj droppedObj)
    {
        Debug.Log("default");
    }

    internal GUI_ItemSlot_Interface getDroppedGUI(DragDropObj droppedObj)
    {
        if (droppedObj._MySrc._myData._equipped == 0)
        {
            return droppedObj._parent.GetComponent<GUI_InvenGrid>();
        }
        else if (droppedObj._MySrc._myData._equipped == 1)
        {
            return droppedObj._parent.GetComponent<GUI_InvenEquipSlot>();
        }

        Debug.Log("Tlqkf");
        return null;
    }

}
    