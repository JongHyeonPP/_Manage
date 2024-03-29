    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CJH_CharacterData : MonoBehaviour
{
    [SerializeField] internal List<CharacterData> _data;//DocId는 내부에 존재 _data;
    private static CJH_CharacterData _ins;
    public List<CharEquipZone> viewData;

    static public CJH_CharacterData getSGT()
    {
        return _ins;
    }

    public void InitData()
    {
        if (_ins != null)
            Debug.Log("????");
        else
            _ins = this;

        var _CharacterManager = CharacterManager.characterManager;
        if (_data.Count != 0)
            return;

        _data.Clear();
        _data = _CharacterManager.GetCharacters();
    }

    public CharacterData getCharData(int _value)
    {
        if (_ins == null)
        {
            return null;
        }

        return _ins._data[_value];
    }

    public void SetEvent(int _value)
    {
        if (_ins == null)
            return;
        _ins.viewData[_value].RefreshText();
    }
}