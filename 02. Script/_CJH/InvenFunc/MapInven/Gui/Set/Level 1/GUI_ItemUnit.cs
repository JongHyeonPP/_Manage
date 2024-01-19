using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ItemUnit : MonoBehaviour
{
    public ItemUnit _myData;
    [SerializeField] internal Values_GUI _GUI;

    public void SetSizeAuto(Transform target = null)
    {
        Transform myTrans = transform;

        if (target != null)
            myTrans.SetParent(target);

        myTrans.localPosition = Vector3.zero;
        myTrans.localScale = Vector3.one;
    }

    public void SetAddressData(List<int> _addr)
    {
        _myData.invenAddr = _addr;
    }

    public Image getImageGUI()
    {
        return _GUI.img;
    }
}

[Serializable]
internal class Values_GUI
{
    public TextMeshProUGUI nameText;
    public Image img;
    public Material color_Focused, color_Default,color_Cash;
}