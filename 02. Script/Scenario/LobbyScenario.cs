using EnumCollection;
using LobbyCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScenario : MonoBehaviour
{
    public GameObject selectLight;
    public GameObject buttonNext;
    public List<GameObject> phaseList;
    public GameObject pubUiObj;
    public GameObject guildUiObj;
    public GameObject incruitUiObj;
    public GameObject departUiObj;
    private UpgradeUi pubUi;
    private UpgradeUi guildUi;
    public Image[] mediumImage_0 = new Image[2];
    public Image[] mediumImage_1 = new Image[2];
    private LobbyCase curCase;
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts = new();
    #region Phase_0
    public TMP_Text text_Fame;
    private Dictionary<string, UpgradeClass> upgrade_Pub;
    private Dictionary<string, UpgradeClass> upgrade_Guild;
    public GameObject panel_Explain;
    public GameObject layBlock;
    public TMP_Text text_UpgradeTitle;
    #endregion
    private void Awake()
    {
        GameManager.lobbyScenario = this;
        #region UiSet
        selectLight.SetActive(false);
        phaseList[0].SetActive(true);
        phaseList[1].SetActive(false);
        pubUiObj.SetActive(false);
        guildUiObj.SetActive(false);
        incruitUiObj.SetActive(false);
        departUiObj.SetActive(false);
        #endregion
        text_Fame.text = GameManager.gameManager.fame.ToString();
        curCase = LobbyCase.None;
        upgrade_Pub = LoadManager.loadManager.upgradeDict.Where(item => item.Value.lobbyCase == "Pub").ToDictionary(item => item.Key, item => item.Value);
        upgrade_Guild = LoadManager.loadManager.upgradeDict.Where(item => item.Value.lobbyCase == "Guild").ToDictionary(item => item.Key, item => item.Value);

        SettingManager.LanguageChangeEvent += OnLanguageChange;
        panel_Explain.SetActive(false);
        InitUpgradeUi(LobbyCase.Pub);
        InitUpgradeUi(LobbyCase.Guild);
        
    }
    public void InitUpgradeUi(LobbyCase _lobbyCase)
    {
        Dictionary<string, UpgradeClass> curUpgrade;
        UpgradeUi curUi;
        switch (_lobbyCase)
        {
            default:
                pubUi = pubUiObj.GetComponent<UpgradeUi>();
                curUpgrade = upgrade_Pub;
                curUi = pubUi;
                break;
            case LobbyCase.Guild:
                guildUi = guildUiObj.GetComponent<UpgradeUi>();
                curUpgrade = upgrade_Guild;
                curUi = guildUi;
                break;
        }
        for (int i = 0; i < 3; i++)
        {
            if (i < curUpgrade.Count)
            {
                curUi.slots[i].gameObject.SetActive(true);
                KeyValuePair<string, UpgradeClass> upgradeKvp = curUpgrade.Where(item => item.Value.index == i).FirstOrDefault();//index가 i인 클래스
                curUi.slots[i].textName.text = upgradeKvp.Value.name[GameManager.language];
                int level = GameManager.gameManager.upgradeLevelDict[upgradeKvp.Key];
                curUi.slots[i].textLv.text = "Lv. " + level;
                curUi.slots[i].curId = upgradeKvp.Key;
                if (level == upgradeKvp.Value.content.Count)
                {
                    curUi.slots[i].button_Up.SetActive(false);
                }
                else
                {
                    curUi.slots[i].button_Up.SetActive(true);
                }
                Dictionary<Language, string> str_name= new() { {Language.Ko,  upgradeKvp.Value.name[GameManager.language]},{ Language.En, upgradeKvp.Value.name[GameManager.language]} };
                texts.Add(curUi.slots[i].textName, str_name);
            }
            else
            {
                curUi.slots[i].gameObject.SetActive(false);
            }
        }
    }
    public void OnPointerClick(LobbyCase _lobbyCase)
    {
        layBlock.SetActive(true);
        Debug.Log(_lobbyCase + " Clicked");
        switch (_lobbyCase)
        {
            case LobbyCase.Pub:
                PubCase();
                break;
            case LobbyCase.Guild:
                GuildCase();
                break;
            case LobbyCase.Incruit:
                IncruitCase();
                break;
            case LobbyCase.Depart:
                DepartCase();
                break;
        }
    }
    private void PubCase()
    {
        pubUiObj.SetActive(true);
        UpgradeCase(LobbyCase.Pub);
    }

    private void GuildCase()
    {
        guildUiObj.SetActive(true);
        UpgradeCase(LobbyCase.Guild);
    }
    private void IncruitCase()
    {

    }
    private void DepartCase()
    {

    }
    public void NextPhase()
    {
        SetMediumImage(true);
        phaseList[0].SetActive(false);
        phaseList[1].SetActive(true);
        buttonNext.SetActive(false);
    }
    private void SetMediumImage(bool _isActive)
    {
        switch (curCase)
        {
            case LobbyCase.Pub:
            case LobbyCase.Guild:
                foreach (var x in mediumImage_0)
                {
                    x.enabled = _isActive;
                }
                break;
            case LobbyCase.Incruit:
            case LobbyCase.Depart:
                foreach (var x in mediumImage_1)
                {
                    x.enabled = _isActive;
                }
                break;
        }
    }
    private void UpgradeCase(LobbyCase _lobbyCase)
    {
        panel_Explain.SetActive(false);
        curCase = _lobbyCase;
        SetMediumImage(false);
   
    }

    public void OnUpBtnClicked(UpgradeSlot _upgradeSlot)
    {
        UpgradeClass upgradeClass = LoadManager.loadManager.upgradeDict[_upgradeSlot.curId];
        int level = GameManager.gameManager.upgradeLevelDict[_upgradeSlot.curId];
        if (level == upgradeClass.content.Count)
        {
            Debug.Log("최고 레벨");
            return;
        }
        int fameResult = GameManager.gameManager.fame - upgradeClass.content[level].Item1;
        if (fameResult >= 0)//구매 가능하다면
        {
            //클라이언트 Fame 계산
            GameManager.gameManager.fame = fameResult;
            //클라이언트 능력 적용
            float valueBefore;
            if (level == 0)
                valueBefore = 0f;
            else
                valueBefore = upgradeClass.content[level - 1].Item2;
            float valueAfter = upgradeClass.content[level].Item2;
            float valueResult = valueAfter - valueBefore;
            if (!GameManager.gameManager.upgradeValueDict.ContainsKey(upgradeClass.type))
                GameManager.gameManager.upgradeValueDict.Add(upgradeClass.type, 0f);
            GameManager.gameManager.upgradeValueDict[upgradeClass.type] += valueResult;
            GameManager.gameManager.upgradeLevelDict[_upgradeSlot.curId]++;
            text_Fame.text = GameManager.gameManager.fame.ToString();
            //Firestore
            DataManager.dataManager.SetDocumentData("Fame", fameResult, "User", GameManager.gameManager.Uid);
            Dictionary<string, object> objDict = DataManager.dataManager.ConvertToObjDictionary(GameManager.gameManager.upgradeLevelDict);
            DataManager.dataManager.SetDocumentData("Upgrade", objDict, "User", GameManager.gameManager.Uid);
        }
        else
        {
            Debug.Log("비용 부족");
            return;
        }
        level++;
        if (level == upgradeClass.content.Count)
        {
            _upgradeSlot.button_Up.SetActive(false);
        }
        _upgradeSlot.textLv.text = "Lv. " + level;
    }
    public void OnPointerEnter_Slot(UpgradeSlot _slot)
    {
        panel_Explain.SetActive(true);
        panel_Explain.transform.position = _slot.transform.position + new Vector3(-0.2f, 0f, 0f);
    }
    public void OnPointerExit_Slot()
    {
        panel_Explain.SetActive(false);
    }
    public void ExitLobbyUi()
    {
        switch (curCase)
        {
            case LobbyCase.Pub:
                pubUiObj.SetActive(false);
                panel_Explain.SetActive(false);
                break;
            case LobbyCase.Guild:
                guildUiObj.SetActive(false);
                panel_Explain.SetActive(false);
                break;
            case LobbyCase.Incruit:
                incruitUiObj.SetActive(false);
                break;
            case LobbyCase.Depart:
                departUiObj.SetActive(false);
                break;
        }
        SetMediumImage(true);
    }
    public void LayBlockClicked()
    {
        layBlock.SetActive(false);
        ExitLobbyUi();
    }
    private void OnLanguageChange(Language _language)
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[_language];
        }
    }
}
