using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ItemUnit : MonoBehaviour
{
    public InvenUnit _myData;
    public DragDropObj _dragCmp;
    public TextMeshProUGUI _nameText;
    public Transform _trans;
    public RectTransform _rectTrans;
    public Image _img;

    public void SetSizeAuto()
    {
        _rectTrans.offsetMin = new Vector2(0, 0);
        _rectTrans.offsetMax = new Vector2(0, 0);
    }
}

