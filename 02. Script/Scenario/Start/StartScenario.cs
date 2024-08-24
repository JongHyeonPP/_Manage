using EnumCollection;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScenario : MonoBehaviour
{
    public Transform canvasStart;

    public StartButton newGame;
    public StartButton loadGame;
    public StartButton setting;
    public StartButton exit;

    private bool isOperating;
    public PanelNewGame panelNewGame;

    private void Awake()
    {
        if (!GameManager.gameManager) return;
        GameManager.startScenario = this;

        newGame.SetStrOnLangauge("새로하기", "New Game");
        loadGame.SetStrOnLangauge("이어하기", "Load Game");
        setting.SetStrOnLangauge("환경설정", "Setting");
        exit.SetStrOnLangauge("종료", "Exit");

        SettingManager.LanguageChangeEvent += LanguageChange;
        LanguageChange();
        panelNewGame.gameObject.SetActive(false);
        isOperating = false;
    }
    private async void Start()
    {
        if (GameManager.gameManager)
            if (GameManager.gameManager.progressDoc == null)
            {
                InActiveLoadBtn();
            }
        if (StageScenarioBase.stageCanvas)
        {
            Destroy(StageScenarioBase.stageCanvas.gameObject);
            StageScenarioBase.stageCanvas = null;
        }
    }
    public void OnNewGameButtonClick()
    {
        if (isOperating)
            return;
        if (GameManager.gameManager.progressDoc != null)
        {
            panelNewGame.gameObject.SetActive(true);
        }
        else
            NewGame();
    }
    public async void NewGame()
    {
        isOperating = true;
        //New Game
        DocumentReference docRef = DataManager.dataManager.GetDocumentReference($"Progress/{GameManager.gameManager.Uid}");
        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
        {
            docRef.DeleteAsync();
            ClearCollection("Characters");
            ClearCollection("Enemies");
            ClearCollection("Inventory");
            return Task.CompletedTask;
        });
        //StartCoroutine(GameManager.gameManager.InactiveLoadingInSecond(1f));
        LoadingScenario.LoadScene("Lobby");

        static async void ClearCollection(string _collection)
        {
            List<DocumentSnapshot> snapshots = await DataManager.dataManager.GetDocumentSnapshots($"Progress/{GameManager.gameManager.Uid}/{_collection}");
            foreach (DocumentSnapshot snapshot in snapshots)
            {
                await snapshot.Reference.DeleteAsync();
            }
        }

    }
    public void ExitBtnClick()
    {
        Application.Quit();
    }
    public async void LoadBtnClick()
    {
        if (isOperating)
            return;
        isOperating = true;
        if (!LoadManager.loadManager.isInit)
        {
            await LoadManager.loadManager.LoadDbBaseData();
        }
        GameManager.gameManager.LoadGame();
    }
    public void SettingBtnClick()
    {
        SettingManager.settingManager.settingUi.gameObject.SetActive(true);
    }
    private void LanguageChange()
    {
        newGame.TextChangeOnLanguage();
        loadGame.TextChangeOnLanguage();
        setting.TextChangeOnLanguage();
        exit.TextChangeOnLanguage();
    }
    public void InActiveLoadBtn()
    {
        loadGame.InactiveButton();
    }
}
