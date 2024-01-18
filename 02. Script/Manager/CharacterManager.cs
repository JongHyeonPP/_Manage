using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]private List<CharacterData> characterDataList;//DocId는 내부에 존재
    public static CharacterManager characterManager;

    private void Awake()
    {
        characterDataList = new();
        if (!characterManager)
        {
            characterManager = this;
        }
    }
    public List<CharacterData> GetCharacters() => characterDataList;
    public CharacterData GetCharacter(int _index) => characterDataList[_index];

    public void SetCharacters(List<CharacterData> _characterDataDict) => characterDataList = _characterDataDict;
}
