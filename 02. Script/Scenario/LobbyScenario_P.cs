using UnityEngine;
using TMPro;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using EnumCollection;
using UnityEngine.UI;
using LobbyCollection;
using System.Threading.Tasks;

public class LobbyScenario_P : MonoBehaviour
{
    //public GameObject canvasSelect;
    //public GameObject canvasNpc;
    //private GameObject panelRayBlock;
    //private GameObject panelDepart;
    //private GameObject panelInfo;
    //[HideInInspector] public GameObject panelRecruit;
    //private GameObject panelUpgrade;
    //public List<CandidateSlot> candidates = new();
    //public List<CandidateSlot> selected = new();
    //private readonly int defaultAbility = 8;
    //private readonly int defaultHp = 100;
    //private readonly int defaultResist = 0;
    //private readonly int defaultSpeed = 1; 
    //private GameObject departNpc;
    //private GameObject recruitNpc;
    //private GameObject upgradeNpc;
    //private GameObject talentExplain;
    //private GameObject[] talentObjects = new GameObject[3];
    //private CandidateSlot focusedCandidate;
    //private static readonly TalentStruct noTalent = new(new() { { Language.Ko, "재능 없음" },{Language.En,"No Talent" } },0,
    //    new() { { Language.Ko, "이 사람은 아무런 재능이 없습니다."},{Language.En, "This guy has no talent" } } 
    //    , null);

    //private string[] candidateNames =
    //    {
    //    "준호", "성민", "지훈", "영진", "준영", "상우",
    //    "민규", "현우", "준성", "승현", "민수","성훈", "준혁", "종현", "상현", "지환",
    //    "민재", "용준", "성진", "준우", "민혁", "진우", "준호", "현석", "태윤", "승민",
    //    "현준", "주원", "대현", "현빈","지영", "민지", "서연", "예린", "하은", "소연",
    //    "지현", "서현", "은지", "수빈", "지수", "주은", "서영", "아름", "현정", "승아",
    //    "지원", "소민", "예은", "은영", "하윤", "수현", "소윤", "선영", "지아", "지안",
    //    "아현", "시은", "혜원", "유진"
    //};
    //private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    //TMP_Text
    //    textUpgrageNpc,
    //    textDepartNpc,
    //    textRecruitNpc,
    //    textDepart,
    //    textDepartYes,
    //    textDepartNo,
    //    textFame,
    //    textAbility,
    //    textResist;
    //Dictionary<Language, string> needMoreRecruit =
    //new()
    //{
    //    { Language.Ko, "동료를 더 모집해야합니다." },
    //    { Language.En, "Need to Recruit more Ally." }
    //};
    //private void Awake()
    //{
    //    panelRayBlock = canvasSelect.transform.GetChild(0).gameObject;
    //    panelRayBlock.SetActive(false);
    //    panelDepart = canvasSelect.transform.GetChild(1).gameObject;
    //    panelDepart.SetActive(false);
    //    panelRecruit = canvasSelect.transform.GetChild(2).gameObject;
    //    panelRecruit.SetActive(false);
    //    panelUpgrade = canvasSelect.transform.GetChild(3).gameObject;
    //    panelUpgrade.SetActive(false);
    //    panelUpgrade.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = ((int)GameManager.gameManager.fame).ToString();
    //    panelInfo = panelRecruit.transform.GetChild(2).gameObject;
    //    departNpc = canvasNpc.transform.GetChild(0).gameObject;
    //    recruitNpc = canvasNpc.transform.GetChild(1).gameObject;
    //    upgradeNpc = canvasNpc.transform.GetChild(2).gameObject;
    //    departNpc.SetActive(false);
    //    recruitNpc.SetActive(false);
    //    upgradeNpc.SetActive(true);
    //    SetText();
    //    for (int i = 0; i < 3; i++)
    //    {
    //        talentObjects[i] = panelInfo.transform.GetChild(2).GetChild(0).GetChild(i).gameObject;
    //        talentObjects[i].SetActive(false);
    //    }
    //    foreach (KeyValuePair<string, GuildClass> x in LoadManager.loadManager.guildDict)
    //    {
    //        SkillUpScript skillupScript = panelUpgrade.transform.GetChild(1).GetChild(x.Value.index).gameObject.AddComponent<SkillUpScript>();
    //        skillupScript.InitSkillUpScript(x.Key);
    //    }
    //    texts =
    //            new()
    //            {
    //                {
    //                    textUpgrageNpc,
    //                    new()
    //                    {
    //                        { Language.Ko, "길드 강화" },
    //                        { Language.En, "Guild Upgrade" }
    //                    }
    //                },
    //                {
    //                    textDepartNpc,
    //                    new()
    //                    {
    //                        { Language.Ko, "출발" },
    //                        { Language.En, "Depart" }
    //                    }
    //                },
    //                {
    //                    textRecruitNpc,
    //                    new()
    //                    {
    //                        { Language.Ko, "동료 고용" },
    //                        { Language.En, "Ally Recruit" }
    //                    }
    //                },
    //                {
    //                    textDepart,
    //                    new()
    //                    {
    //                        { Language.Ko, "출발하시겠습니까?" },
    //                        { Language.En, "Would you like to depart?" }
    //                    }
    //                },
    //                {
    //                    textDepartYes,
    //                    new()
    //                    {
    //                        { Language.Ko, "출발하기" },
    //                        { Language.En, "Depart" }
    //                    }
    //                },
    //                {
    //                    textDepartNo,
    //                    new()
    //                    {
    //                        { Language.Ko, "돌아가기" },
    //                        { Language.En, "Back" }
    //                    }
    //                },
    //                {
    //                    textFame,
    //                    new()
    //                    {
    //                        { Language.Ko, "명성 : " },
    //                        { Language.En, "Fame : " }
    //                    }
    //                },
    //                {
    //                    textAbility,
    //                    new()
    //                    {
    //                        { Language.Ko, "능력" },
    //                        { Language.En, "Ability" }
    //                    }
    //                },
    //                {
    //                    textResist,
    //                    new()
    //                    {
    //                        { Language.Ko, "저항력" },
    //                        { Language.En, "Resist" }
    //                    }
    //                },
    //            };
    //    SettingManager.onLanguageChange += LanguageChange0;
    //    LanguageChange0(GameManager.language);

