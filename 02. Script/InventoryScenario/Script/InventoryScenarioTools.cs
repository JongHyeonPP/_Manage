using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

internal static class InventoryScenarioTools
{
    internal static void MoveEquipment(this InventoryScenario sc, GUI_ItemSlot_Interface src, GUI_ItemSlot_Interface dst)
    {
        GUI_ItemUnit srcDataGUI = src.get_GUI_ItemUnit();

        dst.ApplyDataToGridGUI(srcDataGUI);
        src.ApplyDataToGridGUI(null);
    }

    internal static void SwapEquipment(this InventoryScenario sc, GUI_ItemSlot_Interface src, GUI_ItemSlot_Interface dst)
    {
        Debug.Log((src == null) + " / " + (dst == null));
        GUI_ItemUnit srcDataGUI = src.get_GUI_ItemUnit();
        GUI_ItemUnit dstDataGUI = dst.get_GUI_ItemUnit();

        dst.ApplyDataToGridGUI(srcDataGUI);
        src.ApplyDataToGridGUI(dstDataGUI);
    }
}
