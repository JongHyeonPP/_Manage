using EnumCollection;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScenario : MonoBehaviour
{
    public Transform canvasStart;
    private Transform panelMainMenu;
    private Transform panelDifficultySelect;
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
        panelMainMenu = canvasStart.GetChild(0);
        panelDifficultySelect = canvasStart.GetChild(1);

        textNewGame = panelMainMenu.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        textLoadGame = panelMainMenu.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textExit = panelMainMenu.GetChild(2).GetChild(0).GetComponent<TMP_Text>();

        textEasy = panelDifficultySelect.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        textNormal = panelDifficultySelect.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        textHard = panelDifficultySelect.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        panelDifficultySelect.gameObject.SetActive(false);
        texts =
                new()
                {
                    {
                        textNewGame,
                        new()
                        {
                            { Language.Ko, "새로하기" },
                            { Language.En, "New Game" }
                        }
                    },{
                        textLoadGame,
                        new()
                        {
                            { Language.Ko, "이어하기" },
                            { Language.En, "Load Game" }
                        }
                    },{
                        textExit,
                        new()
                        {
                            { Language.Ko, "게임 종료" },
                            { Language.En, "Exit" }
                        }
                    },{
                        textEasy,
                        new()
                        {
                            { Language.Ko, "쉬움" },
                            { Language.En, "Easy" }
                        }
                    },{
                        textNormal,
                        new()
                        {
                            { Language.Ko, "보통" },
                            { Language.En, "Normal" }
                        }
                    },{
                        textHard,
                        new()
                        {
                            { Language.Ko, "어려움" },
                            { Language.En, "Hard" }
                        }
                    },
                };
        SettingManager.onLanguageChange += LanguageChange;
        LanguageChange(GameManager.language);
    }
    public void NewGameBtnClick()
    {
        panelDifficultySelect.gameObject.SetActive(true);
    }
    public void DifficultySelect(int i)
    {
        GameManager.gameManager.difficulty = (Difficulty)i;
        SceneManager.LoadScene("Lobby");
    }
    public void CloseDifficulty()
    {
        panelDifficultySelect.gameObject.SetActive(false);
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