    //    void SetText()
    //    {
    //        talentExplain = panelInfo.transform.GetChild(2).GetChild(1).gameObject;
    //        talentExplain.SetActive(false);
    //        textUpgrageNpc = upgradeNpc.transform.GetChild(1).GetComponent<TMP_Text>();
    //        textDepartNpc = departNpc.transform.GetChild(1).GetComponent<TMP_Text>();
    //        textRecruitNpc = recruitNpc.transform.GetChild(1).GetComponent<TMP_Text>();
    //        textDepart = canvasSelect.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
    //        textDepartYes = canvasSelect.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
    //        textDepartNo = canvasSelect.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
    //        textFame = canvasSelect.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
    //        textAbility = canvasSelect.transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
    //        textResist = canvasSelect.transform.GetChild(2).GetChild(2).GetChild(1).GetChild(2).GetComponent<TMP_Text>();
    //    }
    //}

    //private void AllocateCandidate()
    //{
    //    List<int> indexList = new();
    //    GameManager.gameManager.guildValueDict.TryGetValue(GuildEffectType.AllocateNumberUp, out float numUp);
    //    for (int i = 0; i < numUp + 3; i++)
    //    {
    //        GameObject candidateObject = Instantiate(Resources.Load<GameObject>("Prefab/CandidateSlot"));
    //        CandidateSlot candidate = candidateObject.GetComponent<CandidateSlot>();
    //        candidates.Add(candidate);
    //        candidateObject.transform.SetParent(panelRecruit.transform.GetChild(0));
    //        candidateObject.transform.localScale = Vector3.one;
    //        candidateObject.transform.localPosition = new Vector3(candidateObject.transform.localPosition.x, candidateObject.transform.localPosition.y, 0);
    //        int temp;
    //        while (true)
    //        {
    //            temp = Random.Range(0, candidateNames.Length);
    //            if (indexList.Contains(temp)) continue;
    //            indexList.Add(temp);
    //            break;
    //        }
    //        string name = candidateNames[temp];
    //        float ability = GetTalentRange(defaultAbility, GuildEffectType.AbilityUp);
    //        float hp = GetTalentRange(defaultHp, GuildEffectType.HpUp);
    //        float resist = GetTalentRange(defaultResist, GuildEffectType.ResistUp);
    //        float speed = GetTalentRange(defaultSpeed, GuildEffectType.SpeedUp);
    //        List<TalentStruct> talents = new();
    //        List<string> talentIds = new List<string>(LoadManager.loadManager.talentDict.Keys);
    //        GameManager.gameManager.guildValueDict.TryGetValue(GuildEffectType.TalentNumUp, out float talentNumUp);
    //        int range = Random.Range(0, (int)talentNumUp + 1);
    //        for (int j = 0; j < range; j++)
    //        {
    //            string talentId = talentIds[Random.Range(0, talentIds.Count)];
    //            talentIds.Remove(talentId);
    //            TalentFormStruct baseTalentForm = LoadManager.loadManager.talentDict[talentId];

