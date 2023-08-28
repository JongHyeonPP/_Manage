using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponedByDrop : MonoBehaviour
{
    public virtual void OnDragDrop(DragDropObj temp)
    {
        Debug.Log("tlqkf");
        //InventoryScenario.inventoryScenario.SwapEquipment(temp.transform.GetComponent<GUI_ItemUnit>(), transform.GetComponent<GUI_ItemUnit>());
        
    }
}
