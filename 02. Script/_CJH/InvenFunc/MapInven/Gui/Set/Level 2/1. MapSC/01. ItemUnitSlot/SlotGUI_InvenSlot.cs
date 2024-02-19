using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SlotGUI_InvenSlot : MonoBehaviour, iInvenSlot
{
    [SerializeField] private GUI_Ctrl myGUI_CTRL;
    [SerializeField] private _SlotItemGuiEvent _ItemEvent;

    public List<int> myAddr = new();
    public GUI_ItemUnit _itemGUI; 
    public Image _ExistEffect;
    public int _index;



    public void _DEBUG(int index, List<int> _data)
    {
        _index = index;
        myAddr = new();
        for (int i = 0; i < _data.Count; i++)
        {
            myAddr.Add(_data[i]);
        }
        myAddr.Add(index);

        _itemGUI = GetComponentInChildren<GUI_ItemUnit>();

        if (_itemGUI)
            _itemGUI.SetAddressData(myAddr);
    }

    public SlotGUI_InvenSlot GetMyItemGUI() { return this; }

    private iRoot_DDO_Manager cash = null;
    public iRoot_DDO_Manager GetDDO_Manager()
    {
        if(cash != null)
            return cash;

        return transform.root.GetComponent<iRoot_DDO_Manager>();
    }


    public Transform GetTransform_ItemGUI()
    {
        if (_itemGUI == null)
            return null;

        return _itemGUI.transform;
    }

    public iSlotGUI GetTargetSlotGUI()
    {
        return myGUI_CTRL;
    }

    // _RBD
    public bool IsInteractable_byGetRBD(iRoot_DDO_Manager _inven, IResponedByDrop target)
    {
        return _inven.CheckUpAvailable(this,target);
    }

    public void InteractDDO_byGetRBD(IResponedByDrop _src)
    {
        if (true)
        {
            if ((_src as SlotGUI_EquipSlot == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, _src as SlotGUI_EquipSlot);
                return;
            }

            else if ((_src as SlotGUI_InvenSlot == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, _src as SlotGUI_InvenSlot);
                return;
            }
            else if ((_src as RBD_CasherZone == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, _src as RBD_CasherZone);
                return;
            }
            else if ((_src as RBD_SellZone == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, _src as RBD_SellZone);
                return;
            }
            else if ((_src as RBD_IngridimentSlot == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, _src as RBD_IngridimentSlot);
                return;
            }
            else if ((_src as RBD_UseDisposable == true))
            {
                GetDDO_Manager().InteractFuncByRBD(this, _src as RBD_UseDisposable);
                return;
            }
        }

        Debug.Log("Not Allocated Case");
        GetDDO_Manager().InteractFuncByRBD(this);
        return;
    }

    public void SetGUI_byItemGUI(GUI_ItemUnit target)
    {
        _ItemEvent.SetGui_byIsNull(target == null);

        if (target == null)
        {
            _ExistEffect.enabled = false;
            _itemGUI = null;
            return;
        }

        _ExistEffect.enabled = true;
        _itemGUI = target;
        target.transform.SetParent(transform);
        target._myData.invenAddr = myAddr;
        target.SetSizeAuto();
    }
}



[Serializable]
internal class GUI_Ctrl : iSlotGUI
{
    public Image SlotImg;
    public Color DEFAULT, ONFOCUS, ABLE, DISABLE;

    public void SetColor_DEFAULT()
    {
        SlotImg.color = DEFAULT;
    }
    public void SetColor_ONFOCUS()
    {
        SlotImg.color = ONFOCUS;
    }
    public void SetColor_ABLE()
    {
        SlotImg.color = ABLE;
    }
    public void SetColor_DISABLE()
    {
        SlotImg.color = DISABLE;
    }

    public void SetColor_BLUE()
    {
        SlotImg.color = Color.blue;
    }
}