    //            List<TalentEffect> effects = new();
    //            int talentValue = Random.Range(0, baseTalentForm.level + 1);
    //            foreach (var x in baseTalentForm.effects)
    //            {
    //                string[] valueArray = x.value.Split('/');
    //                float value = float.Parse(valueArray[talentValue]);
    //                effects.Add(new TalentEffect(value, x.type));
    //            }
    //            talents.Add(new(baseTalentForm.name, baseTalentForm.level, baseTalentForm.explain, effects));
    //        }
    //        if (talents.Count == 0)
    //            talents.Add(noTalent);
    //        candidate.candiInfo = new CandiInfo(hp, ability, resist, speed, name, talents);
    //    }
    //    SettingManager.onLanguageChange += LanguageChange1;

    //    float GetTalentRange(int _defaultValue, GuildEffectType _guildEffect)
    //    {
    //        GameManager.gameManager.guildValueDict.TryGetValue(_guildEffect, out float _value);
    //        return Random.Range(_defaultValue, (float)(_defaultValue * 1.5f + 1 + _value));
    //    }
    //}

    //public void RecruitBtnClicked()
    //{
    //    panelRecruit.SetActive(true);
    //    panelRayBlock.SetActive(true);
    //}
    //public void UpgrageBtnClicked()
    //{
    //    panelUpgrade.SetActive(true);
    //    panelRayBlock.SetActive(true);
    //}
    //public void DepartBtnClicked()
    //{
    //    panelDepart.SetActive(true);
    //    panelRayBlock.SetActive(true);
    //}
    //public void ShowSpeech(GameObject _speech)
    //{
    //    _speech.SetActive(true);
    //}
    //public void HideSpeech(GameObject _speech)
    //{
    //    _speech.SetActive(false);
    //}
    //public async void DepartSelect(bool _depart)
    //{
    //    switch (_depart)
    //    {
    //        case true:
    //            if (selected.Count != 3)
    //            {
    //                panelDepart.transform.GetChild(0).GetComponent<TMP_Text>().text = needMoreRecruit[GameManager.language];
    //            }
    //            else
    //            {
    //                await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async Transaction =>
    //                {
    //                    DataManager.dataManager.SetDocumentData("Scene","Stage 0", "Progress", GameManager.gameManager.Uid);
    //                    await FromCandidateToFriendly(selected);
    //                    return Task.CompletedTask;
    //                });
                    
