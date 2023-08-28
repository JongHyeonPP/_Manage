using EnumCollection;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScenario : MonoBehaviour
{
    public GameObject panelNewGame;
    public GameObject panelMainMenu;
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    TMP_Text
        textNewGame,
        textLoadGame,
        textExit,
        textEasy,
        textNormal,
        textHard;

    private void Awake()
    {
        
        textNewGame = panelMainMenu.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        textLoadGame = panelMainMenu.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textExit = panelMainMenu.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        
        textEasy = panelNewGame.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        textNormal = panelNewGame.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textHard = panelNewGame.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        
        panelNewGame.SetActive(false);
        texts =
                new()
                {
                    {
                        textNewGame,
                        new()
                        {
                            { Language.Ko, "�����ϱ�" },
                            { Language.En, "New Game" }
                        }
                    },{
                        textLoadGame,
                        new()
                        {
                            { Language.Ko, "�̾��ϱ�" },
                            { Language.En, "Load Game" }
                        }
                    },{
                        textExit,
                        new()
                        {
                            { Language.Ko, "���� ����" },
                            { Language.En, "Exit" }
                        }
                    },{
                        textEasy,
                        new()
                        {
                            { Language.Ko, "����" },
                            { Language.En, "Easy" }
                        }
                    },{
                        textNormal,
                        new()
                        {
                            { Language.Ko, "����" },
                            { Language.En, "Normal" }
                        }
                    },{
                        textHard,
                        new()
                        {
                            { Language.Ko, "�����" },
                            { Language.En, "Hard" }
                        }
                    },
                };
        SettingManager.onLanguageChange += LanguageChange;
        LanguageChange(GameManager.language);
    }
    public void NewGameBtnClick()
    {
        panelNewGame.SetActive(true);
    }
    public void DifficultySelect(int i)
    {
        GameManager.gameManager.difficulty = (EnumCollection.Difficulty)i;
        SceneManager.LoadScene("Lobby");
    }
    public void CloseDifficulty()
    {
        panelNewGame.SetActive(false);
    }
    public void ExitBtnClick()
    {
        Application.Quit();
    }
    public void LoadBtnClick()
    {
        GameManager.gameManager.LoadGame();
    }
    private void LanguageChange(Language _language)
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[_language];
        }
    }
}
