using EnumCollection;
using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private List<CharacterData> characterDataDict;//DocumentId : CharacterData
    public static CharacterManager characterManager;

    private void Awake()
    {
        characterDataDict = new();
        if (!characterManager)
        {
            characterManager = this;
        }
    }
    public List<CharacterData> GetChracters() => characterDataDict;
    public void SetCharacters(List<CharacterData> _characterDataDict) => characterDataDict = _characterDataDict;
}
