using BattleCollection;
using EnumCollection;
using Firebase.Firestore;
using LobbyCollection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public Dictionary<string, int> guildLevelDict = new();//DocId : Level
    public Dictionary<GuildEffectType, float> guildValueDict = new();
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

    private GameObject scenarioObject;
    public GameObject objectHpBar;
    #endregion
    public static readonly Color[] talentColors = new Color[4] { Color.blue, Color.green, Color.yellow, Color.red };
    EventTrigger eventTrigger;

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
            uid = "FMefxTlgP9aHsgfE0Grc";
            //uid = "KF5U1XMs5cy7n13dgKjF";
        }
    }
    async void Start()
    {
        progressDoc = await DataManager.dataManager.GetField("Progress", Uid);
        BattleScenario.friendlies = new();
        BattleScenario.enemies = new();
    }
    public async Task LoadUserDoc()
    {
        userDoc = await DataManager.dataManager.GetField("User", Uid);
        Dictionary<string, object> guildDict;
        if (userDoc.TryGetValue("Guild", out object guildObj))
        {
            guildDict = guildObj as Dictionary<string, object>;
        }
        else
        {
            guildDict = new();
        }
        fame = GetFloatValue(userDoc, "Fame");
        bool needToSet = false;
        
        foreach (KeyValuePair<string, GuildClass> kvp in LoadManager.loadManager.guildDict)
        {
            if (guildDict.TryGetValue(kvp.Key, out object userObj))
            {
                int level = (int)(long)userObj;
                guildLevelDict.Add(kvp.Key, level);
                if (level != 0)
                {
                    GuildEffectType type = kvp.Value.type;
                    if (!guildValueDict.ContainsKey(type))
                    {

                        guildValueDict.Add(type, kvp.Value.content[level - 1].value);//1 Level이면 0번 째 Value를 챙겨야 함
                    }
                    else
                        guildValueDict[type] += kvp.Value.content[level - 1].value;
                }
            }
            else
            {
                needToSet = true;
                guildLevelDict.Add(kvp.Key, 0);
            }
        }
        if (needToSet)
        {
            Dictionary<string, object> objDict = DataManager.dataManager.ConvertToObjDictionary(guildLevelDict);
            DataManager.dataManager.SetDocumentData("Guild", objDict, "User", Uid);
        }
        
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
                battleScenario.OnBattleLoaded();
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
        panelFriendly.GetComponent<RectTransform>().sizeDelta = new Vector2(groupFrinedly.cellSize.x * 3 + BattleScenario.gridCorrection, groupFrinedly.cellSize.y * 3 + BattleScenario.gridCorrection);
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

        eventTrigger = panelFriendly.gameObject.AddComponent<EventTrigger>();
        Entry downEntry = new();
        eventTrigger.triggers.Add(downEntry);
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
            BattleScenario.FriendlyGrids.Add(friendlyGrid);
            friendlyGrid.SetClickEvent().SetDownEvent().SetDragEvent().SetEnterEvent().SetExitEvent().SetUpEvent();

            ObjectGrid enemyGrid = panelEnemy.GetChild(i).gameObject.AddComponent<ObjectGrid>();
            enemyGrid.isEnemy = true;
            enemyGrid.index = i;
            BattleScenario.EnemyGrids.Add(enemyGrid);
            enemyGrid.SetClickEvent().SetDownEvent().SetDragEvent().SetEnterEvent().SetExitEvent().SetUpEvent();
        }
    }

    public async void LoadGame()
    {
        seed = (int)(long)progressDoc["Seed"];
        string sceneName = (string)progressDoc["Scene"];
        await LoadFriendly();
        if (sceneName == "Battle")
            await LoadEnemy();
        SceneManager.LoadScene(sceneName);
    }
    public void NewGame()
    {
        seed = Random.Range(0, int.MaxValue);
        Random.InitState(seed);
    }
    private async Task LoadFriendly()
    {
        foreach (var x in BattleScenario.FriendlyGrids)
        {
            x.gameObject.SetActive(true);
        }
        List<DocumentSnapshot> friendlyDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.uid, "Friendlies"));
        List<CharacterData> characterDataDict = new();
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
            ObjectGrid _grid = BattleScenario.FriendlyGrids[(int)(long)tempDict["Index"]];
            GameObject friendlyObject = InitCharacterObject(_grid, false, jobId);

            CharacterData characterData = new(snapShot.Id, maxHp, hp, ability, resist, speed, (int)(long)tempDict["Index"], jobId, skills);
            characterDataDict.Add(characterData);
            FriendlyScript friendlyScript = friendlyObject.AddComponent<FriendlyScript>();
            friendlyScript.InitFriendly(snapShot.Id);
            BattleScenario.friendlies.Add(friendlyScript);
        }
        CharacterManager.characterManager.SetCharacters(characterDataDict);
    }
    public static Skill LocalizeSkill(string x1)//Skill_n/n 형태의 x1을 기반으로 LoadManager에 있는 EffectForm을 가진 SkillStruct 접근해서 Effect를 가진 Skill를 리턴
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
        foreach (var x in BattleScenario.EnemyGrids)
        {
            x.gameObject.SetActive(true);
        }
        List<DocumentSnapshot> enemyDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.Uid, "Enemies"));
        foreach (DocumentSnapshot snapShot in enemyDocs)
        {
            Dictionary<string, object> tempDict = snapShot.ToDictionary();
            int index = (int)(long)tempDict["Index"];
            EnemyClass enemyClass = LoadManager.loadManager.enemyiesDict[(string)tempDict["Id"]];
            ObjectGrid grid = BattleScenario.EnemyGrids[index];
            GameObject enemyObject = InitCharacterObject(grid, true, (string)tempDict["Id"]);
            EnemyScript enemyScript = enemyObject.AddComponent<EnemyScript>();

            enemyScript.InitEnemy(enemyClass,  grid);
            BattleScenario.enemies.Add(enemyScript);
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

}