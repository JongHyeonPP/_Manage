using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StructCollection;

public class SkillUpScript : MonoBehaviour
{
    public int index;
    private int level;
    TMP_Text textName;
    TMP_Text textLevel;
    GuildStruct abilityStruct;
    private void Awake()
    {
        abilityStruct = LoadManager.loadManager.guildDict[index.ToString()];
        textName = transform.GetChild(0).GetComponent<TMP_Text>();
        textName.text = abilityStruct.name;
        textLevel = transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        level = GameManager.gameManager.guild[index];
        textLevel.text = level.ToString();
    }
    public void AbilityUpBtnClicked()
    {
        if (level >= abilityStruct.prices.Count) return;
        if (GameManager.gameManager.fame >= abilityStruct.prices[level])
        {
            GameManager.gameManager.fame -= abilityStruct.prices[level];
            GameManager.gameManager.guild[index] = ++level;
            textLevel.text = level.ToString();
            GameManager.lobbyScenario.GuildSet(index, level);
        }
        else
        {
            //안 될때
            Debug.Log("돈 부족!");
        }
    }
}
