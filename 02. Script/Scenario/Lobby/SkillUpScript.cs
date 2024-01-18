using LobbyCollection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpScript : MonoBehaviour
{
    public string docId;
    TMP_Text textName;
    TMP_Text textLevel;
    Button upButton;
    private GuildClass guildClass;
    public void InitSkillUpScript(string _docId)
    {
        docId = _docId;
        guildClass = LoadManager.loadManager.guildDict[docId];
        textName = transform.GetChild(0).GetComponent<TMP_Text>();
        textName.text = guildClass.name[GameManager.language];
        textLevel = transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textLevel.text = GameManager.gameManager.guildLevelDict[docId].ToString();
        upButton = transform.GetChild(1).GetChild(1).GetComponent<Button>();
        upButton.onClick.AddListener(GuildUpBtnClicked);
    }
    public void GuildUpBtnClicked()
    {
        int level = GameManager.gameManager.guildLevelDict[docId];
        if (level >= guildClass.content.Count) return;
        int price = guildClass.content[level].price;
        if (GameManager.gameManager.fame >= price)
        {
            GameManager.gameManager.fame -= price;
            if (!GameManager.gameManager.guildValueDict.ContainsKey(guildClass.type))
            {
                GameManager.gameManager.guildValueDict.Add(guildClass.type, 0);
            }
            GameManager.gameManager.guildValueDict[guildClass.type] += guildClass.content[level].value;
            if (level > 0)
            {
                GameManager.gameManager.guildValueDict[guildClass.type] -= guildClass.content[level-1].value;
            }
            GameManager.gameManager.guildLevelDict[docId] = ++level;
            textLevel.text = level.ToString();
            GameManager.lobbyScenario.GuildSet();
        }
        else
        {
            //안 될때
            Debug.Log("돈 부족!");
        }
    }
}
