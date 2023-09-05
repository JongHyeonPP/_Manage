using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapScenario;

public class RBD_Inven : ResponedByDrop
{
    public GUI_ItemUnit iu;
    public Transform myData;

    override public void OnDragDrop(DragDropObj droppedObj)
    {
        Debug.Log("item to item");

        InventoryScenario.inventoryScenario.SwapEquipment(
                getDroppedGUI(droppedObj),
            getMyGUI());


        GUI_ItemSlot_Interface getMyGUI()
        {
            int target = iu._myData._equipped;
            Debug.Log("Response. index = " + target);

            if (target == 0)
            {
                return myData.parent.GetComponent<GUI_InvenGrid>();
            }
            else if (target == 1)
            {
                return myData.parent.GetComponent<GUI_InvenEquipSlot>();
            }

            Debug.Log("Tlqkf");
            return null;
        }
    }
}
