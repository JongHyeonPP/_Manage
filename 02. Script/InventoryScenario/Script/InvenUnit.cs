using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[Serializable]
public class InvenUnit
{
    internal static int INDEX;

    public string _name;
    public int _equipped;
    public int _index;
    public int _type;
    public int _level;
    public int _gridPos;
    public GameObject _mySlave;

    public void Update_gridPos(int x) => _gridPos = x;
    public void Update_equipped(int x) => _equipped = x;

    static public InvenUnit getNullData()
    {
        InvenUnit temp = new InvenUnit();
        temp._name = "NULL";
        return temp;
    }
}

public static class InvenUnitFunc
{
    static internal InvenUnit Debug_AddRandom(this List<InvenUnit> unitList)
    {
        InvenUnit unit = new InvenUnit();//prefab.AddComponent<InvenUnit>();
        unit._index = InvenUnit.INDEX++;
        unit._name = "rd - " + unit._index;
        unit._level = UnityEngine.Random.Range(0, 3);
        unit._type = UnityEngine.Random.Range(0, 3);
        unitList.Add(unit);

        return unit;
    }

    static internal void SortByType(this List<InvenUnit> invenList)
    {
        invenList.Sort(new Comparison<InvenUnit>((n1, n2) => (n1._type).CompareTo(n2._type)));
    }

    static internal void SortByLevel(this List<InvenUnit> invenList)
    {
        invenList.Sort(new Comparison<InvenUnit>((n1, n2) => (n1._level).CompareTo(n2._level)));
    }

    static internal void SortByIndex(this List<InvenUnit> invenList)
    {
        invenList.Sort(new Comparison<InvenUnit>((n1, n2) => (n1._index).CompareTo(n2._index)));
    }
}

