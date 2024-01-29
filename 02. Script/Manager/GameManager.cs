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
    public static StartScenario startScenario;

    #region Battle
    [Header("Battle")]
    public Transform canvasGrid;

    private GameObject scenarioObject;
    public GameObject objectHpBar;
    #endregion
    public static readonly Color[] talentColors = new Color[4] { Color.blue, Color.green, Color.yellow, Color.red };
    EventTrigger eventTrigger;
    public int nodeLevel = 0;
    public string scene;
    public string history;
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
            //uid = "FMefxTlgP9aHsgfE0Grc";
            uid = "KF5U1XMs5cy7n13dgKjF";
        }
    }
    async void Start()
    {
        progressDoc = await DataManager.dataManager.GetField("Progress", Uid);
        if (progressDoc != null)
        {
            startScenario.ActiveLoadBtn(true);
        }
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
        fame = GetFloatValue(userDoc["Fame"]);
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
        startScenario = null;
        scenarioObject = GameObject.FindWithTag("SCENARIO");


        switch (_arg0.name)
        {
            case "Awake":
                SceneManager.LoadScene("Start");
                break;
            case "Start":
                startScenario = scenarioObject.GetComponent<StartScenario>();
                    startScenario.ActiveLoadBtn(false);
                break;
            case "Battle":
                battleScenario = scenarioObject.GetComponent<BattleScenario>();
                break;
            case "Lobby":
                lobbyScenario = scenarioObject.GetComponent<LobbyScenario>();
                DataManager.dataManager.SetDocumentData("Scene", "Lobby", "Progress", Uid);
                break;
            case "Stage0":
                DataManager.dataManager.SetDocumentData("Scene", "Stage0", "Progress", Uid);
                break;
        }
        //Skill currentSkill = CharacterManager.characterManager.GetCharacter(0).ChangeSkill(0, 스킬);
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
        Random.InitState(seed);
        scene = (string)progressDoc["Scene"];
        nodeLevel = (int)(long)progressDoc["NodeLevel"];
        await LoadFriendly();
        if (scene == "Battle")
            await LoadEnemy((string)progressDoc["EnemyCase"]);
        SceneManager.LoadScene(scene);
    }
    public void NewGame()
    {
        //temp
        nodeLevel = 0;
        Dictionary<string, object> dict = new();
        dict.Add("Seed", seed);
        dict.Add("NodeLevel", nodeLevel);
        FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
        {
            InitSeed();
            DataManager.dataManager.SetDocumentData(dict, "Progress", Uid);
            return Task.CompletedTask;
        });
    }
    private async Task LoadInventory()
    {
     
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
            object obj;
            float ability;
            string hpValue;
            float resist;
            float speed;
            string[] skillNames = new string[2];
            string[] weaponNames = new string[2]; //Right, Left
            if (tempDict.TryGetValue("Ability", out obj))
            {
                ability = GetFloatValue(obj);
            }
            else
            {
                ability = 0;
            }
            if (tempDict.TryGetValue("Hp", out obj))
            {
                hpValue = (string)obj;
            }
            else
            {
                hpValue = "1/1";
            }
            float hp = float.Parse(hpValue.Split('/')[0]);
            float maxHp = float.Parse(hpValue.Split('/')[1]);
            if (tempDict.TryGetValue("Resist", out obj))
            {
                resist = GetFloatValue(obj);
            }
            else
            {
                resist = 0;
            }
            if (tempDict.TryGetValue("Speed", out obj))
            {
                speed = GetFloatValue(obj);
            }
            else
            {
                speed = 1f;
            }
            for (int i = 0; i < 2; i++)
            {
                if (tempDict.TryGetValue(string.Format("Skill_{0}", i), out obj))
                {
                    skillNames[i] =((string)obj);
                }
            }
            if(tempDict.TryGetValue("Weapon_R", out obj))
            {
                weaponNames[0] = (string)obj;
            }
            if (tempDict.TryGetValue("Weapon_L", out obj))
            {
                weaponNames[1] = (string)obj;
            }
            ObjectGrid _grid = BattleScenario.FriendlyGrids[(int)(long)tempDict["Index"]];
            string jobId = GetJobId(skillNames);
            InitFriendlyObject(snapShot.Id, jobId, _grid);
            CharacterData characterData = new(snapShot.Id, jobId, maxHp, hp, ability, resist, speed, (int)(long)tempDict["Index"], skillNames, weaponNames);
            characterDataDict.Add(characterData);
        }
        CharacterManager.characterManager.SetCharacters(characterDataDict);
    }
    public string GetJobId(string[] _skillNames)
    {
        int job = 0;
        int num = 0;
        foreach (var x in _skillNames)
        {
            switch (x.Split("_")[0])
            {
                case "Power":
                    job += 100;
                    num++;
                    break;
                case "Sustain":
                    job += 10;
                    num++;
                    break;
                case "Util":
                    job += 1;
                    num++;
                    break;
            }
        }
        if (num == 2)
        {
            return AddZero(job);
        }
        else
            return "000";
    }
    public void InitFriendlyObject(string _docId,string _jobId, ObjectGrid _grid)
    {

        GameObject friendlyObject = InitCharacterObject(_grid, false, _jobId);
        FriendlyScript friendlyScript = friendlyObject.AddComponent<FriendlyScript>();
        friendlyScript.InitFriendly(_docId);
        BattleScenario.friendlies.Add(friendlyScript);
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
    private async Task LoadEnemy(string _enemyCase)
    {
        foreach (var x in BattleScenario.EnemyGrids)
        {
            x.gameObject.SetActive(true);
        }
        object enemyObj = await DataManager.dataManager.GetFieldData("Enemies","EnemyCase", _enemyCase);
        foreach (object obj in enemyObj as List<object>)
        {
            Dictionary<string, object> enemy = obj as Dictionary<string, object>;
            int index = (int)(long)enemy["Index"];
            string id = (string)enemy["Id"];
            EnemyClass enemyClass = LoadManager.loadManager.enemyiesDict[id];
            ObjectGrid grid = BattleScenario.EnemyGrids[index];
            GameObject enemyObject = InitCharacterObject(grid, true, id);
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

    float GetFloatValue(object _obj)
    {
        if (_obj is long)
            return (int)(long)_obj;
        else
            return (float)(double)_obj;
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
    public async void GameOver()
    {
        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
        {
            BattleScenario.ClearEnemy();
            BattleScenario.ClearFriendly();
            DocumentReference documentRef = FirebaseFirestore.DefaultInstance.Collection("Progress").Document(Uid);
            documentRef.DeleteAsync();
        });
        canvasGrid.gameObject.SetActive(false);
        uiCamera.SetActive(false);
        scene = null;
        progressDoc = null;

    }
    public void InitSeed()
    {
        seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(seed);
        DataManager.dataManager.SetDocumentData("Seed", seed, "Progress", Uid);
        Debug.Log("Seed : " + seed);
    }
    [ContextMenu("ChangeTest")]
    public void ChangeTest()
    {
        List<string> temp = CharacterManager.characterManager.GetCharacter(0).ChangeWeapon(0, "rninQtNgoycJzwOfBoew");
        foreach (string x in temp)
        {
            Debug.Log("id : " + x);
        }
    }
}