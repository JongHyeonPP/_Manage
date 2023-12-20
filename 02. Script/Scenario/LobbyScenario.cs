using UnityEngine;
using TMPro;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using EnumCollection;
using StructCollection;
using UnityEngine.UI;

public class LobbyScenario : MonoBehaviour
{
    public GameObject canvasSelect;
    public GameObject canvasNpc;
    private GameObject panelRayBlock;
    private GameObject panelDepart;
    private GameObject panelInfo;
    [HideInInspector] public GameObject panelRecruit;
    private GameObject panelUpgrade;
    public List<RecruitCandidate> candidates = new();
    public List<RecruitCandidate> selected = new();
    private readonly int defaultAbility = 8;
    private readonly int defaultHp = 100;
    private readonly int defaultResist = 0;
    private GameObject departNpc;
    private GameObject recruitNpc;
    private GameObject upgradeNpc;
    private GameObject talentExplain;
    private GameObject[] talentObjects = new GameObject[3];
    private RecruitCandidate focusedCandidate;
    private static readonly TalentStruct noTalent = new(new() { { Language.Ko, "재능 없음" },{Language.En,"No Talent" } },0,
        new() { { Language.Ko, "이 사람은 아무런 재능이 없습니다."},{Language.En, "This guy has no talent" } } 
        , null);

    private string[] candidateNames =
        {
        "준호", "성민", "지훈", "영진", "준영", "상우",
        "민규", "현우", "준성", "승현", "민수","성훈", "준혁", "종현", "상현", "지환",
        "민재", "용준", "성진", "준우", "민혁", "진우", "준호", "현석", "태윤", "승민",
        "현준", "주원", "대현", "현빈","지영", "민지", "서연", "예린", "하은", "소연",
        "지현", "서현", "은지", "수빈", "지수", "주은", "서영", "아름", "현정", "승아",
        "지원", "소민", "예은", "은영", "하윤", "수현", "소윤", "선영", "지아", "지안",
        "아현", "시은", "혜원", "유진"
    };
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    TMP_Text
        textUpgrageNpc,
        textDepartNpc,
        textRecruitNpc,
        textDepart,
        textDepartYes,
        textDepartNo,
        textFame,
        textAbility,
        textResist;
    Dictionary<Language, string> needMoreRecruit =
    new()
    {
        { Language.Ko, "동료를 더 모집해야합니다." },
        { Language.En, "Need to Recruit more Ally." }
    };
    private void Awake()
    {
        panelRayBlock = canvasSelect.transform.GetChild(0).gameObject;
        panelRayBlock.SetActive(false);
        panelDepart = canvasSelect.transform.GetChild(1).gameObject;
        panelDepart.SetActive(false);
        panelRecruit = canvasSelect.transform.GetChild(2).gameObject;
        panelRecruit.SetActive(false);
        panelUpgrade = canvasSelect.transform.GetChild(3).gameObject;
        panelUpgrade.SetActive(false);
        panelUpgrade.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = ((int)GameManager.gameManager.fame).ToString();
        panelInfo = panelRecruit.transform.GetChild(2).gameObject;
        departNpc = canvasNpc.transform.GetChild(0).gameObject;
        recruitNpc = canvasNpc.transform.GetChild(1).gameObject;
        upgradeNpc = canvasNpc.transform.GetChild(2).gameObject;
        departNpc.SetActive(false);
        recruitNpc.SetActive(false);
        upgradeNpc.SetActive(true);
        talentExplain = panelInfo.transform.GetChild(2).GetChild(1).gameObject;
        talentExplain.SetActive(false);
        textUpgrageNpc = upgradeNpc.transform.GetChild(1).GetComponent<TMP_Text>();
        textDepartNpc = departNpc.transform.GetChild(1).GetComponent<TMP_Text>();
        textRecruitNpc = recruitNpc.transform.GetChild(1).GetComponent<TMP_Text>();
        textDepart = canvasSelect.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textDepartYes = canvasSelect.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textDepartNo = canvasSelect.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        textFame = canvasSelect.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        textAbility = canvasSelect.transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        textResist= canvasSelect.transform.GetChild(2).GetChild(2).GetChild(1).GetChild(2).GetComponent<TMP_Text>();
        for (int i = 0; i < 3; i++)
        {
            talentObjects[i] = panelInfo.transform.GetChild(2).GetChild(0).GetChild(i).gameObject;
            talentObjects[i].SetActive(false);
        }
        texts =
                new()
                {
                    {
                        textUpgrageNpc,
                        new()
                        {
                            { Language.Ko, "길드 강화" },
                            { Language.En, "Guild Upgrade" }
                        }
                    },
                    {
                        textDepartNpc,
                        new()
                        {
                            { Language.Ko, "출발" },
                            { Language.En, "Depart" }
                        }
                    },
                    {
                        textRecruitNpc,
                        new()
                        {
                            { Language.Ko, "동료 고용" },
                            { Language.En, "Ally Recruit" }
                        }
                    },
                    {
                        textDepart,
                        new()
                        {
                            { Language.Ko, "출발하시겠습니까?" },
                            { Language.En, "Would you like to depart?" }
                        }
                    },
                    {
                        textDepartYes,
                        new()
                        {
                            { Language.Ko, "출발하기" },
                            { Language.En, "Depart" }
                        }
                    },
                    {
                        textDepartNo,
                        new()
                        {
                            { Language.Ko, "돌아가기" },
                            { Language.En, "Back" }
                        }
                    },
                    {
                        textFame,
                        new()
                        {
                            { Language.Ko, "명성 : " },
                            { Language.En, "Fame : " }
                        }
                    },
                    {
                        textAbility,
                        new()
                        {
                            { Language.Ko, "능력" },
                            { Language.En, "Ability" }
                        }
                    },
                    {
                        textResist,
                        new()
                        {
                            { Language.Ko, "저항력" },
                            { Language.En, "Resist" }
                        }
                    },
                };
        SettingManager.onLanguageChange += LanguageChange0;
        LanguageChange0(GameManager.language);
        
    }

