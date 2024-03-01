using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class IngredientPoker : MonoBehaviour
{
    [SerializeField] internal Values_CombColor _Value_Comb;

    [SerializeField] private List<MyIngredient> DiceValues = new();
    [SerializeField] internal List<Image> MyObj_InLine = new();
    [SerializeField] internal List<Image> MyObj_OutLine = new();

    private void Start()
    {
        SetDefault();
    }

    internal void InitValues(List<Vector2Int> _input)
    {
        DiceValues.Clear();

        for (int i = 0; i < _input.Count; i++)
        {
            DiceValues.Add(new MyIngredient(_input[i].x, _input[i].y));
        }
    }


    [ContextMenu("Check_PatternList")]
    internal List<Vector2Int> Check_PatternList()
    {
        List<Vector2Int> _PatternList = new();

        if (true)
        {
            for (int i = 0; i < DiceValues.Count; i++)
            {
                int temp_Value = CheckPattern_Value(
                    DiceValues[ReformIndex(i - 1)],
                    DiceValues[ReformIndex(i)],
                    DiceValues[ReformIndex(i + 1)]
                );
                
                int temp_Type = CheckPattern_Type(
                    DiceValues[ReformIndex(i - 1)],
                    DiceValues[ReformIndex(i)],
                    DiceValues[ReformIndex(i + 1)]
                );
                _PatternList.Add(new Vector2Int(temp_Value, temp_Type));
            }
        }

        return _PatternList;


        int ReformIndex(int _value)
        {
            return (5 + _value) % 5;
        }
        int CheckPattern_Value(MyIngredient _prev, MyIngredient _curr, MyIngredient _next)
        {
            List<int> _ValueList = new() { _prev._value, _curr._value, _next._value };
            for (int i = 0; i < _ValueList.Count; i++)
            {
                if (_ValueList[i] == -1)
                    return -1;
            }

            if (_ValueList[2] == _ValueList[0] && _ValueList[2] == _ValueList[1])
            {
                return 1;
            }

            if ((_ValueList[2] - _ValueList[1] == -1) && (_ValueList[1] - _ValueList[0] == -1))
            {
                return 2;
            }
            if ((_ValueList[2] - _ValueList[1] == 1) && (_ValueList[1] - _ValueList[0] == 1))
            {
                return 2;
            }
            return 0;
        }

        int CheckPattern_Type(MyIngredient _prev, MyIngredient _curr, MyIngredient _next)
        {
            List<int> _TypeList = new() { _prev._type, _curr._type, _next._type };
            for (int i = 0; i < _TypeList.Count; i++)
            {
                if (_TypeList[i] == -1)
                    return -1;
            }

            _TypeList.Sort();

            if (_TypeList[2] - _TypeList[0] == 0)
                return 1;
            else if (_TypeList[2] - _TypeList[0] == 2)
                return 2;
            else
                return 0;
        }
    }

    internal void SetDefault()
    {
        for (int i = 0; i < MyObj_OutLine.Count; i++)
        {
            MyObj_InLine[i].color = _Value_Comb.color_CaseX[0];
            MyObj_OutLine[i].color = _Value_Comb.color_CaseY[0];
        }
    }

    internal void SetColor_byReturnData(List<Vector2Int> byReturnData)
    {
        for (int i = 0; i < MyObj_InLine.Count; i++)
        {
            MyObj_InLine[i].color = _Value_Comb.color_CaseX[byReturnData[i].x +1];
        }

        for (int i = 0; i < MyObj_OutLine.Count; i++)
        {
            MyObj_OutLine[i].color = _Value_Comb.color_CaseY[byReturnData[i].y + 1];
        }
    }
}

[Serializable]
internal class MyIngredient
{
    [SerializeField] Vector2Int toShow;
    internal int _type;
    internal int _value;

    internal MyIngredient(int input_1, int input_2)
    {
        _type = input_1;
        _value = input_2;
        toShow = new Vector2Int(_type, _value);
    }
}

[Serializable]
internal class Values_CombColor
{
    public Color[] color_CaseX;
    public Color[] color_CaseY;
}