using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class InventoryScenarioTools
{
    internal static void SwapEquipment(this InventoryScenario sc, GUI_ItemUnit src, GUI_ItemUnit dst)
    {
        InvenUnit temp_DST = dst._myMaster;
        InvenUnit temp_SRC = src._myMaster;

        dst._myMaster._mySlave = src.gameObject;
        src._myMaster._mySlave = dst.gameObject;

        dst._myMaster = temp_SRC;
        src._myMaster = temp_DST;

        src.UpdateObjectGUI(sc.GUI_inven.UnitDataTable);
        dst.UpdateObjectGUI(sc.GUI_inven.UnitDataTable);

        //sc.GUI_inven.Apply();
    }
}
