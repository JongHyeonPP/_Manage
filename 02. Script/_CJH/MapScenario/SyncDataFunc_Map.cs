using UnityEngine;

static public class SyncDataFunc_Map
{
    static public void SetMapData_History(this GameManager gameManager, string _history)
    {
        int _nodeLevel = _history.Split('/').Length - 1;
        /*
        if (gameManager != null)
            gameManager.history = _history;
        else
            Debug.Log(_nodeLevel +"_ "+ _history + " <<< Try to Set History");*/
        //DataManager.dataManager.SetDocumentData("NodeLevel", 노드레벨값, "Progress", GameManager.gameManager.Uid);

        gameManager.SetMapData_NodeLevel(_nodeLevel);
    }

    static public void SetMapData_NodeLevel(this GameManager gameManager, int _NodeLevel)
    {
        if (gameManager != null)
            gameManager.nodeLevel = _NodeLevel;
        
        //Debug.Log(_NodeLevel + " <<< Try to Set NodeLevel");
    }

    static public void SetMapData_CharSkills(this GameManager gameManager, int _NodeLevel)
    {
        Debug.Log(_NodeLevel + " <<< Try to Set NodeLevel");
    }

    static public string GetSceneName_byEventIndex(this GameManager gameManager, int _NodeEvent)
    {
        int index = (_NodeEvent / 100);
        switch (index)
        {
            case 0: return "Stage0";
            case 1: return "Shop";
            case 2: return "Stage0"; // << Battle
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
            return "Stage0";

        return "Stage" + gameManager.nodeLevel;
    }
}
