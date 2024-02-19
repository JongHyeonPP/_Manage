    using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CJH_CharacterData : MonoBehaviour
{
    [SerializeField] internal List<CharacterData> _data;//DocId는 내부에 존재 _data;
    private static CJH_CharacterData _ins;
    public List<CharEquipZone> viewData;

    public void InitSGT(ref CJH_CharacterData _localData)
    {
        if (_ins == null)
        {
            _ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(_localData.gameObject);
            _localData = _ins;
        }
    }

    static public CJH_CharacterData getSGT() {
        if (_ins == null)
            return null;
        else
            return _ins;
    }

    static public CharacterData getCharData(int _value)
    {
        if (_ins == null)
        {
            Debug.Log("???");
            return null;
        }

        return _ins._data[_value];
    }
    static public void SetEvent(int _value)
    {
        if (_ins == null)
            return;
        _ins.viewData[_value].RefreshText();
    }
}