    private void AllocateCandidate()
    {
        //List<int> indexList = new();
        //for (int i = 0; i < GameManager.gameManager.guild[0] + 3; i++)
        //{
        //    GameObject candidateObject = Instantiate(Resources.Load<GameObject>("Prefab/Friendly/RecruitCandidate"));
        //    RecruitCandidate candidate = candidateObject.GetComponent<RecruitCandidate>();
        //    candidates.Add(candidate);
        //    candidateObject.transform.SetParent(panelRecruit.transform.GetChild(0));
        //    candidateObject.transform.localScale = Vector3.one;
        //    candidateObject.transform.localPosition = new Vector3(candidateObject.transform.localPosition.x, candidateObject.transform.localPosition.y, 0);
        //    int temp;
        //    while (true)
        //    {
        //        temp = Random.Range(0, candidateNames.Length);
        //        if (indexList.Contains(temp)) continue;
        //        indexList.Add(temp);
        //        break;
        //    }
        //    string name = candidateNames[temp];
        //    float ability = Random.Range(defaultAbility, (int)(defaultAbility * 1.5f + 1 + GameManager.gameManager.guild[1]));
        //    float hp = Random.Range(defaultHp, (int)(defaultHp * 1.5f + 1 + GameManager.gameManager.guild[2] * 10));
        //    float resist = Random.Range(defaultResist, (int)(defaultResist * 1.5f + 1 + GameManager.gameManager.guild[2]));
        //    List<TalentStruct> talents = new();
        //    int startIndex = Random.Range(0, 2);
        //    List<int> dupList = new();
        //    for (int j = startIndex; j < Random.Range(1, GameManager.gameManager.guild[4]+ 2); j++)
        //    {
        //        int talentId = Random.Range(0, LoadManager.loadManager.talentDict.Count);
        //        TalentFormStruct baseTalentForm = LoadManager.loadManager.talentDict[talentId.ToString()];

        //        while (dupList.Contains(talentId))
        //        {
        //            talentId = Random.Range(0, LoadManager.loadManager.talentDict.Count);
        //        }
        //        dupList.Add(talentId);
                
        //        List<T_Effect> effects = new();
        //        int talentValue = Random.Range(0, baseTalentForm.level + 1);
        //        foreach (var x in baseTalentForm.effects)
        //        {
        //            string[] valueArray = x.value.Split('/');
        //            float value = float.Parse(valueArray[talentValue]);
        //            effects.Add(new T_Effect(value, x.type));
        //        }
        //        talents.Add(new(baseTalentForm.name,baseTalentForm.level, baseTalentForm.explain, effects));
        //    }
        //    if(talents.Count==0)
        //    talents.Add(noTalent);
        //    candidate.info = new CandidateInfoStruct().
        //        SetName(name).
        //        SetAbility(ability).
        //        SetHp(hp).
        //        SetResist(resist).
        //        SetTalent(talents);
        //}
        //SettingManager.onLanguageChange += LanguageChange1;
    }

