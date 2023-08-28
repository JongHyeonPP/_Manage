using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RBD_Equip : ResponedByDrop
{
    public Transform myData;

    override public void OnDragDrop(DragDropObj temp)
    {
        InventoryScenario.inventoryScenario.SwapEquipment(temp.transform.GetComponent<GUI_ItemUnit>(), myData.GetComponent<GUI_ItemUnit>());

    }
}
