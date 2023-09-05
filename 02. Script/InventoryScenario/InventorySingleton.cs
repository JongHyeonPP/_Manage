using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySingleton : MonoBehaviour
{
    public static InventorySingleton inventorySingleton;
    public GUI_Inventory GUI_inven;
    public InvenDataSingleton data;
    public InventoryScenario currScenario;

    public bool CheckInitOn(InventoryScenario _scenario)
    {
        bool returnValue = false;
        data = data.CheckInitOn_DataSgt();

        if (inventorySingleton == null)
        {
            inventorySingleton = this;
            DontDestroyOnLoad(this);

            _UpdateData();
            returnValue = true;
        }
        else if (_scenario == this)
        {
            Debug.Log("it is strange");
            _UpdateData();
            returnValue = false;
        }
        else
        {
            inventorySingleton.gameObject.SetActive(true);
            _scenario.invenDataSGT = inventorySingleton.data;
            Destroy(gameObject);
            _UpdateData();
            returnValue = false;
        }

        currScenario = _scenario;

        return returnValue;

        void _UpdateData()
        {
            _scenario.GUI_inven = GUI_inven;
        }
    }
}