    //                SceneManager.LoadScene("Stage 0");
    //            }
    //            break;
    //        case false:
    //            panelDepart.transform.GetChild(0).GetComponent<TMP_Text>().text = texts[textDepart][GameManager.language];
    //            panelDepart.SetActive(false);
    //            panelRayBlock.SetActive(false);
    //            break;
    //    }
    //}
    //public void LayBlockClicked()
    //{
    //    if (panelRecruit.activeSelf == true)
    //    {
    //        RefreshCandidate();
    //    }
    //    panelDepart.SetActive(false);
    //    panelRecruit.SetActive(false);
    //    panelUpgrade.SetActive(false);
    //    panelRayBlock.SetActive(false);

    //}
    //public void GuildSet()
    //{
    //    panelUpgrade.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = string.Format("{0}", GameManager.gameManager.fame);
    //    DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("User").Document(GameManager.gameManager.Uid);
    //    FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
    //    {
    //        DataManager.dataManager.SetDocumentData( "Guild", DataManager.dataManager.ConvertToObjDictionary(GameManager.gameManager.guildLevelDict), "User", GameManager.gameManager.Uid);
    //        DataManager.dataManager.SetDocumentData( "Fame", GameManager.gameManager.fame, "User", GameManager.gameManager.Uid);
    //        return Task.CompletedTask;
    //    });
    //}
    //public void OnCandidateClicked(CandidateSlot _clickedCandidate)
    //{
    //    if(focusedCandidate)
    //    focusedCandidate.transform.GetChild(1).GetChild(0).GetComponent<Animator>().enabled = false;
    //    focusedCandidate = _clickedCandidate;
    //    for (int i = 0; i < focusedCandidate.candiInfo.talents.Count; i++)
    //    {
    //        talentObjects[i].SetActive(true);
    //        talentObjects[i].GetComponent<Image>().color = GameManager.talentColors[focusedCandidate.candiInfo.talents[i].level];
    //        talentObjects[i].transform.GetChild(0).GetComponent<TMP_Text>().text = focusedCandidate.candiInfo.talents[i].name[GameManager.language];
    //    }
    //    for (int i = focusedCandidate.candiInfo.talents.Count; i < 3; i++)
    //    {
    //        talentObjects[i].SetActive(false);
    //    }
    //    //선택된 후보 능력치 표시
    //    panelInfo.transform.GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.candiInfo.name;
    //    panelInfo.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.candiInfo.ability.ToString("F0");
    //    panelInfo.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.candiInfo.hp.ToString("F0");
    //    panelInfo.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.candiInfo.resist.ToString("F1");
    //    panelInfo.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.candiInfo.speed.ToString("F1");
    //    panelRecruit.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = texts[textAbility][GameManager.language];
    //    panelRecruit.transform.GetChild(2).GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = texts[textResist][GameManager.language];
    //    foreach (var x in candidates)
    //    {
    //        x.objectButton.SetActive(x == _clickedCandidate);
    //    }
    //    _clickedCandidate.transform.GetChild(1).GetChild(0).GetComponent<Animator>().enabled = true;
    //}
    //public void RefreshCandidate()
    //{
    //    panelInfo.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Empty;
    //    foreach (var x in talentObjects)
    //    {
    //        x.SetActive(false);
    //    }
    //    if(focusedCandidate)
    //    focusedCandidate.transform.GetChild(1).GetChild(0).GetComponent<Animator>().enabled = false;
    //    focusedCandidate = null;
    //    foreach (var x in candidates)
    //    {
    //        panelInfo.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "-";
    //        panelInfo.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "-";
    //        panelInfo.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "-";
    //        panelInfo.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = "-";
    //        x.objectButton.SetActive(false);
    //    }
    //}
    //private void LanguageChange0(Language _language)
    //{
    //    foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
    //    {
    //        keyValue.Key.text = keyValue.Value[_language];
    //    }
    //}
    //private void LanguageChange1(Language _language)
    //{
    //    for (int i = 0; i < focusedCandidate.candiInfo.talents.Count; i++)
    //    {
    //        TalentStruct talent = focusedCandidate.candiInfo.talents[i];
    //        talentObjects[i].transform.GetChild(0).GetComponent<TMP_Text>().text = talent.name[_language];
    //    }
    //}
    //public void NextBtnClicked()
    //{
    //    canvasSelect.transform.GetChild(4).gameObject.SetActive(false);
    //    departNpc.SetActive(true);
    //    recruitNpc.SetActive(true);
    //    upgradeNpc.SetActive(false);
    //    AllocateCandidate();
    //}
    //public void EnterTalentObject(int _index)
    //{
    //    talentExplain.SetActive(true);
    //    TalentStruct talent = focusedCandidate.candiInfo.talents[_index];
    //    if (talent.effects == null)
    //    {
    //        talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = talent.explain[GameManager.language];
    //        return;
    //    }
    //    switch (talent.effects.Count)
    //    {
    //        default:
    //            talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format(talent.explain[GameManager.language], Formating(talent.effects[0].value));
    //            break;
    //        case 2:
    //            talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format(talent.explain[GameManager.language], Formating(talent.effects[0].value), Formating(talent.effects[1].value));
    //            break;
    //        case 3:
    //            talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format(talent.explain[GameManager.language], Formating(talent.effects[0].value), Formating(talent.effects[1].value), Formating(talent.effects[2].value));
    //            break;
    //    }

