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

    public void GetEventMSG()
    {
        SwitchSceneFunc("TestInventort");
    }
    public void SwitchSceneFunc(string str)
    {
        SceneManager.LoadScene(str);    
    }

    public void ResponFunc(GameObject response, GameObject drop)
    {
        Debug.Log("Scenario Responsed : " + response.transform.name + " => " + drop.transform.name);
    }

}
