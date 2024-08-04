using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using LobbyCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScenario : MonoBehaviour
{
    public GameObject selectLight;
    public GameObject buttonNext;
    public List<GameObject> phaseList;
    public UpgradeUi pubUi;
    public UpgradeUi guildUi;
    public RecruitUi recruitUi;
    public DepartUi departUi;
    public Image[] mediumImage_0 = new Image[2];
    public Image[] mediumImage_1 = new Image[2];
    private LobbyCase curCase;
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts = new();
    public GameObject layBlock;
    #region Phase_0
    private Dictionary<string, UpgradeClass> upgrade_Pub;
    private Dictionary<string, UpgradeClass> upgrade_Guild;
    public UpgradeExplainUi upgradeExplainUi;
    #endregion
    #region Phase_1



    public static readonly float defaultHp = 100f;
    public static readonly float defaultAbility = 10f;
    public static readonly float defaultSpeed = 1f;
    public static readonly float defaultResist = 10f;

    public static readonly float hpSd = 10f;
    public static readonly float abilitySd = 1f;
    public static readonly float speedSd = 0.1f;
    public static readonly float resistSd = 1f;

    #endregion
    private void Awake()
    {
        if (GameManager.gameManager == null)
            return;
        GameManager.lobbyScenario = this;
        #region UiSet
        layBlock.SetActive(false);
        selectLight.SetActive(false);
        phaseList[0].SetActive(true);
        phaseList[1].SetActive(false);
        pubUi.gameObject.SetActive(false);
        guildUi.gameObject.SetActive(false);
        recruitUi.gameObject.SetActive(false);
        departUi.gameObject.SetActive(false);
        #endregion
        GameManager.gameManager.textFame.text = GameManager.gameManager.fame.ToString();
        curCase = LobbyCase.None;
        upgrade_Pub = LoadManager.loadManager.upgradeDict.Where(item => item.Value.lobbyCase == "Pub").ToDictionary(item => item.Key, item => item.Value);
        upgrade_Guild = LoadManager.loadManager.upgradeDict.Where(item => item.Value.lobbyCase == "Guild").ToDictionary(item => item.Key, item => item.Value);

        SettingManager.LanguageChangeEvent += OnLanguageChange;
        upgradeExplainUi.gameObject.SetActive(false);
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
                curUpgrade = upgrade_Pub;
                curUi = pubUi;
                break;
            case LobbyCase.Guild:
                curUpgrade = upgrade_Guild;
                curUi = guildUi;
                break;
        }
        TMP_Text titleText = curUi.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        Dictionary<Language, string> langDict = new();
        switch (_lobbyCase)
        {
            default:
                langDict.Add(Language.En, "Pub");
                langDict.Add(Language.Ko, "주점");
                break;
            case LobbyCase.Guild:
                langDict.Add(Language.En, "Guild");
                langDict.Add(Language.Ko, "길드");
                break;
        }
        titleText.text = langDict[GameManager.language];
        texts.Add(titleText, langDict);
        for (int i = 0; i < 3; i++)
        {
            UpgradeSlot slot = curUi.slots[i];
            if (i < curUpgrade.Count)
            {
                slot.gameObject.SetActive(true);
                KeyValuePair<string, UpgradeClass> upgradeKvp = curUpgrade.Where(item => item.Value.index == i).FirstOrDefault();//index가 i인 클래스
                slot.textName.text = upgradeKvp.Value.name[GameManager.language];
                int level = GameManager.gameManager.upgradeLevelDict[upgradeKvp.Key];
                slot.textLv.text = "Lv. " + (level+1);
                slot.curId = upgradeKvp.Key;
                if (level == upgradeKvp.Value.content.Count)
                {
                    slot.button_Up.SetActive(false);
                }
                else
                {
                    slot.button_Up.SetActive(true);
                }
                Dictionary<Language, string> str_name = new() { { Language.Ko, upgradeKvp.Value.name[Language.Ko] }, { Language.En, upgradeKvp.Value.name[Language.En] } };
                texts.Add(slot.textName, str_name);
                //slot
                slot.imageIcon.sprite = upgradeKvp.Value.iconSprite;
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
    public void InitRecruitUi()
    {
        TMP_Text titleText = recruitUi.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        Dictionary<Language, string> langDict = new();
        langDict.Add(Language.En, "Recruit");
        langDict.Add(Language.Ko, "모집");
        titleText.text = langDict[GameManager.language];
        texts.Add(titleText, langDict);
    }
    public void OnPointerClick(LobbyCase _lobbyCase)
    {
        layBlock.SetActive(true);
        curCase = _lobbyCase;
        switch (_lobbyCase)
        {
            case LobbyCase.Pub:
                PubCase();
                break;
            case LobbyCase.Guild:
                GuildCase();
                break;
            case LobbyCase.Recruit:
                RecruitCase();
                break;
            case LobbyCase.Depart:
                DepartCase();
                break;
        }
        SetMediumImage(false);
    }
    private void PubCase()
    {
        pubUi.gameObject.SetActive(true);
        upgradeExplainUi.gameObject.SetActive(false);
    }

    private void GuildCase()
    {
        guildUi.gameObject.SetActive(true);
        upgradeExplainUi.gameObject.SetActive(false);
    }
    private void RecruitCase()
    {
        recruitUi.gameObject.SetActive(true);
    }
    private void DepartCase()
    {
        departUi.gameObject.SetActive(true);
    }
    public void NextPhase()
    {
        SetMediumImage(true);
        phaseList[0].SetActive(false);
        phaseList[1].SetActive(true);
        buttonNext.SetActive(false);
        recruitUi.AllocateApplicant();
    }
    public void SetMediumImage(bool _isActive)
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
            case LobbyCase.Recruit:
            case LobbyCase.Depart:
                foreach (var x in mediumImage_1)
                {
                    x.enabled = _isActive;
                }
                break;
        }
    }


    public void OnUpBtnClicked(UpgradeSlot _upgradeSlot)
    {
        UpgradeClass upgradeClass = LoadManager.loadManager.upgradeDict[_upgradeSlot.curId];
        int level = GameManager.gameManager.upgradeLevelDict[_upgradeSlot.curId];
        if (level == upgradeClass.content.Count-1)
        {
            Debug.Log("최고 레벨");
            return;
        }
        int fameResult = GameManager.gameManager.fame - upgradeClass.content[level+1].price;
        if (fameResult >= 0)//구매 가능하다면
        {
            //클라이언트 Fame 계산
            GameManager.gameManager.fame = fameResult;
            //클라이언트 능력 적용
            GameManager.gameManager.upgradeValueDict[upgradeClass.type] += upgradeClass.content[level + 1].value;
            GameManager.gameManager.upgradeLevelDict[_upgradeSlot.curId]++;
            GameManager.gameManager.textFame.text = GameManager.gameManager.fame.ToString();
            //Firestore
            DataManager.dataManager.SetDocumentData("Fame", fameResult, "User", GameManager.gameManager.Uid);
            DataManager.dataManager.SetDocumentData("Upgrade", GameManager.gameManager.upgradeLevelDict, "User", GameManager.gameManager.Uid);
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
        _upgradeSlot.textLv.text = "Lv. " + (level+1);
    }
    public void OnPointerEnter_Slot(UpgradeSlot _upgradeSlot)
    {
        upgradeExplainUi.gameObject.SetActive(true);
        upgradeExplainUi.gameObject.transform.position = _upgradeSlot.transform.position + new Vector3(-0.2f, 0f, 0f);

        UpgradeClass upgradeClass = LoadManager.loadManager.upgradeDict[_upgradeSlot.curId];
        int level = GameManager.gameManager.upgradeLevelDict[_upgradeSlot.curId];
        upgradeExplainUi.SetExplain(upgradeClass.explain[GameManager.language]);

        string cur;
        if (level != -1)
        {
            cur = upgradeClass.info[GameManager.language].Replace("{Value}", upgradeClass.content[level].value.ToString());
        }
        else
        {
            cur = string.Empty;
        }
        string next;
        if (level != upgradeClass.content.Count-1)
        {
            next = upgradeClass.info[GameManager.language].Replace("{Value}", upgradeClass.content[level+1].value.ToString());
        }
        else
        {
            next = string.Empty;
        }
        upgradeExplainUi.SetInfo(cur, next);

        upgradeExplainUi.SetSize();
    }
    public void OnPointerExit_Slot()
    {
        upgradeExplainUi.gameObject.SetActive(false);
    }
    public void LayBlockClicked()
    {
        if (!GameManager.lobbyScenario)
            return;
        layBlock.SetActive(false);
        LobbyUiBase curUi = null;
        switch (curCase)
        {
            case LobbyCase.Pub:
                curUi = pubUi;
                break;
            case LobbyCase.Guild:
                curUi = guildUi;
                break;
            case LobbyCase.Recruit:
                curUi = recruitUi;
                break;
            case LobbyCase.Depart:
                curUi = departUi;
                break;
        }
        curUi.ExitBtnClicked();
    }

    private void OnLanguageChange()
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[GameManager.language];
        }
    }






    public async void DepartAsync()
    {
        if(recruitUi.selectedSlots.Contains(null))
        {
            string popUpMessage = (GameManager.language == Language.Ko) ? "<color=#0096FF>3명</color>을 고용해야합니다." : "Need to hire <color=#0096FF>3 people.</color>";
            GameManager.gameManager.SetPopUp(popUpMessage);
            return;
        }
        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
        {
            await FromSlotToCharacter();
            GameManager.gameManager.InitProgress();
        });
        GameManager.gameManager.SetGold(0);
        StageScenarioBase.stageNum = 0;
        SceneManager.LoadScene("Stage0");
        
    }
    async Task FromSlotToCharacter( )
    {
        List<ApplicantSlot> selectedSlots = recruitUi.selectedSlots;
        List<CharacterData> characterDataList = new() { null, null, null};
        for (int i = 0; i < selectedSlots.Count; i++)
        {
            ApplicantSlot _slot = selectedSlots[i];
            _slot.templateAnimator.speed = 1f;
            int gridIndex = i + 3;

            int weaponTypeNum = GameManager.AllocateProbability(0.25f, 0.25f, 0.25f, 0.25f);
            WeaponType weaponType;
            string weaponTypeStr;
            switch (weaponTypeNum)
            {
                default:
                    weaponType = WeaponType.Sword;
                    weaponTypeStr = "Sword";
                    break;
                case 1:
                    weaponType = WeaponType.Club;
                    weaponTypeStr = "Club";
                    break;
                case 2:
                    weaponType = WeaponType.Bow;
                    weaponTypeStr = "Bow";
                    break;
                case 3:
                    weaponType = WeaponType.Magic;
                    weaponTypeStr = "Magic";
                    break;

            }

            string weaponId = $"{weaponTypeStr}:::Default";
            List<string> talentStrs = _slot.talents.Select(item => item.talentId).ToList();
            Dictionary<string, object> characterDict = new()
            {
                { "MaxHp", _slot.Hp },
                { "Hp", _slot.Hp },
                { "Ability", _slot.Ability },
                { "Resist", _slot.Resist },
                { "Speed", _slot.Speed },
                { "Body", _slot.bodyDict },
                { "WeaponId", weaponId },
                { "GridIndex", gridIndex },
                { "CharacterIndex", i },
                { "Skill_0", string.Empty },
                { "Skill_1", string.Empty },
                { "Exp", new int[]{ 0,0} },
                { "JobId", "000" },
                { "Talent",  talentStrs}
            };
            string docId = await DataManager.dataManager.SetDocumentData(characterDict,$"Progress/{GameManager.gameManager.Uid}/Characters");

            WeaponClass weapon = LoadManager.loadManager.weaponDict[weaponType]["Default"];
            CharacterData data = _slot.templateObject.AddComponent<CharacterData>();
            data.InitCharacterData(docId, "000", _slot.Hp, _slot.Hp, _slot.Ability, _slot.Resist, _slot.Speed, gridIndex, new SkillAsItem[2] { null, null},new int[2] {0,0 } ,weapon, _slot.talents);
            data.characterHierarchy.SetWeaponSprite(weapon);
            characterDataList[i] = data;

            CharacterInBattle characterAtBattle = _slot.templateObject.AddComponent<CharacterInBattle>();
            characterAtBattle.InitCharacter(data, BattleScenario.CharacterGrids[gridIndex]);
            BattleScenario.characters.Add(characterAtBattle);
            data.characterAtBattle = characterAtBattle;
        }
        GameManager.gameManager.characterList =  characterDataList;
    }
}
