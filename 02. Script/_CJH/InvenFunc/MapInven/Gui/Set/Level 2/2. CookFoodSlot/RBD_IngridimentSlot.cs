using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBD_IngridimentSlot : MonoBehaviour, IResponedByDrop
{
    [SerializeField] internal GUI_Ctrl myGUI_CTRL;
    [SerializeField] internal GUI_ItemUnit _GUI_ItemUnit;
    public GUI_IngridiSlot myGUI;

    private iRoot_DDO_Manager cash = null;

    public iRoot_DDO_Manager GetDDO_Manager()
    {
        if (cash != null)
            return cash;

        return transform.root.GetComponent<iRoot_DDO_Manager>();
    }

    public iSlotGUI GetTargetSlotGUI()
    {
        return myGUI_CTRL;
    }

    public void DDO_Event_byInvenSlot(SlotGUI_InvenSlot _src)
    {
        RDM_CookSC _RDM_CookSC = GetDDO_Manager() as RDM_CookSC;
        if (_RDM_CookSC)
        {
            _RDM_CookSC.SetIngridiment_byInvenSlot(_src,this);
        }
    }
}
