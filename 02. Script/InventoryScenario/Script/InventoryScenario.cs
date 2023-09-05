using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryScenario : MonoBehaviour
{
    public static InventoryScenario inventoryScenario;
    public GUI_Inventory GUI_inven;
    public InvenDataSingleton invenDataSGT;
    private void Awake()
    {
        inventoryScenario = this;
        invenDataSGT = invenDataSGT.CheckInitOn_DataSgt();
        GUI_inven._InvenData = invenDataSGT.data;
        invenDataSGT.Refreash();
        GUI_inven.RefreshInvenGUI();
    }
}
