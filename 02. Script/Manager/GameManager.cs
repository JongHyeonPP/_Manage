using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnumCollection;
using StructCollection;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public Difficulty difficulty;
    public static Language language;
    private string uid;
    public string Uid
    {
        get { return uid; }
    }
    public Dictionary<string, object> userDoc;
    public Dictionary<string, object> progressDoc;
    public IScenarioMessenger messenger;
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
    public Image ImageFriendlyArea;
    public Image ImageEnemyArea;
    public List<GameObject> FriendlyGrids { get; private set; } = new();
    public List<GameObject> EnemyGrids { get; private set; } = new();
    private GameObject scenarioObject;
    public List<CharacterBase> Friendlies { get; private set; } = new();
    public List<CharacterBase> Enemies { get; private set; } = new();
    public GameObject objectHpBar;
    #endregion
    public static readonly Color[] talentColors = new Color[4]{Color.blue, Color.green, Color.yellow, Color.red};
    void Awake()//매니저 세팅은 Awake
    {
        if (!gameManager)
        {
            gameManager = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
            InitGrid();
            //Until Steam API
            //uid = "FMefxTlgP9aHsgfE0Grc";//다수
            uid = "KF5U1XMs5cy7n13dgKjF";//소수
        }
    }
    async void Start()
    {
        progressDoc = await DataManager.dataManager.GetField("Progress", Uid); //얘는 Progress에 대한 예외처리 하면서 만져야함
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

    public void LoadSceneVia(string _destination)
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Map":
                if (_destination == "Battle")
                {
                    messenger = new IScenarioMessenger.MapToBattleData(MapArea.Plains, BattleDifficulty.Normal);
                    SceneManager.LoadScene("Battle");
                }
                break;
        }
    }
    private void OnSceneLoaded(Scene _arg0, LoadSceneMode _arg1)
    {
        battleScenario = null;
        lobbyScenario = null;
        scenarioObject = GameObject.FindWithTag("SCENARIO");
        SettingManager.settingManager.uiCamera.SetActive(_arg0.name == "Battle");
        switch (_arg0.name)
        {
            case "Awake":
                SceneManager.LoadScene("Start");
                break;
            case "Battle":
                battleScenario = scenarioObject.GetComponent<BattleScenario>();
                if (messenger is IScenarioMessenger.MapToBattleData)
                {
                    IScenarioMessenger.MapToBattleData x = (IScenarioMessenger.MapToBattleData)messenger;
                    battleScenario.battleDifficulty = x.battleDifficulty;
                }
                break;
            case "Lobby":
                lobbyScenario = scenarioObject.GetComponent<LobbyScenario>();
                break;
        }
        messenger = null;
    }
    private void InitGrid()
    {
        DontDestroyOnLoad(canvasGrid);
        canvasGrid.gameObject.SetActive(false);
        ImageFriendlyArea = canvasGrid.GetChild(0).GetComponent<Image>();
        ImageFriendlyArea.enabled = false;
        ImageEnemyArea = canvasGrid.GetChild(1).GetComponent<Image>();
        ImageEnemyArea.enabled = false;
        GameObject panelFriendly = canvasGrid.GetChild(2).gameObject;
        GameObject panelEnemy = canvasGrid.GetChild(3).gameObject;

        for (int i = 0; i < 9; i++)
        {
            InitGrid(i, panelFriendly, FriendlyGrids);
            InitGrid(i, panelEnemy, EnemyGrids);
        }

        static void InitGrid(int _index, GameObject _panel, List<GameObject> _grids)
        {
            GameObject grid = _panel.transform.GetChild(_index).gameObject;
            _grids.Add(grid);
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
        foreach (GameObject x in FriendlyGrids)
        {
            x.SetActive(true);
        }
        List<DocumentSnapshot> friendlyDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.uid, "Friendlies"));
        foreach (DocumentSnapshot snapShot in friendlyDocs)
        {
            Dictionary<string, object> tempDict = snapShot.ToDictionary();
            int job = 0;
            string jobString = "0";
            int count = 0;
            List<Skill> skills = new();
            float ability = GetFloatValue(tempDict, "Ability");
            string hpValue = (string)tempDict["Hp"];
            float hp = float.Parse(hpValue.Split('/')[0]);
            float maxHp = float.Parse(hpValue.Split('/')[1]);
            float resist = GetFloatValue(tempDict, "Resist");
            for (int i = 0; i < 2; i++)
            {
                if (tempDict.TryGetValue(string.Format("Skill_{0}", i), out object valueObj))
                {
                    count++;
                    string skillID = ((string)valueObj).Split(":::")[0];
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
            jobString = AddZero(job);
            for (int i = 0; i < 2; i++)
            {
                if (tempDict.TryGetValue(string.Format("Skill_{0}", i), out object valueObj))
                {
                    Skill localizedSkill = LocalizeSkill((string)valueObj);
                    foreach(var x0 in localizedSkill.effects)
                    {
                            SkillEffect effect = x0;
                        if (jobString == "101")
                        {
                            Debug.Log("IsSelf 변수 만든 뒤 작업해야할 곳");
                        }
                    }
                    skills.Add(localizedSkill);
                }
            }
            int index = (int)(long)tempDict["Index"];
            GameObject friendlyObject = InitCharacterObject(index, false, jobString);

            
            FriendlyScript friendlyScript = friendlyObject.AddComponent<FriendlyScript>();
            List<TalentStruct> talents = new();
            if (tempDict.ContainsKey("Talent"))
            {
                foreach (object talentObj in tempDict["Talent"] as List<object>)
                {
                    string talentStr = (string)talentObj;
                    TalentFormStruct talentForm = LoadManager.loadManager.talentDict[talentStr.Split(":::")[0]];
                    List<T_Effect> effects = new();
                    string[] array = talentStr.Split(":::")[1].Split('/');
                    for (int i = 0; i < array.Length; i++)
                    {
                        effects.Add(new(float.Parse(array[i]), talentForm.effects[i].type));
                    }
                    talents.Add(new(talentForm.name, talentForm.level, talentForm.explain, effects));
                }
            }
            friendlyScript.InitFriendly(skills, talents, index, maxHp, hp, ability, resist, job);
            Friendlies.Add(friendlyScript);
        }
    }
    public static Skill LocalizeSkill(string x1)//Skill_n/n 형태의 x1을 기반으로 LoadManager에 있는 EffectForm을 가진 SkillStruct 접근해서 Effect를 가진 SkillStruct를 리턴
    {
        string skillID = x1.Split(":::")[0];
        byte skillLevel = byte.Parse(x1.Split(":::")[1]);
        SkillForm tempSkillForm = LoadManager.loadManager.skillsDict[skillID];
        return new Skill(tempSkillForm, skillLevel);
    }
    private async Task LoadEnemy()
    {
        foreach (GameObject x in EnemyGrids)
        {
            x.SetActive(true);
        }
        List<DocumentSnapshot> enemyDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.Uid, "Enemies"));
        foreach (DocumentSnapshot snapShot in enemyDocs)
        {
            Dictionary<string, object> tempDict = snapShot.ToDictionary();
            int index = (int)(long)tempDict["Index"];
            EnemyStruct enemyStruct = LoadManager.loadManager.enemyiesDict[(string)tempDict["Id"]];
            GameObject enemyObject = InitCharacterObject(index, true, (string)tempDict["Id"]);
            EnemyScript enemyScript = enemyObject.AddComponent<EnemyScript>();
            enemyScript.InitEnemy(enemyStruct.skills, index, enemyStruct.hp, enemyStruct.ability, enemyStruct.resist);
            Enemies.Add(enemyScript);
        }
    }

    public GameObject InitCharacterObject(int _index, bool _isEnemy, string _characterId)
    {
        List<GameObject> grids = _isEnemy ? EnemyGrids : FriendlyGrids;
        string type = _isEnemy ? "Enemy" : "Friendly";
        GameObject characterObject = Instantiate(Resources.Load<GameObject>(string.Format("Prefab/{0}/{0}_{1}", type, _characterId)));


        characterObject.transform.GetChild(0).localScale = Vector3.one * 70;
        GameObject tempGrid = grids[_index];
        characterObject.transform.SetParent(tempGrid.transform);
        tempGrid.GetComponent<Image>().enabled = true;
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
    public void OnEnemyGridClicked(int _girdIndex)
    {
        battleScenario.OnCharacterGridClicked(_girdIndex, true);
    }public void OnFriendlyGridClicked(int _girdIndex)
    {
        battleScenario.OnCharacterGridClicked(_girdIndex, false);
    }
    public void OnFriendlyGridPointEnter(int _gridIndex)
    {
        battleScenario.OnGridPointEnter(_gridIndex, false);
    }
    public void OnEnemyGridPointEnter(int _gridIndex)
    {
        battleScenario.OnGridPointEnter(_gridIndex, true);
    }

    public void OnFriendlyGridPointExit(int _gridIndex)
    {
        battleScenario.OnGridPointExit(_gridIndex, false);
    }
    public void OnEnemyGridPointExit(int _gridIndex)
    {
        battleScenario.OnGridPointExit(_gridIndex, true);
    }
    public static bool CalculateProbability(float _probability)
    {
        return (float)Random.Range(0, int.MaxValue) % 100 / 100 <= Mathf.Clamp(_probability, 0f, 1f);
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
            friendlyScript.InitFriendly(new List<Skill>(),_candidates[i].info.talents, i+3, _candidates[i].info.hp, _candidates[i].info.hp, _candidates[i].info.ability, _candidates[i].info.ability, 0);
            Friendlies.Add(friendlyScript);
            friendlyObject.transform.localPosition = Vector3.zero;
            friendlyObject.transform.localScale = Vector3.one;
            friendlyObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
            
        }
    }

}