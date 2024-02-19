using System.Collections.Generic;
using UnityEngine;

static public class SyncDataFunc_Map
{
    static public void SetMapData_History(this GameManager gameManager, string _history)
    {
        int _nodeLevel = _history.Split('/').Length - 1;

        if (gameManager != null)
            gameManager.history = _history;
        else
            Debug.Log(_nodeLevel +"_ "+ _history + " <<< Try to Set History");
        //DataManager.dataManager.SetDocumentData("NodeLevel", 노드레벨값, "Progress", GameManager.gameManager.Uid);

        gameManager.SetMapData_NodeLevel(_nodeLevel);
    }

    static public void SetMapData_NodeLevel(this GameManager gameManager, int _NodeLevel)
    {
        if (gameManager != null)
            gameManager.nodeLevel = _NodeLevel;
        
        //Debug.Log(_NodeLevel + " <<< Try to Set NodeLevel");
    }


    static public void SetMapData_CharSkills(this MapScenario _MapScenario, int Char_Index, string[] Skill_NameSet)
    {
        CharacterManager _CharacterManager = CharacterManager.characterManager;
        for (int i = 0; i < Skill_NameSet.Length; i++)
        {
            if (false)
            {
                if (Skill_NameSet[i] != "Null")
                {
                    if (_CharacterManager != null)
                    {
                        CharacterData getChar = _CharacterManager.GetCharacter(Char_Index);
                        getChar.ChangeSkill(i, Skill_NameSet[i]);
                    }
                }
            }   
            else
            {
                if (Skill_NameSet[i] != "Null")
                    Debug.Log(Char_Index + "_Charactor " + i + " skill " + Skill_NameSet[i] + " ");
            }
        }
    }
    static public void SetMapData_CharEquips(this MapScenario _MapScenario, int Char_Index, string[] Skill_NameSet)
    {
        CharacterManager _CharacterManager = CharacterManager.characterManager;
        for (int i = 0; i < Skill_NameSet.Length; i++)
        {
            if (false)
            {
                if (Skill_NameSet[i] != "Null")
                {
                    if (_CharacterManager != null)
                    {
                        CharacterData getChar = _CharacterManager.GetCharacter(Char_Index);
                        getChar.ChangeSkill(i, Skill_NameSet[i]);
                    }
                }
            }
            else
            {
                if (Skill_NameSet[i] != "Null")
                    Debug.Log(Char_Index + "_Charactor " + i + " equip is "+ Skill_NameSet[i]);
            }
        }
    }

    static public string GetSceneName_byEventIndex(this GameManager gameManager, int _NodeEvent)
    {
        int index = (_NodeEvent / 100);
        switch (index)
        {
            case 0: return gameManager.GetCurrStageName(); // non battle event
            case 1: return gameManager.GetCurrStageName(); // balttle
            case 2: return gameManager.GetCurrStageName(); // << Battle
            default:
                break;
        }
        return "???";
    }
}


public static class GetSceneName_currStage
{
    public static string GetCurrStageName(this GameManager gameManager)
    {
        if (gameManager == null)
        {
            return "Stage " + MapDataSGT.GlobalInit().myHistory.stage;
        }

        return "Stage " + gameManager.nodeLevel;
    }
}
