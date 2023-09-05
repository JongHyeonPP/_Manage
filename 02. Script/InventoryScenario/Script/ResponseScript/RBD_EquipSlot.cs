using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RBD_EquipSlot : ResponedByDrop
{
    public GUI_InvenEquipSlot _EquipGUI;
    public InvenUnit _myData;
    public Transform _trans;
    public GUI_ItemUnit myItem;

    override public void OnDragDrop(DragDropObj droppedObj)
    {
        Debug.Log("item to Null");

        InventoryScenario.inventoryScenario.SwapEquipment(getDroppedGUI(droppedObj), _EquipGUI);
    }
}