    public void RecruitBtnClicked()
    {
        panelRecruit.SetActive(true);
        panelRayBlock.SetActive(true);
    }
    public void UpgrageBtnClicked()
    {
        panelUpgrade.SetActive(true);
        panelRayBlock.SetActive(true);
    }
    public void DepartBtnClicked()
    {
        panelDepart.SetActive(true);
        panelRayBlock.SetActive(true);
    }
    public void ShowSpeech(GameObject _speech)
    {
        _speech.SetActive(true);
    }
    public void HideSpeech(GameObject _speech)
    {
        _speech.SetActive(false);
    }
    public void DepartSelect(bool _depart)
    {
        switch (_depart)
        {
            case true:
                if (selected.Count != 3)
                {
                    panelDepart.transform.GetChild(0).GetComponent<TMP_Text>().text = needMoreRecruit[GameManager.language];
                }
                else
                {
                    GameManager.gameManager.FromCandidateToFriendly(selected);
                    SceneManager.LoadScene("Map");
                }
                break;
            case false:
                panelDepart.transform.GetChild(0).GetComponent<TMP_Text>().text = texts[textDepart][GameManager.language];
                panelDepart.SetActive(false);
                panelRayBlock.SetActive(false);
                break;
        }
    }
    public void LayBlockClicked()
    {
        if (panelRecruit.activeSelf == true)
        {
            RefreshCandidate();
        }
        panelDepart.SetActive(false);
        panelRecruit.SetActive(false);
        panelUpgrade.SetActive(false);
        panelRayBlock.SetActive(false);

    }
    public void GuildSet(int _index, int _value)
    {
        panelUpgrade.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = string.Format("{0}", GameManager.gameManager.fame);
        DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("User").Document(GameManager.gameManager.Uid);
        FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
        {
            DataManager.dataManager.SetDocumentData("User", GameManager.gameManager.Uid, "Guild_" + _index, _value);
            DataManager.dataManager.SetDocumentData("User", GameManager.gameManager.Uid, "Fame", GameManager.gameManager.fame);
            return System.Threading.Tasks.Task.CompletedTask;
        });
    }
    public void OnCandidateClicked(RecruitCandidate _clickedCandidate)
    {
        if(focusedCandidate)
        focusedCandidate.transform.GetChild(1).GetChild(0).GetComponent<Animator>().enabled = false;
        focusedCandidate = _clickedCandidate;
        for (int i = 0; i < focusedCandidate.info.talents.Count; i++)
        {
            talentObjects[i].SetActive(true);
            talentObjects[i].GetComponent<Image>().color = GameManager.talentColors[focusedCandidate.info.talents[i].level];
            talentObjects[i].transform.GetChild(0).GetComponent<TMP_Text>().text = focusedCandidate.info.talents[i].name[GameManager.language];
        }
        for (int i = focusedCandidate.info.talents.Count; i < 3; i++)
        {
            talentObjects[i].SetActive(false);
        }
        panelInfo.transform.GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.info.name;
        panelInfo.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.info.ability.ToString();
        panelInfo.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.info.hp.ToString();
        panelInfo.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = _clickedCandidate.info.resist.ToString();

        panelRecruit.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = texts[textAbility][GameManager.language];
        panelRecruit.transform.GetChild(2).GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = texts[textResist][GameManager.language];
        foreach (var x in candidates)
        {
            x.objectButton.SetActive(x == _clickedCandidate);
        }
        _clickedCandidate.transform.GetChild(1).GetChild(0).GetComponent<Animator>().enabled = true;
    }
    public void RefreshCandidate()
    {
        panelInfo.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Empty;
        foreach (var x in talentObjects)
        {
            x.SetActive(false);
        }
        if(focusedCandidate)
        focusedCandidate.transform.GetChild(1).GetChild(0).GetComponent<Animator>().enabled = false;
        focusedCandidate = null;
        foreach (var x in candidates)
        {
            panelInfo.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "-";
            panelInfo.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "-";
            panelInfo.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "-";
            x.objectButton.SetActive(false);
        }
    }
    private void LanguageChange0(Language _language)
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[_language];
        }
    }
    private void LanguageChange1(Language _language)
    {
        for (int i = 0; i < focusedCandidate.info.talents.Count; i++)
        {
            TalentStruct talent = focusedCandidate.info.talents[i];
            talentObjects[i].transform.GetChild(0).GetComponent<TMP_Text>().text = talent.name[_language];
        }
    }
    public void NextBtnClicked()
    {
        canvasSelect.transform.GetChild(4).gameObject.SetActive(false);
        departNpc.SetActive(true);
        recruitNpc.SetActive(true);
        upgradeNpc.SetActive(false);
        AllocateCandidate();
    }
    public void EnterTalentObject(int _index)
    {
        talentExplain.SetActive(true);
        TalentStruct talent = focusedCandidate.info.talents[_index];
        if (talent.effects == null)
        {
            talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = talent.explain[GameManager.language];
            return;
        }
        switch (talent.effects.Count)
        {
            default:
                talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format(talent.explain[GameManager.language], Formating(talent.effects[0].value));
                break;
            case 2:
                talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format(talent.explain[GameManager.language], Formating(talent.effects[0].value), Formating(talent.effects[1].value));
                break;
            case 3:
                talentExplain.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format(talent.explain[GameManager.language], Formating(talent.effects[0].value), Formating(talent.effects[1].value), Formating(talent.effects[2].value));
                break;
        }

        static int Formating(float _value)
        {
            return (int)(System.Math.Round(_value,2) * 100);
        }
    }
    public void ExitTalentObject()
    {
        talentExplain.SetActive(false);
    }
}
