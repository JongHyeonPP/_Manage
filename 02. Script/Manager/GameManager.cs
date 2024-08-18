using BattleCollection;
using DefaultCollection;
using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using LobbyCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public Camera uiCamera;
    //public Difficulty difficulty;
    public static Language language;
    private string uid;
    public string Uid
    {
        get { return uid; }
    }
    public Dictionary<string, object> userDoc;
    public Dictionary<string, object> progressDoc;
    public int fame;
    public float gold;
    public float fameAscend = 0f;
    public float goldAscend = 0f;
    public Dictionary<string, int> upgradeLevelDict = new();//DocId : Level
    public Dictionary<UpgradeEffectType, float> upgradeValueDict = new();
    private static bool isPaused = false;
    public static bool IsPaused
    {
        get { return isPaused; }
        set { Time.timeScale = value ? 0 : 1; isPaused = value; }
    }
    public static BattleScenario battleScenario;
    public static LobbyScenario lobbyScenario;
    public static StartScenario startScenario;
    public static StageScenarioBase mapScenario;
    public static StoreScenario storeScenario;

    public GameObject CharacterTemplate;
    #region CanvasGrid
    public Transform canvasGrid;
    public GameObject prefabGridObject;
    public GameObject prefabHpBarInScene;
    public GameObject prefabFire0;
    public GameObject gridPre;
    [HideInInspector] public RectTransform parentCharacter;
    #endregion
    EventTrigger eventTrigger;
    public int stage;
    public string scene;
    public string history;
    public List<CharacterData> characterList;
    #region Map
    public GameObject smallDotPrefab;
    public GameObject[] stageBaseCanvases;
    #endregion
    //Menu
    public GameObject buttonSetting;
    public GameObject buttonInventory;
    public TMP_Text textGold;
    public TMP_Text textFame;
    public PopUpUi popupUi;
    public GameObject uiRaycastBlock;

    //Score
    public int enemyNum;
    public int destinationNum;
    public int bossNum;
    public int foodNum;
    public GameObject showDamagePrefab;

    void Awake()//매니저 세팅은 Awake
    {
        if (!gameManager)
        {
            gameManager = this;
            //Until Steam API
            uid = "Xp1RrEgUPPIK7kwziyOA";
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(uiCamera);
            DontDestroyOnLoad(canvasGrid);
            InitGrids();
            uiCamera.gameObject.SetActive(true);
            buttonInventory.SetActive(true);
            popupUi.gameObject.SetActive(false);
            uiRaycastBlock.SetActive(false);
            DontDestroyOnLoad(GameObject.FindWithTag("CANVASGROUP"));
        }
    }
    public async Task LoadProgressDoc()
    {
        progressDoc = await DataManager.dataManager.GetField("Progress", Uid);
    }
    public async Task LoadUserDoc()
    {
        userDoc = await DataManager.dataManager.GetField("User", Uid);
        Dictionary<string, object> upgradeDict;
        if (userDoc.TryGetValue("Upgrade", out object upgradeObj))
        {
            upgradeDict = upgradeObj as Dictionary<string, object>;
        }
        else
        {
            upgradeDict = new();
        }
        fame = (int)(long)userDoc["Fame"];
        bool needToSet = false;



        foreach (KeyValuePair<string, UpgradeClass> kvp in LoadManager.loadManager.upgradeDict)
        {
            if (upgradeDict.TryGetValue(kvp.Key, out object userObj))
            {
                int level = (int)(long)userObj;
                
                upgradeLevelDict.Add(kvp.Key, level);
                UpgradeEffectType type = kvp.Value.type;
                if (level == -1)
                {
                    upgradeValueDict[type] = 0f;
                }
                else
                {
                    upgradeValueDict[type] = kvp.Value.content[level].value;
                }
            }
            else
            {
                needToSet = true;
                upgradeLevelDict.Add(kvp.Key, -1);
            }
        }
        if (needToSet)
        {
            DataManager.dataManager.SetDocumentData("Upgrade", upgradeLevelDict, "User", Uid);
        }

    }
    private void OnSceneLoaded(Scene _arg0, LoadSceneMode _arg1)
    {
        Dictionary<string, object> docDict = new();
        if (_arg0.name != "Awake" && _arg0.name != "Start")
        {
            var mainCameraData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
            var uiCameraData = uiCamera.GetComponent<UniversalAdditionalCameraData>();
            if (mainCameraData != null && uiCameraData != null)
            {
                // Overlay 카메라의 Render Type을 Overlay로 설정
                uiCameraData.renderType = CameraRenderType.Overlay;

                // Base 카메라의 Stack에 Overlay 카메라 추가
                mainCameraData.cameraStack.Add(uiCamera);
            }
        }
        if (_arg0.name != "Awake" && _arg0.name != "Start" && _arg0.name != "Loading"&& _arg0.name != "Lobby")
        {
            if (_arg0.name.Contains("Stage"))
                docDict.Add("Scene", "Stage");
            else
                docDict.Add("Scene", _arg0.name);
            DataManager.dataManager.SetDocumentData(docDict, "Progress", Uid);
        }
        if (_arg0.name != "Awake" && _arg0.name != "Start" && _arg0.name != "Lobby" && _arg0.name != "Battle")
        {
            buttonInventory.SetActive(true);
            textGold.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            buttonInventory.SetActive(false);
            textGold.transform.parent.gameObject.SetActive(false);
        }
        if (_arg0.name == "Lobby")
        {
            textFame.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            textFame.transform.parent.gameObject.SetActive(false);
        }
        if (_arg0.name == "Battle" || _arg0.name == "Store")
            StageScenarioBase.state = StateInMap.NeedPhase;
        if (StageScenarioBase.stageBaseCanvas)
            StageScenarioBase.stageBaseCanvas.gameObject.SetActive(_arg0.name.Contains("Stage"));
       buttonSetting.SetActive(_arg0.name.Contains("Stage") || _arg0.name == "Battle" || _arg0.name == "Store" || _arg0.name == "Lobby");
    }
    private void InitGrids()
    {
        canvasGrid.gameObject.SetActive(false);
        parentCharacter = canvasGrid.GetChild(1).GetComponent<RectTransform>();

        GridLayoutGroup groupFrinedly = parentCharacter.GetComponent<GridLayoutGroup>();
        parentCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(groupFrinedly.cellSize.x * 3 + BattleScenario.gridCorrection, groupFrinedly.cellSize.y * 3 + BattleScenario.gridCorrection);

        Transform panelEnemy = canvasGrid.GetChild(2);

        for (int i = 0; i < 9; i++)
        {
            GridObject characterGrid = parentCharacter.GetChild(i).GetComponent<GridObject>();
            characterGrid.InitObject();
            characterGrid.isEnemy = false;
            characterGrid.index = i;
            BattleScenario.CharacterGrids.Add(characterGrid);
            GameObject prefabPre = Instantiate(gridPre, characterGrid.transform);
            prefabPre.SetActive(true);
            characterGrid.imagePre = prefabPre.GetComponent<Image>();
            characterGrid.imagePre.enabled = false;

            GridObject enemyGrid = panelEnemy.GetChild(i).GetComponent<GridObject>();
            enemyGrid.InitObject();
            enemyGrid.isEnemy = true;
            enemyGrid.index = i;
            BattleScenario.EnemyGrids.Add(enemyGrid);
        }
    }

    public async void LoadGame()
    {
        scene = (string)progressDoc["Scene"];
        if (scene != "Lobby")
        {
            float gold = GetFloatValue(progressDoc["Gold"]);
            SetGold(gold);
            await LoadCharacter();
            if (progressDoc.ContainsKey("Inventory"))
                ItemManager.itemManager.LoadInventory(progressDoc["Inventory"]);
            if (progressDoc.ContainsKey("Nodes"))
                StageScenarioBase.nodes = (List<object>)progressDoc["Nodes"];
            else
                StageScenarioBase.nodes = new();
            if (progressDoc.ContainsKey("NodeTypes"))
                StageScenarioBase.nodeTypes = ((List<object>)progressDoc["NodeTypes"]).Select(item => item?.ToString()).ToArray();
            
            StageScenarioBase.stageNum = (int)(long)progressDoc["StageNum"];
            //Score
            enemyNum = (int)(long)progressDoc["EnemyNum"];
            destinationNum = (int)(long)progressDoc["DestinationNum"];
            bossNum = (int)(long)progressDoc["BossNum"];
            foodNum = (int)(long)progressDoc["FoodNum"];
            ItemManager.itemManager.LoadEquip();
            switch (scene)
            {
                case "Battle":
                    await BattleScenario.LoadEnemy();
                    break;
            }
            StageBaseCanvas canvas = StageScenarioBase.MakeCanvas(StageScenarioBase.stageNum);
            if (scene == "Stage")
            {
                scene += StageScenarioBase.stageNum;
                canvas.gameObject.SetActive(true);
            }
            else
            {
                canvas.gameObject.SetActive(false);
            }
        }
        LoadingScenario.LoadScene(scene);
    }

    public void SetGold(float _gold)
    {
        gold += _gold;
        textGold.text = gold.ToString("F0");
    }

    public void InitProgress()
    {
        //temp
        stage = 0;
        gold = 0f;
        StageScenarioBase.nodes = new();
        Dictionary<string, object> dict = new()
        {
            { "Nodes", StageScenarioBase.nodes },
            { "Gold", gold },
            { "Scene", "Stage" },
            { "Inventory", new object[24] },
            {"NodeTypes", StageScenarioBase.nodeTypes },
            {"StageNum", 0 },
            //Score
            {"EnemyNum", 0  },
            {"DestinationNum", 0  },
            {"BossNum", 0  },
            {"FoodNum", 0  }
        };
        DataManager.dataManager.SetDocumentData(dict, "Progress", Uid);
    }

    public async Task LoadCharacter(int _battleLevel = -1)
    {
        foreach (var x in BattleScenario.CharacterGrids)
        {
            x.gameObject.SetActive(true);
        }
        List<DocumentSnapshot> characterDocs;
        if (_battleLevel == -1)
            characterDocs = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", gameManager.uid, "Characters"));
        else
            characterDocs = await DataManager.dataManager.GetDocumentSnapshots($"Simulation/Simulation_{_battleLevel}/Characters");
        if (characterDocs.Count == 0)
        {
            Debug.LogError("No Characters");
        }
        List<CharacterData> dataList = new() { null, null, null };
        List<BaseInBattle> inBattleList = new() { null, null, null };
        for (int i = 0; i < characterDocs.Count; i++)
        {
            DocumentSnapshot snapShot = characterDocs[i];
            Dictionary<string, object> tempDict = snapShot.ToDictionary();
            object obj;
            float ability;
            float maxHp;
            float hp;
            float resist;
            float speed;
            SkillAsItem[] skills = new SkillAsItem[2];
            int[] exp = new int[2];
            WeaponClass weapon;
            List<TalentClass> talents = new();

            Sprite hair = null,
            faceHair = null,
            eyesFront = null,
            eyesBack = null,
            head = null,
            armL = null,
            armR = null;
            Color hairColor = Color.black;
            Color eyeColor = Color.black;
            if (tempDict.TryGetValue("Ability", out obj))
            {
                ability = GetFloatValue(obj);
            }
            else
            {
                ability = 0;
            }
            if (tempDict.TryGetValue("MaxHp", out obj))
            {
                maxHp = GetFloatValue(obj);
            }
            else
            {
                maxHp = 10000f;
            }
            if (tempDict.TryGetValue("Hp", out obj))
            {
                hp = GetFloatValue(obj);
            }
            else
            {
                hp = 10000f;
            }
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
            for (int i0 = 0; i0 < 2; i0++)
            {
                if (tempDict.TryGetValue(string.Format("Skill_{0}", i0), out obj))
                {
                    string skillId = (string)obj;
                    if (string.IsNullOrEmpty(skillId))
                        continue;
                    string[] splittedId = skillId.Split(":::");
                    Skill skill = LoadManager.loadManager.skillsDict[splittedId[0]];
                    int level;
                    switch (splittedId[1])
                    {
                        default:
                            level = 0;
                            break;
                        case "Rare":
                            level = 1;
                            break;
                        case "Unique":
                            level = 2;
                            break;

                    }
                    skills[i0] = skill.GetAsItem(level);
                }
            }
            if (tempDict.TryGetValue("Exp", out obj))
            {
                List<object> list = (List<object>)obj;
                for (int i1 = 0; i1 < list.Count; i1++)
                {
                  exp[i1] = (int)(long)list[i1];
                }
            }
            if (tempDict.TryGetValue("Body", out obj))
            {
                object obj1;
                Species species = Species.Human;
                Dictionary<string, object> bodyDict = obj as Dictionary<string, object>;
                if (bodyDict.TryGetValue("Species", out obj1))
                {
                    switch ((string)obj1)
                    {
                        case "Elf":
                            species = Species.Elf;
                            break;
                        case "Devil":
                            species = Species.Devil;
                            break;
                        case "Skelton":
                            species = Species.Skelton;
                            break;
                        case "Orc":
                            species = Species.Orc;
                            break;
                    }
                }
                if (bodyDict.TryGetValue("Hair", out obj1))
                {
                    string hairStr = (string)obj1;
                    if (hairStr == string.Empty)
                    {
                        hair = null;
                    }
                    else
                        hair = LoadManager.loadManager.hairDict[hairStr];
                }
                if (bodyDict.TryGetValue("FaceHair", out obj1))
                {
                    string faceHairStr = (string)obj1;
                    if (faceHairStr == string.Empty)
                    {
                        faceHair = null;
                    }
                    else
                        faceHair = LoadManager.loadManager.faceHairDict[faceHairStr];
                }
                if (bodyDict.TryGetValue("Eye", out obj1))
                {
                    EyeClass eye = LoadManager.loadManager.eyeDict[species][(string)obj1];
                    eyesFront = eye.front;
                    eyesBack = eye.back;
                }
                if (bodyDict.TryGetValue("Body", out obj1))
                {
                    BodyPartClass bodyPart = LoadManager.loadManager.BodyPartDict[species][(string)obj1];
                    head = bodyPart.head;
                    armL = bodyPart.armL;
                    armR = bodyPart.armR;
                }
                if (bodyDict.TryGetValue("HairColor", out obj1))
                {
                    Dictionary<string, object> colorDict = obj1 as Dictionary<string, object>;
                    float red = GetFloatValue(colorDict["R"]);
                    float green = GetFloatValue(colorDict["G"]);
                    float blue = GetFloatValue(colorDict["B"]);
                    hairColor = new Color(red, green, blue);
                }
                if (bodyDict.TryGetValue("EyeColor", out obj1))
                {
                    Dictionary<string, object> colorDict = obj1 as Dictionary<string, object>;
                    float red = GetFloatValue(colorDict["R"]);
                    float green = GetFloatValue(colorDict["G"]);
                    float blue = GetFloatValue(colorDict["B"]);
                    eyeColor = new Color(red, green, blue);
                }
            }
            if (tempDict.TryGetValue("WeaponId", out obj))
            {
                WeaponType weaponType;
                string weaponId = (string)obj;
                string weaponName;
                string[] splittedStr = weaponId.Split(":::");
                switch (splittedStr[0])
                {
                    default:
                        weaponType = WeaponType.Sword;
                        break;
                    case "Bow":
                        weaponType = WeaponType.Bow;
                        break;
                    case "Magic":
                        weaponType = WeaponType.Magic;
                        break;
                    case "Club":
                        weaponType = WeaponType.Club;
                        break;
                }
                weaponName = splittedStr[1];
                 weapon = LoadManager.loadManager.weaponDict[weaponType][weaponName];
                hp += weapon.hp;
                ability += weapon.ability;
                speed += weapon.speed;
                resist += weapon.resist;
            }
            else
            {
                weapon = null;
            }
            int gridIndex = (int)(long)tempDict["GridIndex"];
            int characterIndex = -1;
            if (tempDict.ContainsKey("CharacterIndex"))
                characterIndex = (int)(long)tempDict["CharacterIndex"];
            GridObject _grid = BattleScenario.CharacterGrids[gridIndex];
            string jobId = (string)tempDict["JobId"];
            //Talents
            foreach (object x in (List<object>)tempDict["Talent"])
            {
                string str = (string)x;
                string[] splittedStr = (str).Split(":::");
                string talentId = splittedStr[0];
                int effectLevel = int.Parse(splittedStr[1]);
                talents.Add(LoadManager.loadManager.talentDict[talentId].SetEffectLevel(effectLevel));
            }

            GameObject characterObject = Instantiate(CharacterTemplate);
            //CharacterHierarchy
            CharacterHierarchy characterHierarchy = characterObject.transform.GetChild(0).GetComponent<CharacterHierarchy>();
            characterHierarchy.SetBodySprite(hair, faceHair, eyesFront, eyesBack, head, armL, armR, weapon.sprite, hairColor, eyeColor);

            //CharacterData
            CharacterData data = characterObject.AddComponent<CharacterData>();
            data.InitCharacterData(snapShot.Id, jobId, maxHp, hp, ability, resist, speed, gridIndex, skills, exp, weapon, talents);
            if (characterIndex != -1)
                dataList[characterIndex] = data;
            else
                dataList[i] = data;
            
            CharacterInBattle characterAtBattle = characterObject.AddComponent<CharacterInBattle>();
            characterAtBattle.InitCharacter(data, _grid);
            inBattleList[characterIndex] = characterAtBattle;
            characterObject.name = "Character_" + characterIndex;
        }
        BattleScenario.characters = inBattleList;
        characterList = dataList;
    }

    public JobClass GetJob(string _id0,string _id1)
    {
        int job = 0;
        int num = 0;
        SkillCategori[] categories = new SkillCategori[2] { LoadManager.loadManager.skillsDict[_id0].categori, LoadManager.loadManager.skillsDict[_id1].categori }; 
        foreach (var categori in categories)
        {
            switch (categori)
            {
                case SkillCategori.Power:
                    job += 100;
                    num++;
                    break;
                case SkillCategori.Sustain:
                    job += 10;
                    num++;
                    break;
                case SkillCategori.Util:
                    job += 1;
                    num++;
                    break;
            }
        }
        string jobId = AddZero(job);
        return LoadManager.loadManager.jobsDict[jobId];
    }

    public GameObject GetEnemyPrefab(string _characterId)
    {
        GameObject enemyObject = Instantiate(Resources.Load<GameObject>(string.Format("Prefab/Enemy/" + _characterId)));

            Transform body = enemyObject.transform.GetChild(0);
            body.localScale = Vector3.one * 60f;
            var sortingGroup = body.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingOrder = 0;
            Vector3 curRot = body.eulerAngles;
            curRot.y = 180f;
            body.eulerAngles = curRot;

        return enemyObject;
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
    public static int AllocateProbability(params float[] probabilities)
    {
        // 확률의 총합 계산
        float totalProbability = 0;
        foreach (float probability in probabilities)
        {
            totalProbability += probability;
        }

        // 확률의 총합이 1이 아니면 에러 발생
        if (System.Math.Abs(totalProbability - 1f) > 0.00001f)
        {
            throw new System.ArgumentException("확률의 합이 1이 아닙니다.");
        }

        // 0과 1 사이의 랜덤 값을 생성
        float randomValue = Random.Range(0f, 1f);

        // 누적 확률을 사용하여 결과를 결정
        float cumulativeProbability = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability)
            {
                return i; // 해당 결과를 반환
            }
        }

        // 여기에 도달하는 경우는 없지만, 컴파일러의 경고를 제거하기 위해 기본적으로 0을 반환
        return 0;
    }

    public void GameOver()
    { 
        scene = null;
        progressDoc = null;
    }
    public void SetPopUp(string _content, string _emphasizeStr = "")
    {
        StartCoroutine(popupUi.SetContent(_content, _emphasizeStr));
    }
    public void OnSettingButtonClick()
    {
        SettingUi settingUi = SettingManager.settingManager.settingUi;
        if (settingUi.gameObject.activeSelf)
        {
            settingUi.OnCancelBtnClick();
        }
        else
        {
            settingUi.gameObject.SetActive(true);
        }
    }
}