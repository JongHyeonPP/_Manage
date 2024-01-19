using BattleCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cm_DebugSet : MonoBehaviour
{
    public static List<DATA_char> _data;
    public static cm_DebugSet characterManager;

    public CharacterManager cm;
    [SerializeField] List<DATA_char> data;

    private void Awake()
    {
        _data = data;
        /*
        Debug.Log("setData");
        
        List<CharacterData> _Chracters = new(); 
        for (int i = 0; i < data.Count; i++)
        {
            setCharData_byData(i);
        }*/

    }
}

[Serializable]
public class DATA_char
{
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public float speed;
    public Vector2Int skill;
}