using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<CharacterData> dataList;//DocId�� ���ο� ����
    public static CharacterManager characterManager;
    public int testNum = 1;
    private void Awake()
    {
        testNum = 1;
        dataList = new();
        if (!characterManager)
        {
            characterManager = this;
        }
    }
    public List<CharacterData> GetCharacters() => dataList;
    public CharacterData GetCharacter(int _index) => dataList[_index];

    public void SetCharacters(List<CharacterData> _characterDataDict) => dataList = _characterDataDict;

}
