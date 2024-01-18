using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotGUI_EquipSlot : SlotGUI_InvenSlot, iInvenSlot
{
    public int compIndex_0 = 0;
    public int charIndex = 0;
    public int equipIndex = 0;

    public bool IsInteractable_byGetRBD(RDM_MapSC _inven, IResponedByDrop target)
    {
        return _inven.CheckUpScale(this,target);
    }

    new public void InteractDDO_byGetRBD(IResponedByDrop target)
    {
        if (true)
        {
            if ((target as SlotGUI_EquipSlot == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, target as SlotGUI_EquipSlot);
                return;
            }

            else if ((target as SlotGUI_InvenSlot == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, target as SlotGUI_InvenSlot);
                return;
            }
        }

        GetDDO_Manager().InteractFuncByRBD(this);
        return;
    }
}