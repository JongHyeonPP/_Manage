using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ItemUnit : MonoBehaviour
{
    public InvenUnit _myMaster;
    public DragDropObj _dragCmp;
    public TextMeshProUGUI _nameText;
    public Transform _trans;

    public Image _img;
    public int _index = -1;

    internal void UpdateObjectGUI(UnitDataTable unitTableValue)
    {
        _nameText.text = _myMaster._name;
        _img.sprite = unitTableValue.getSprite(_myMaster._type, _myMaster._level);
        _img.color = unitTableValue.getColor(_myMaster._type, _myMaster._level);
    }
}
