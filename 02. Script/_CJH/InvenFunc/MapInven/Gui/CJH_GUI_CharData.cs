    using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CJH_GUI_CharData : MonoBehaviour
{
    private CharacterManager _cm;

    public void InitSGT(ref CJH_GUI_CharData _localData)
    {
        _cm = CharacterManager.characterManager;
    }

    static public CharacterManager getSGT()
    {
        return CharacterManager.characterManager;
    }
}
