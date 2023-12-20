using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnumCollection;
using StructCollection;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public GameObject uiCamera;
    public Difficulty difficulty;
    public static Language language;
    private string uid;
    public string Uid
    {
        get { return uid; }
    }
    public Dictionary<string, object> userDoc;
    public Dictionary<string, object> progressDoc;
    public static int seed { get; private set; }
    public float fame;
    public float gold;
    public float fameAscend = 0f;
    public float goldAscend = 0f;
    public List<int> guild = new();

    private static bool isPaused = false;
    public static bool IsPaused
    {
        get { return isPaused; }
        set { Time.timeScale = value ? 0 : 1; isPaused = value; }
    }

    public static BattleScenario battleScenario;
    public static LobbyScenario lobbyScenario;

    #region Battle
    [Header("Battle")]
    public Transform canvasGrid;
    public static List<ObjectGrid> FriendlyGrids { get; private set; } = new();
    public static List<ObjectGrid> EnemyGrids { get; private set; } = new();
    private GameObject scenarioObject;
    public static List<CharacterBase> Friendlies { get; private set; } = new();
    public static List<CharacterBase> Enemies { get; private set; } = new();
    public GameObject objectHpBar;
    #endregion
    public static readonly Color[] talentColors = new Color[4] { Color.blue, Color.green, Color.yellow, Color.red };
    EventTrigger eventTriggerFriendly;
    private readonly float gridCorrection = 20f;
    void Awake()//매니저 세팅은 Awake
    {
        if (!gameManager)
        {
            gameManager = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
            InitGrids();
            DontDestroyOnLoad(uiCamera);
            uiCamera.SetActive(false);
            //Until Steam API
            //uid = "FMefxTlgP9aHsgfE0Grc";//다수
            uid = "KF5U1XMs5cy7n13dgKjF";//소수
        }   
    }
    async void Start()
    {
        progressDoc = await DataManager.dataManager.GetField("Progress", Uid);
    }
    public async Task LoadUserDoc()
    {
        userDoc = await DataManager.dataManager.GetField("User", Uid);

        fame = GetFloatValue(userDoc, "Fame");
        for (int i = 0; i < LoadManager.loadManager.guildDict.Count; i++)
        {
            try
            {
                guild.Add((int)(long)userDoc["Guild_" + i]);
            }
            catch
            {
                DataManager.dataManager.SetDocumentData("User", Uid, "Guild_" + i, 0);
            }
        }
    }
    public void GameOver()
    {
        Debug.Log("GameOver");
        foreach (var x in Enemies)
            x.StopAllCoroutines();
    }
    private void OnSceneLoaded(Scene _arg0, LoadSceneMode _arg1)
    {
        battleScenario = null;
        lobbyScenario = null;
        scenarioObject = GameObject.FindWithTag("SCENARIO");
        uiCamera.SetActive(_arg0.name == "Battle");
        switch (_arg0.name)
        {
            case "Awake":
                SceneManager.LoadScene("Start");
                break;
            case "Battle":
                battleScenario = scenarioObject.GetComponent<BattleScenario>();
                break;
            case "Lobby":
                lobbyScenario = scenarioObject.GetComponent<LobbyScenario>();
                break;
        }
    }
    private void InitGrids()
    {
        DontDestroyOnLoad(canvasGrid);
        canvasGrid.gameObject.SetActive(false);
        Transform panelFriendly = canvasGrid.GetChild(0);

        GridLayoutGroup groupFrinedly = panelFriendly.GetComponent<GridLayoutGroup>();
        panelFriendly.GetComponent<RectTransform>().sizeDelta = new Vector2(groupFrinedly.cellSize.x * 3 + gridCorrection, groupFrinedly.cellSize.y * 3 + gridCorrection);
        var trigger =  panelFriendly.gameObject.AddComponent<EventTrigger>();


        Entry enterEntry = new();
        trigger.triggers.Add(enterEntry);
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) =>
        {
            battleScenario.isInFriendly = true;
        });
        Entry exitEntry = new();
        trigger.triggers.Add(exitEntry);
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) =>
        {
            if (!battleScenario.isDragging) return;
            battleScenario.isInFriendly = false;
        });


        Transform panelEnemy = canvasGrid.GetChild(1);

        eventTriggerFriendly = panelFriendly.gameObject.AddComponent<EventTrigger>();
        Entry downEntry = new();
        eventTriggerFriendly.triggers.Add(downEntry);
        downEntry.eventID = EventTriggerType.PointerDown;
        // Button 이벤트 추가
        gameObject.AddComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Down");
        });


        for (int i = 0; i < 9; i++)
        {
            ObjectGrid friendlyGrid = panelFriendly.GetChild(i).gameObject.AddComponent<ObjectGrid>();
            friendlyGrid.isEnemy = false;
            friendlyGrid.index = i;
            FriendlyGrids.Add(friendlyGrid);
            friendlyGrid.SetClickEvent().SetDownEvent().SetDragEvent().SetEnterEvent().SetExitEvent().SetUpEvent();

            ObjectGrid enemyGrid = panelEnemy.GetChild(i).gameObject.AddComponent<ObjectGrid>();
            enemyGrid.isEnemy = true;
            enemyGrid.index = i;
            EnemyGrids.Add(enemyGrid);
            enemyGrid.SetClickEvent().SetDownEvent().SetDragEvent().SetEnterEvent().SetExitEvent().SetUpEvent();
        }
    }

    public async void LoadGame()
    {
        seed = (int)(long)progressDoc["Seed"];
        await LoadEnemy();
        await LoadFriendly();
        SceneManager.LoadScene((string)progressDoc["Scene"]);
    }
    public void NewGame()
    {
        seed = Random.Range(0, int.MaxValue);
        Random.InitState(seed);
    }
    private async Task LoadFriendly()
    {
        foreach (var x in FriendlyGrids)
        {
            x.gameObject.SetActive(true);
        }
        List<DocumentSnapshot> friendlyDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.uid, "Friendlies"));
        foreach (DocumentSnapshot snapShot in friendlyDocs)
        {
            Dictionary<string, object> tempDict = snapShot.ToDictionary();
            int job = 0;
            string jobId = "0";
            int count = 0;
            List<Skill> skills = new();
            float ability;
            string hpValue;
            float resist;
            float speed;
            if (tempDict.ContainsKey("Ability"))
            {
                ability = GetFloatValue(tempDict, "Ability");
            }
            else
            {
                ability = 0;
            }
            if (tempDict.ContainsKey("Hp"))
            {
                hpValue = (string)tempDict["Hp"];
            }
            else
            {
                hpValue = "1/1";
            }
            float hp = float.Parse(hpValue.Split('/')[0]);
            float maxHp = float.Parse(hpValue.Split('/')[1]);
            if (tempDict.ContainsKey("Resist"))
            {
                resist = GetFloatValue(tempDict, "Resist");
            }
            else
            {
                resist = 0;
            }
            if (tempDict.ContainsKey("Speed"))
            {
                speed = GetFloatValue(tempDict, "Speed");
            }
            else
            {
                speed = 1f;
            }
            for (int i = 0; i < 2; i++)
            {
                if (tempDict.TryGetValue(string.Format("Skill_{0}", i), out object valueObj))
                {
                    count++;
                    string skillID = ((string)valueObj).Split(":::")[0];
                    if (skillID == string.Empty) continue;
                    switch (LoadManager.loadManager.skillsDict[skillID].categori)
                    {
                        case SkillCategori.Power:
                            job += 100;
                            break;
                        case SkillCategori.Sustain:
                            job += 10;
                            break;
                        case SkillCategori.Util:
                            job += 1;
                            break;
                    }
                }
            }
            if (count < 2)
                job = 0;
            jobId = AddZero(job);
            for (int i = 0; i < 2; i++)
            {
                if (tempDict.TryGetValue(string.Format("Skill_{0}", i), out object valueObj))
                {
                    string valueStr = (string)valueObj;
                    if (valueStr == string.Empty) continue;
                    Skill localizedSkill = LocalizeSkill((valueStr));
                    foreach (var x0 in localizedSkill.effects)
                    {
                        SkillEffect effect = x0;
                    }
                    skills.Add(localizedSkill);
                }
            }
            if (skills.Count < 2)
                jobId = "000";
            ObjectGrid _grid = FriendlyGrids[(int)(long)tempDict["Index"]];
            GameObject friendlyObject = InitCharacterObject(_grid, false, jobId);


            FriendlyScript friendlyScript = friendlyObject.AddComponent<FriendlyScript>();
            List<TalentStruct> talents = new();
            if (tempDict.ContainsKey("Talent"))
            {
                foreach (object talentObj in tempDict["Talent"] as List<object>)
                {
                    string talentStr = (string)talentObj;
                    TalentFormStruct talentForm = LoadManager.loadManager.talentDict[talentStr.Split(":::")[0]];
                    List<SkillEffect> effects = new();
                    string[] array = talentStr.Split(":::")[1].Split('/');
                    for (int i = 0; i < array.Length; i++)
                    {
                        //effects.Add(new(float.Parse(array[i]), talentForm.effects[i].type));
                    }
                    talents.Add(new(talentForm.name, talentForm.level, talentForm.explain, effects));
                }
            }
            friendlyScript.InitFriendly(LoadManager.loadManager.jobsDict[jobId], skills, talents, _grid, maxHp, hp, ability, resist, speed );
            Friendlies.Add(friendlyScript);
        }
    }
    public static Skill LocalizeSkill(string x1)//Skill_n/n 형태의 x1을 기반으로 LoadManager에 있는 EffectForm을 가진 SkillStruct 접근해서 Effect를 가진 SkillStruct를 리턴
    {
        string skillID;
        byte skillLevel;
        if (x1.Contains(":::"))
        {
            skillID = x1.Split(":::")[0];
            skillLevel = byte.Parse(x1.Split(":::")[1]);
        }
        else
        {
            skillID = x1;
            skillLevel = 0;
        }
        SkillForm tempSkillForm = LoadManager.loadManager.skillsDict[skillID];
        return new Skill(tempSkillForm, skillLevel);
    }
    private async Task LoadEnemy()
    {
        foreach (var x in EnemyGrids)
        {
            x.gameObject.SetActive(true);
        }
        List<DocumentSnapshot> enemyDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.Uid, "Enemies"));
        foreach (DocumentSnapshot snapShot in enemyDocs)
        {
            Dictionary<string, object> tempDict = snapShot.ToDictionary();
            int index = (int)(long)tempDict["Index"];
            EnemyClass enemyStruct = LoadManager.loadManager.enemyiesDict[(string)tempDict["Id"]];
            ObjectGrid grid = EnemyGrids[index];
            GameObject enemyObject = InitCharacterObject(grid, true, (string)tempDict["Id"]);
            EnemyScript enemyScript = enemyObject.AddComponent<EnemyScript>();

            enemyScript.InitEnemy(enemyStruct.skills, grid, enemyStruct.hp, enemyStruct.ability, enemyStruct.resist, enemyStruct.speed);
            Enemies.Add(enemyScript);
        }
    }

    public GameObject InitCharacterObject(ObjectGrid _grid, bool _isEnemy, string _characterId)
    {
        string type = _isEnemy ? "Enemy" : "Friendly";
        GameObject characterObject = Instantiate(Resources.Load<GameObject>(string.Format("Prefab/{0}/{0}_{1}", type, _characterId)));

        characterObject.transform.GetChild(0).localScale = Vector3.one * 70;
        characterObject.transform.SetParent(_grid.transform);
        _grid.GetComponent<Image>().enabled = true;
        RectTransform rectTransform = characterObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        rectTransform.localScale = new Vector2(1, 1);
        return characterObject;
    }
    static float GetFloatValue(Dictionary<string, object> _dict, string _field)
    {
        if (_dict[_field] is long)
            return (long)_dict[_field];
        else
            return (float)(double)_dict[_field];
    }
    private static string AddZero(int _num)
    {
        if (_num < 10)
            return "00" + _num;
        else if (_num < 100)
            return "0" + _num;
        else
            return _num.ToString();
    }
    public static bool CalculateProbability(float _probability)
    {
        return Random.Range(0f, 1f) <= Mathf.Clamp(_probability, 0f, 1f);
    }
    public void FromCandidateToFriendly(List<RecruitCandidate> _candidates)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject friendlyObject = _candidates[i].transform.GetChild(1).gameObject;
            Debug.Log(friendlyObject.gameObject.name);
            friendlyObject.transform.SetParent(FriendlyGrids[i + 3].transform);
            FriendlyScript friendlyScript = friendlyObject.gameObject.AddComponent<FriendlyScript>();
            Instantiate(objectHpBar, friendlyObject.transform);
            friendlyScript.InitFriendly(LoadManager.loadManager.jobsDict["000"], new List<Skill>(), _candidates[i].info.talents, FriendlyGrids[i + 3], _candidates[i].info.hp, _candidates[i].info.hp, _candidates[i].info.ability, _candidates[i].info.ability,_candidates[i].info.speed );
            Friendlies.Add(friendlyScript);
            friendlyObject.transform.localPosition = Vector3.zero;
            friendlyObject.transform.localScale = Vector3.one;
            friendlyObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;

        }
    }
}