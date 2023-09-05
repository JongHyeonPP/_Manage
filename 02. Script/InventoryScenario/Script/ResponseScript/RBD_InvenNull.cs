using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBD_InvenNull : ResponedByDrop
{
    public int gridIndex;
    public GUI_InvenGrid myDataGUI;

    override public void OnDragDrop(DragDropObj droppedObj)
    {
        Debug.Log("item to Null");

        InventoryScenario.inventoryScenario.SwapEquipment(getDroppedGUI(droppedObj), myDataGUI);
    }
}


/*
private void Start()
{
    gridIndex = transform.parent.GetSiblingIndex();
}*/