using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class _CharStatFuncSet_DDO
{
    static public bool CompareItem_withStat(this SlotGUI_EquipSlot targetEquip, ItemUnit target)
    {
        int charComp_0 = targetEquip.compIndex_0; int itemComp_0 = target.itemData[0];

        if (charComp_0 != itemComp_0)
        {
            return false;
        }

        if (charComp_0 == 0)
        {
            int charType = targetEquip.equipIndex;
            int itemType = target.itemData[1];

            if (charType != itemType)
            {
                return false;
            }
        }

        if (true)
        {
            int charStat = (int)CJH_GUI_CharData.getSGT().GetChracters()[targetEquip.charIndex].ability;
            int itemStat = target.itemData[2];

            Debug.Log("Need \"ability\" above " + itemStat + ", char stat is "+charStat);
            if(charStat < itemStat)
                return false;
        }

        return true;
    }

    /*
    static public bool CompareItem_withStat(this SlotGUI_ShopEquip targetEquip, ItemUnit target)
    {
        int charComp_0 = targetEquip.compIndex_0; int itemComp_0 = target.itemData[0];

        if (charComp_0 != itemComp_0)
        {
            return false;
        }

        if (charComp_0 == 0)
        {
            int charType = targetEquip.equipIndex;
            int itemType = target.itemData[1];

            if (charType != itemType)
            {
                return false;
            }
        }

        if (true)
        {
            int charStat = SGT_GUI_CharData.getSGT().dataList[targetEquip.charIndex].stat.x;
            int itemStat = target.itemData[2];

            if (charStat < itemStat)
                return false;
        }

        return true;
    }*/
}
