using BattleCollection;
using EnumCollection;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScenario : MonoBehaviour
{
    public System.Action regularEffect;
    public BattleDifficulty battleDifficulty;
    public static List<BaseInBattle> enemies;
    public static List<BaseInBattle> characters;
    public GridObject gridOnPointer;
    public bool isDragging = false;
    #region UI
    public Transform canvasBattle;
    public Transform canvasTest;
    public LootUi LootUi;
    public Transform panelGameOver;
    #endregion
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    private BattleScenarioTest battleScenarioTest;
    public RectTransform rectCharacterGroup;
    public bool isInCharacter;
    public static List<EffectType> buffOrDebuff;
    public static BattlePatern battlePatern = BattlePatern.OnReady;
    public float moveGauge;
    public Transform panelHpUi;
    public static List<GridObject> CharacterGrids { get; private set; } = new();
    public static List<GridObject> EnemyGrids { get; private set; } = new();
    public float RewardAscend = 0;
    public static readonly float gridCorrection = 20f;
    public Transform prefabSet;
    private Dictionary<BackgroundType, GameObject> backgrounds = new();
    int battleLevel;
    private async void Awake()
    {
        if (!GameManager.gameManager)
            return;
        GameManager.battleScenario = this;

        SetBattleLevel();
        if (battleSimulator)
            await GameManager.gameManager.LoadCharacter(battleLevel);
        Init_BattleSetAsync();
        Init_UiSet();
        Init_RegularEffectSet();
    }

    private void SetBattleLevel()
    {
        if (battleSimulator)
        {
            battleLevel = battleSimulator.currentLevel;

        }
        else
        {
            int i = GameManager.gameManager.nodeNum;
            int result;
            if (i <= 3)
            {
                result = 0;
            }
            else if (i <= 6)
            {
                result = 1;
            }
            else if (i <= 7)
            {
                result = 2;
            }
            else if (i <= 10)
            {
                result = 3;
            }
            else if (i <= 13)
            {
                result = 4;
            }
            else if (i <= 14)
            {
                result = 5;
            }
            else if (i <= 17)
            {
                result = 6;
            }
            else if (i <= 20)
            {
                result = 7;
            }
            else
            {
                result = 8;
            }
            battleLevel = result;
        }
    }

    private void Init_RegularEffectSet()
    {
        foreach (var x in characters)
        {
            regularEffect += x.ActiveRegularEffect;
        }
        foreach (var x in enemies)
        {
            regularEffect += x.ActiveRegularEffect;
        }
    }

    private void Init_UiSet()
    {
        GameManager.gameManager.canvasGrid.gameObject.SetActive(true);
        GameManager.gameManager.canvasGrid.GetComponent<Canvas>().worldCamera = Camera.main;
        LootUi.gameObject.SetActive(false);
        panelGameOver.gameObject.SetActive(false);
        texts =
                new()
                {
                    //{
                    //    textSkill[0],
                    //    new()
                    //    {
                    //        { Language.Ko, FocusedFriendly.skills[0].name[Language.Ko] },
                    //        { Language.En, FocusedFriendly.skills[0].name[Language.En] }
                    //    }
                    //},
                };
        SettingManager.LanguageChangeEvent += LanguageChange;
        LanguageChange();



        battleScenarioTest = GetComponent<BattleScenarioTest>();
        if (battleScenarioTest)
            canvasTest.gameObject.SetActive(true);
        rectCharacterGroup = GameManager.gameManager.canvasGrid.GetChild(0).GetComponent<RectTransform>();
        GameManager.gameManager.uiCamera.SetActive(true);


        //Stage 0
        backgrounds[BackgroundType.Plains] = prefabSet.GetChild(0).gameObject;
        backgrounds[BackgroundType.Forest] = prefabSet.GetChild(1).gameObject;
        backgrounds[BackgroundType.Ruins] = prefabSet.GetChild(2).gameObject;
        //Stage 1
        backgrounds[BackgroundType.Beach] = prefabSet.GetChild(3).gameObject;
        backgrounds[BackgroundType.Swamp] = prefabSet.GetChild(4).gameObject;
        backgrounds[BackgroundType.Cave] = prefabSet.GetChild(5).gameObject;
        //Stage 2
        backgrounds[BackgroundType.Desert] = prefabSet.GetChild(6).gameObject;
        backgrounds[BackgroundType.Lava] = prefabSet.GetChild(7).gameObject;
        backgrounds[BackgroundType.IceField] = prefabSet.GetChild(8).gameObject;

        switch (battleLevel)
        {
            default:
                ChangeMap(BackgroundType.Plains);
                break;
            case 1:
                ChangeMap(BackgroundType.Forest);
                break;
            case 2:
                ChangeMap(BackgroundType.Ruins);
                break;
            case 3:
                ChangeMap(BackgroundType.Beach);
                break;
            case 4:
                ChangeMap(BackgroundType.Swamp);
                break;
            case 5:
                ChangeMap(BackgroundType.IceField);
                break;
            case 6:
                ChangeMap(BackgroundType.Cave);
                break;
            case 7:
                ChangeMap(BackgroundType.Desert);
                break;
            case 8:
                ChangeMap(BackgroundType.Lava);
                break;
        }

    }

    private async void Init_BattleSetAsync()
    {
        if (battleSimulator)
        {
            await LoadEnemy(battleLevel);
        }
        else//Enemies 없다면 BattleCase를 통해서 생성
            if (enemies.Count == 0)
        {
            List<EnemyPiece> selectedCase = MakeEnemies(battleLevel);//적 생성

            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction => {
                for (int i = 0; i < selectedCase.Count; i++)
                {
                    Dictionary<string, object> enemyDict = new()
                    {
                        { "Id", selectedCase[i].id },
                        { "GridIndex", selectedCase[i].index }
                    };
                    DataManager.dataManager.SetDocumentData(enemyDict, $"Progress/{GameManager.gameManager.Uid}/Enemies");
                }
                DataManager.dataManager.SetDocumentData("Scene", "Battle", "Progress", GameManager.gameManager.Uid); ;
                return Task.CompletedTask;
            });
        }
        CharacterAtBattleInit();//Data->Base
        List<CharacterData> characters = CharacterManager.characterManager.GetCharacters();
        for (int i = 0; i < 3; i++)
        {
            CharacterInBattle characterAtBattle = characters[i].characterAtBattle;

        }
    }

    public void OnGridPointerDown()
    {
        GameManager.battleScenario.isDragging = true;
        GameManager.IsPaused = true;

    }
    public static List<BaseInBattle> GetTargetsByRange(EffectRange _range, BaseInBattle _target)
    {
        List<BaseInBattle> targets = null;
        List<BaseInBattle> targetsBase = (_target.IsEnemy ? enemies : characters).Where(item => !item.isDead).ToList();
        switch (_range)
        {
            case EffectRange.Dot://가장 가까운 대상
            case EffectRange.Self:
                targets = new() { _target };
                break;
            case EffectRange.Row:
                targets = targetsBase.Where(item => item.grid.index / 3 == _target.grid.index / 3).ToList();
                break;
            case EffectRange.Column:
                targets = targetsBase.Where(item => item.grid.index % 3 == _target.grid.index % 3).ToList();
                break;
            case EffectRange.Behind:
                if (_target.IsEnemy)
                    targets = targetsBase.Where(item => item.grid.index % 3 > _target.grid.index % 3).ToList();
                else
                    targets = targetsBase.Where(item => item.grid.index % 3 < _target.grid.index % 3).ToList();
                break;
            case EffectRange.Front:
                if (_target.IsEnemy)
                    targets = targetsBase.Where(item => item.grid.index % 3 < _target.grid.index % 3).ToList();
                else
                    targets = targetsBase.Where(item => item.grid.index % 3 > _target.grid.index % 3).ToList();
                break;
        }
        return targets;
    }
    public static List<GridObject> GetTargetGridsByRange(EffectRange _range, GridObject _targetGrid)
    {
        List<GridObject> targetGrids = null;
        List<GridObject> gridsBase = (_targetGrid.isEnemy ? EnemyGrids : CharacterGrids).ToList();
        switch (_range)
        {
            case EffectRange.Dot://가장 가까운 대상
            case EffectRange.Self:
                targetGrids = new() { _targetGrid };
                break;
            case EffectRange.Row:
                targetGrids = gridsBase.Where(item => item.index / 3 == _targetGrid.index / 3).ToList();
                break;
            case EffectRange.Column:
                targetGrids = gridsBase.Where(item => item.index % 3 == _targetGrid.index % 3).ToList();
                break;
            case EffectRange.Behind:
                if (_targetGrid.isEnemy)
                    targetGrids = gridsBase.Where(item => item.index % 3 > _targetGrid.index % 3).ToList();
                else
                    targetGrids = gridsBase.Where(item => item.index % 3 < _targetGrid.index % 3).ToList();
                break;
            case EffectRange.Front:
                if (_targetGrid.isEnemy)
                    targetGrids = gridsBase.Where(item => item.index % 3 < _targetGrid.index % 3).ToList();
                else
                    targetGrids = gridsBase.Where(item => item.index % 3 > _targetGrid.index % 3).ToList();
                break;
        }
        return targetGrids;
    }
    public void MoveCharacterByGrid(GridObject _startGrid, GridObject _targetGrid)
    {
        BaseInBattle targetCharacter = null;
        if (_targetGrid.owner)
            targetCharacter = _targetGrid.owner;
        _startGrid.owner.MoveToTargetGrid(_targetGrid);
        if (targetCharacter)
        {
            targetCharacter.MoveToTargetGrid(_startGrid);
        }
        else
        {
            _startGrid.owner = null;
        }
    }

    private void CharacterAtBattleInit()
    {
        List<CharacterData> characterDataList = CharacterManager.characterManager.GetCharacters();
        for (int i = 0; i < characterDataList.Count; i++)
        {
            CharacterData data = characterDataList[i];
            CharacterInBattle characterAtBattle = data.characterAtBattle;
            HpBarInUi hpBarInUI = panelHpUi.GetChild(i).GetComponent<HpBarInUi>();
            characterAtBattle.hpBarInUi = hpBarInUI;
            characterAtBattle.SynchronizeCharacterData(data);
            if (hpBarInUI)
            {
                hpBarInUI.characterHierarchy.CopyHierarchySprite(data.characterHierarchy);
                //이미지 셋도 여기서 해야하는듯
                for (int i1 = 0; i1 < 2; i1++)
                {
                    if (data.skills[i1]!=null)
                        hpBarInUI.skillObject[i1].SetActive(true);
                    else
                        hpBarInUI.skillObject[i1].SetActive(false);
                }
            }
            
        }
        battlePatern = BattlePatern.OnReady;
    }



    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameManager.IsPaused = !GameManager.IsPaused;
        }
    }
    public bool IsTargetGrid(int _i, bool _isEnemyGrid)
    {
        List<int> indexes = new();
        if (_isEnemyGrid)
        {
            foreach (EnemyInBattle x in enemies)
            {
                indexes.Add(x.grid.index);
            }
        }
        else
        {
            foreach (CharacterInBattle x in characters)
            {
                indexes.Add(x.grid.index);
            }
        }
        if (_i % 3 != 0)
        {
            foreach (int x in indexes)
            {
                if (_i % 3 == 1)
                {
                    if (x == _i - 1)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_i % 3 == 2)
                    {
                        if (x == _i - 1 || x == _i - 2)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void LanguageChange()
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[GameManager.language];
        }
    }
    public IEnumerator ActiveRegualrEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (regularEffect != null)
                regularEffect();
        }
    }

    public async Task StageClearAsync()
    {
        Debug.Log("StageClear");
        GameManager.battleScenario.StopAllCoroutines();
        battlePatern = BattlePatern.OnReady;
        
        List<CharacterData> dataList = CharacterManager.characterManager.GetCharacters();
        if (!battleSimulator)
        {
            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
            {
                foreach (CharacterData data in dataList)
                {
                    data.hp = data.characterAtBattle.Hp;
                    DataManager.dataManager.SetDocumentData("Hp", Mathf.Max(data.hp, 1), string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Characters"), data.docId);
                }
                DataManager.dataManager.SetDocumentData("Scene", "Stage 0", "Progress", GameManager.gameManager.Uid);
                return Task.CompletedTask;
            });
        }
        foreach (BaseInBattle x in characters)
        {
            x.StopBattle();
        }
        RefreshGrid();
        await ClearEnemyAsync();
        ItemManager.itemManager.SetLootAsync();
        LootUi.gameObject.SetActive(true);
    }

    private static void RefreshGrid()
    {
        foreach (GridObject grid in CharacterGrids)
        {
            grid.RefreshGrid();
        }
    }

    public void ToMap()
    {
        if (battleSimulator)
        {
            GoToBattleSimulation();
        }
        else
        {
            foreach (BaseInBattle x in characters)
            {
                x.InBattleFieldZero();
            }
            GameManager.gameManager.canvasGrid.gameObject.SetActive(false);
            SceneManager.LoadScene("Stage 0");
        }
    }
    private IEnumerator MoveGaugeCor()
    {
        moveGauge = 10f;
        while (true)
        {
            if (moveGauge < 10f)
                moveGauge += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
    public void StartBattle()
    {
        foreach (var x in enemies)
        {
            x.StartBattle();
        }
        foreach (var x in characters)
        {
            x.StartBattle();
        }
        canvasBattle.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(ActiveRegualrEffect());
        battlePatern = BattlePatern.Battle;
        StartCoroutine(MoveGaugeCor());
    }
    private List<EnemyPiece> MakeEnemies(int _nodeLevel)
    {
        Dictionary<string, EnemyCase> values = LoadManager.loadManager.enemyCaseDict;
        List<string> ableCases = values.Where(item => item.Value.levelRange.Contains(_nodeLevel))
                              .Select(item => item.Key)
                              .ToList();
        string selectedCase = ableCases[Random.Range(0, ableCases.Count)];
        List<EnemyPiece> enemyPieces = LoadEnemiesByCase(selectedCase);
        return enemyPieces;
    }
    public async Task ClearEnemyAsync()
    {
        ClearEnemyObject();
        List<DocumentSnapshot> snapshots = await DataManager.dataManager.GetDocumentSnapshots($"Progress/{GameManager.gameManager.Uid}/Enemies");
        foreach (DocumentSnapshot snapshot in snapshots)
        {
            await snapshot.Reference.DeleteAsync();
        }
    }

    private void ClearEnemyObject()
    {
        foreach (var x in enemies)
        {
            Destroy(x.gameObject);
        }
        enemies.Clear();
    }

    public async Task ClearCharacterAsync()
    {
        ClearCharacterObject();
        string collectionRef = "Progress/" + GameManager.gameManager.Uid + "/Characters";
        List<DocumentSnapshot> result = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Characters"));
        foreach (DocumentSnapshot doc in result)
        {
            await doc.Reference.DeleteAsync();
        }
    }

    private void ClearCharacterObject()
    {
        foreach (var x in characters)
        {
            Destroy(x.gameObject);
        }
        characters.Clear();
    }

    public void GoToStart() => SceneManager.LoadScene("Start");
    public void CreateVisualEffect(VisualEffect _visualEffect, BaseInBattle _character, bool _isSkillVe)
    {
        Transform target;
        target = _visualEffect.fromRoot ? _character.rootTargetTransform : _character.skillTargetTransform;
        GameObject effectObj = Instantiate(_visualEffect.effectObject, target);
        if (_isSkillVe && !_visualEffect.fromRoot)
        {
            int rangeX = UnityEngine.Random.Range(-3, 3);
            int rangeY = UnityEngine.Random.Range(-3, 3);
            effectObj.transform.position += new Vector3(rangeX, rangeY);
        }
        if (_visualEffect.sound != string.Empty)
        {
            SoundManager.soundManager.SfxPlay(_visualEffect.sound);
        }
        Destroy(effectObj, _visualEffect.duration);
    }
    public void ChangeMap(BackgroundType _backgroundType)
    {
        foreach (GameObject backgroundObj in backgrounds.Values)
        {
            backgroundObj.SetActive(false);
        }
        backgrounds[_backgroundType].SetActive(true);
    }
    [ContextMenu("ChangeTest")]
    public void ChangeTest()
    {
        BackgroundType[] enumValues = (BackgroundType[])System.Enum.GetValues(typeof(BackgroundType));
        ChangeMap(enumValues[UnityEngine.Random.Range(0, enumValues.Length)]);
    }
    public string visualEffectStr;
    public float visualEffectDur;
    public static BattleSimulator battleSimulator;

    [ContextMenu("VisualEffetTest")]
    public void VisualEffectTest()
    {
        List<BaseInBattle> temp = new(enemies);
        temp.AddRange(characters);
        foreach (BaseInBattle x in temp)
        {
            Transform target;
            if (LoadManager.loadManager.skillVisualEffectDict[visualEffectStr].fromRoot)
            {
                target = x.rootTargetTransform;
            }
            else
            {
                target = x.skillTargetTransform;
            }
            GameObject effectObj = Instantiate(LoadManager.loadManager.skillVisualEffectDict[visualEffectStr].effectObject, target);
            int rangeX = UnityEngine.Random.Range(-2, 2);
            int rangeY = UnityEngine.Random.Range(-2, 2);
            if (!LoadManager.loadManager.skillVisualEffectDict[visualEffectStr].fromRoot)
                effectObj.transform.position += new Vector3(rangeX, rangeY);
            if (LoadManager.loadManager.skillVisualEffectDict[visualEffectStr].sound != string.Empty)
            {
                SoundManager.soundManager.SfxPlay(LoadManager.loadManager.skillVisualEffectDict[visualEffectStr].sound);
            }
            if (visualEffectDur == 0)
                Destroy(effectObj, LoadManager.loadManager.skillVisualEffectDict[visualEffectStr].duration);
            else
                Destroy(effectObj, visualEffectDur);
        }
    }
    public List<EnemyPiece> LoadEnemiesByCase(string _caseStr)
    {
        List<EnemyPiece> enemyPieces = new();
        //foreach (var x in BattleScenario.EnemyGrids)
        //{
        //    x.gameObject.SetActive(true);
        //}
        EnemyCase enemyCase = LoadManager.loadManager.enemyCaseDict[_caseStr];
        Dictionary<string, EnemyClass> enemyDict = LoadManager.loadManager.enemyiesDict;
        foreach (EnemyPieceForm pieceForm in enemyCase.pieces)
        {
            string id;
            if (pieceForm.id != null)
            {
                id = pieceForm.id;
            }
            else if (pieceForm.type != null)
            {
                List<KeyValuePair<string, EnemyClass>> values = enemyDict.ToList();
                List<KeyValuePair<string, EnemyClass>> typeValues = values.Where(item => item.Value.type == pieceForm.type).ToList();
                id = typeValues[Random.Range(0, typeValues.Count)].Key;
            }
            else
            {
                List<KeyValuePair<string, EnemyClass>> values = enemyDict.ToList();
                List<KeyValuePair<string, EnemyClass>> typeValues = values.Where(item => item.Value.enemyLevel == pieceForm.enemyLevel).ToList();
                id = typeValues[Random.Range(0, typeValues.Count)].Key;
            }
            GridObject grid = EnemyGrids[pieceForm.index];
            GameObject enemyObject;
            EnemyClass enemyClass = enemyDict[id];
            enemyObject = GameManager.gameManager.GetEnemyPrefab(id, enemyClass.isMonster);

            EnemyInBattle enemyScript = enemyObject.AddComponent<EnemyInBattle>();
            enemyScript.InitEnemy(enemyClass, grid, enemyClass.isMonster);
            enemies.Add(enemyScript);
            enemyPieces.Add(new(id, pieceForm.index));
        }
        return enemyPieces;
    }
    public static async Task LoadEnemy(int _simulationLevel = -1)
    {
        Dictionary<string, EnemyClass> enemyDict = LoadManager.loadManager.enemyiesDict;
        List<DocumentSnapshot> snapshots;
        if (_simulationLevel > -1)
            snapshots = await DataManager.dataManager.GetDocumentSnapshots($"Simulation/Simulation_{_simulationLevel}/Enemies");
        else
            snapshots = await DataManager.dataManager.GetDocumentSnapshots($"Progress/{GameManager.gameManager.Uid}/Enemies");
        foreach (DocumentSnapshot snapshot in snapshots)
        {
            Dictionary<string, object> dict = snapshot.ToDictionary();
            string id = (string)dict["Id"];
            int gridIndex = (int)(long)dict["GridIndex"];

            GridObject grid = EnemyGrids[gridIndex];
            GameObject enemyObject;
            EnemyClass enemyClass = enemyDict[id];
            enemyObject = GameManager.gameManager.GetEnemyPrefab(id, enemyClass.isMonster);

            EnemyInBattle enemyScript = enemyObject.AddComponent<EnemyInBattle>();
            enemyScript.InitEnemy(enemyClass, grid, enemyClass.isMonster);
            enemies.Add(enemyScript);
        }
    }
    public void GoToBattleSimulation()
    {
        ClearEnemyObject();
        ClearCharacterObject();
        RefreshGrid();
        SceneManager.LoadScene("BattleSimulation");
    }
}