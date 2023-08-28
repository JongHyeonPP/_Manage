using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InvenUnit_SkillBook : InvenUnit
{
    public GameObject _go;

    public InvenUnit_SkillBook()
    {
        _index = INDEX;
        _name = "random - " + INDEX;
    }
}