    //    static int Formating(float _value)
    //    {
    //        return (int)(System.Math.Round(_value,2) * 100);
    //    }
    //}
    //public void ExitTalentObject()
    //{
    //    talentExplain.SetActive(false);
    //}
    //public async Task FromCandidateToFriendly(List<CandidateSlot> _candidates)
    //{
    //    List<CandidateSlot> candidates = new(_candidates);
    //    List<CharacterData> characterDataList = new();
    //    for (int i = 0; i < 3; i++)
    //    {
    //        GameObject friendlyObject = candidates[i].transform.GetChild(1).gameObject;
    //        int gridIndex = i + 3;
    //        friendlyObject.transform.SetParent(BattleScenario.FriendlyGrids[gridIndex].transform);
    //        FriendlyScript friendlyScript = friendlyObject.gameObject.AddComponent<FriendlyScript>();
    //        Dictionary<string, object> characterField = new();
    //        characterField.Add("Ability", candidates[i].candiInfo.ability);
    //        characterField.Add("Hp", candidates[i].candiInfo.hp + "/" + candidates[i].candiInfo.hp);
    //        characterField.Add("Index", gridIndex);
    //        characterField.Add("Resist", candidates[i].candiInfo.resist);
    //        characterField.Add("Skill_0", string.Empty);
    //        characterField.Add("Skill_1", string.Empty);
    //        characterField.Add("Weapon", string.Empty);
    //        characterField.Add("Speed", candidates[i].candiInfo.speed);
    //        string docId = await DataManager.dataManager.SetDocumentData(characterField, string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Friendlies"));
            
    //        friendlyScript.InitFriendly(docId);
    //        characterDataList.Add(new CharacterData(docId, "000", candidates[i].candiInfo.hp, candidates[i].candiInfo.hp,
    //            candidates[i].candiInfo.ability, candidates[i].candiInfo.resist, candidates[i].candiInfo.speed,gridIndex, new string[2] { "",""}, string.Empty));
            
    //        BattleScenario.friendlies.Add(friendlyScript);
    //        friendlyObject.transform.localPosition = Vector3.zero;
    //        friendlyObject.transform.localScale = Vector3.one;
    //        friendlyObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;

    //    }
    //    CharacterManager.characterManager.SetCharacters(characterDataList);
    //    candidates.Clear();
    //    selected.Clear();
    //}
}
