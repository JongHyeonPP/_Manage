using BattleCollection;
using EnumCollection;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScenario : MonoBehaviour
{
    public System.Action regularEffect;
    public BattleDifficulty battleDifficulty;
    public static List<BaseInBattle> enemies = new();
    public static List<BaseInBattle> characters { get; set; } = new();
    public GridObject gridOnPointer;
    public bool isDragging = false;
    #region UI
    public Transform canvasBattle;
    public Transform canvasTest;
    public GameObject canvasClear;
    public LootUi lootUi;
    public GameObject buttonNext;
    public CanvasGameOver canvasGameOver;
    #endregion
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    private BattleScenarioTest battleScenarioTest;
    public RectTransform rectCharacterGroup;
    public static List<EffectType> buffOrDebuff;
    public static BattlePatern battlePatern;
    public float moveGauge;
    public Transform panelHpUi;
    public static List<GridObject> CharacterGrids { get; private set; } = new();
    public static List<GridObject> EnemyGrids { get; private set; } = new();
    public float RewardAscend = 0f;
    public static readonly float gridCorrection = 20f;
    public Transform prefabSet;
    private Dictionary<BackgroundType, GameObject> backgrounds = new();
    public static BackgroundType currentBackground;
    public ItemTooltip battleTooltip;
    public string visualEffectStr;
    public float visualEffectDur;
    public StatusExplain_Battle statusExplain;
    public TMP_Text damagePrefab;

    public static float battleProgress = 0f;
    private void Awake()
    {
        if (!GameManager.gameManager)
            return;
        GameManager.battleScenario = this;
        lootUi.InitLootUi();
        Init_UiSet();
        Init_RegularEffectSet();
        for (int i = 0; i < 3; i++)
        {
            CharacterData data = GameManager.gameManager.characterList[i];
            HpBarInUi hpBarInUI = panelHpUi.GetChild(i).GetComponent<HpBarInUi>();
            if (data != null)
            {
                BaseInBattle characterAtBattle = characters[i];
                characterAtBattle.hpBarInUi = hpBarInUI;

                if (hpBarInUI)
                {
                    hpBarInUI.InitHpBarInUi(data.skillAsItems, data.characterHierarchy);
                    //hpBarInUI.SetHp(characterAtBattle.Hp, characterAtBattle.armor, characterAtBattle.maxHp);
                    characterAtBattle.Hp = characterAtBattle.Hp;
                }
            }
            else
            {
                hpBarInUI.gameObject.SetActive(false);
            }
        }
    }



    private void Init_RegularEffectSet()
    {
        foreach (var x in characters)
        {
            if (x)
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

        canvasGameOver.gameObject.SetActive(false);
        canvasBattle.gameObject.SetActive(true);
        canvasClear.gameObject.SetActive(false);
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
        GameManager.gameManager.uiCamera.gameObject.SetActive(true);


        //Stage 0
        backgrounds[BackgroundType.Plains] = prefabSet.GetChild(0).gameObject;
        backgrounds[BackgroundType.Forest] = prefabSet.GetChild(1).gameObject;
        backgrounds[BackgroundType.Beach] = prefabSet.GetChild(2).gameObject;
        backgrounds[BackgroundType.Ruins] = prefabSet.GetChild(3).gameObject;
        backgrounds[BackgroundType.ElfCity] = prefabSet.GetChild(4).gameObject;
        
        backgrounds[BackgroundType.MysteriousForest] = prefabSet.GetChild(5).gameObject;
        backgrounds[BackgroundType.VineForest] = prefabSet.GetChild(6).gameObject;
        backgrounds[BackgroundType.Swamp] = prefabSet.GetChild(7).gameObject;
        backgrounds[BackgroundType.WinterForest] = prefabSet.GetChild(8).gameObject;
        backgrounds[BackgroundType.IceField] = prefabSet.GetChild(9).gameObject;
        
        backgrounds[BackgroundType.DesertRuins] = prefabSet.GetChild(10).gameObject;
        backgrounds[BackgroundType.Cave] = prefabSet.GetChild(11).gameObject;
        backgrounds[BackgroundType.Desert] = prefabSet.GetChild(12).gameObject;
        backgrounds[BackgroundType.RedRock] = prefabSet.GetChild(13).gameObject;
        backgrounds[BackgroundType.Lava] = prefabSet.GetChild(14).gameObject;

        ChangeBackground(StageScenarioBase.stageBaseCanvas.currentNode.nodeType.backgroundType);
        battleTooltip.gameObject.SetActive(false);
        statusExplain.gameObject.SetActive(false);
    }

    public static async Task Init_BattleSetAsync()
    {
        float totalSteps = 5; // 총 단계 수
        battleProgress = 0f;

        battlePatern = BattlePatern.OnReady;
        if (enemies.Count == 0)
        {
            List<EnemyPiece> selectedCase = MakeEnemies(); // 적 생성

            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async Transaction =>
            {
                for (int i = 0; i < selectedCase.Count; i++)
                {
                    Dictionary<string, object> enemyDict = new()
                    {
                        { "Id", selectedCase[i].id },
                        { "GridIndex", selectedCase[i].index }
                    };
                    DataManager.dataManager.SetDocumentData(enemyDict, $"Progress/{GameManager.gameManager.Uid}/Enemies");
                    battleProgress += 1f / totalSteps; // 진행률 업데이트
                }
                return Task.CompletedTask;
            });
        }
        battleProgress = 4 / 5; // 진행률 업데이트
        // 캐릭터 초기화
        CharacterAtBattleInit(); // Data -> Base

        battleProgress = 1f; // 진행률 업데이트
        Debug.Log("Battle Load Complete");
    }


    public static List<BaseInBattle> GetTargetsByRange(EffectRange _range, BaseInBattle _target)
    {
        List<BaseInBattle> targets = null;
        List<BaseInBattle> targetsBase = (_target.IsEnemy ? enemies : characters).Where(item => item&& !item.isDead).ToList();
        switch (_range)
        {
            case EffectRange.Dot://가장 가까운 대상
            case EffectRange.Self:
                targets = new List<BaseInBattle>() { _target };
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
            case EffectRange.TargetsAllies:
                targets = targetsBase;
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
        _targetGrid.owner = _startGrid.owner;
        _targetGrid.owner.grid = _targetGrid;
        if (targetCharacter)
        {
            targetCharacter.MoveToTargetGrid(_startGrid);
            _startGrid.owner = targetCharacter;
            targetCharacter.grid = _startGrid;
        }
        else
        {
            _startGrid.owner = null;
        }
        TargetRefreshAll();
    }
    public void TargetRefreshAll()
    {
        List<BaseInBattle> enemiesBase = enemies.Where(item => item && !item.isDead).ToList();
        List<BaseInBattle> charactersBase = characters.Where(item => item && !item.isDead).ToList();
        foreach (BaseInBattle x in enemiesBase)
        {
            x.FindNewTargetAlly();
            x.FindNewTargetOpponent();
        }
        foreach (BaseInBattle x in charactersBase)
        {
            x.FindNewTargetAlly();
            x.FindNewTargetOpponent();
        }
        PassiveReconnect();
    }
    private  static void CharacterAtBattleInit()
    {
        List<CharacterData> characterDataList = GameManager.gameManager.characterList;
        for (int i = 0; i < characterDataList.Count; i++)
        {
            CharacterData data = characterDataList[i];
            if (data == null)
                continue;
            CharacterInBattle characterAtBattle = data.characterAtBattle;
            characterAtBattle.SynchronizeCharacterData(data);   
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
    public IEnumerator StageClearCoroutine()
    {
        RefreshGrid();
        foreach (BaseInBattle x in characters)
        {
            if (x)
                x.StopBattle();
        }

        yield return new WaitForSeconds(2f);
        StageClearAsync();

    }
    public async Task StageClearAsync()
    {
        Debug.Log("StageClear");
        GameManager.battleScenario.StopAllCoroutines();
        foreach (var x in characters)
        {
            if (x)
                x.StopAllCoroutines();
        }
        foreach (var x in enemies)
        {
            x.StopAllCoroutines();
        }
        battlePatern = BattlePatern.OnReady;
        StageScenarioBase.nodes.Add(null);
        List<CharacterData> dataList = GameManager.gameManager.characterList;
        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
        {
            foreach (CharacterData data in dataList)
            {
                if (!data)
                    continue;
                CharacterInBattle atBattle = data.characterAtBattle;
                data.hp = Mathf.Max(1, atBattle.Hp);
                data.gridIndex = atBattle.grid.index;
                Dictionary<string, object> dict = new();
                dict.Add("Hp", data.hp);
                dict.Add("GridIndex", data.gridIndex);
                DataManager.dataManager.SetDocumentData(dict, string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Characters"), data.docId);
            }
            Dictionary<string, object> docDict = new()
            {
                { "Scene", "Stage"},
                { "Nodes", StageScenarioBase.nodes },
                { "EnemyNum", GameManager.gameManager.enemyNum},
                { "BossNum", GameManager.gameManager.bossNum }
            };
            DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.Uid);
            return Task.CompletedTask;
        });

        await ClearEnemyAsync();
        await ItemManager.itemManager.SetLootAsync();
        canvasClear.SetActive(true);
        buttonNext.gameObject.SetActive(true);
    }

    public static void RefreshGrid()
    {
        foreach (GridObject grid in CharacterGrids)
        {
            grid.RefreshGrid();
        }
    }

    public void ToMap()
    {
        foreach (BaseInBattle x in characters)
        {
            if (x)
                x.InBattleFieldZero();
        }
        GameManager.gameManager.canvasGrid.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("Stage" + StageScenarioBase.stageNum);
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
            if (x)
                x.StartBattle();
        }
        canvasBattle.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(ActiveRegualrEffect());
        battlePatern = BattlePatern.Battle;
        StartCoroutine(MoveGaugeCor());
    }
    private static List<EnemyPiece> MakeEnemies()
    {
        List<string> casesStr = StageScenarioBase.stageBaseCanvas.currentNode.nodeType.casesStr;
        string caseStr = casesStr[Random.Range(0, casesStr.Count)];
        List<EnemyPiece> enemyPieces = LoadEnemiesByCase(caseStr);
        return enemyPieces;
    }
    public static async Task ClearEnemyAsync()
    {
        ClearEnemyObject();
        List<DocumentSnapshot> snapshots = await DataManager.dataManager.GetDocumentSnapshots($"Progress/{GameManager.gameManager.Uid}/Enemies");
        foreach (DocumentSnapshot snapshot in snapshots)
        {
            await snapshot.Reference.DeleteAsync();
        }
    }

    private static void ClearEnemyObject()
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
            if (x)
                Destroy(x.gameObject);
        }
        characters.Clear();
    }

    public void GoToStart() => SceneManager.LoadSceneAsync("Start");
    public void CreateVisualEffect(VisualEffect _visualEffect, BaseInBattle _character, bool _isSkillVe)
    {
        Transform target;
        target = _visualEffect.fromRoot ? _character.rootTargetTransform : _character.skillTargetTransform;
        GameObject effectObj = Instantiate(_visualEffect.effectObject, target);
        if (_isSkillVe && !_visualEffect.fromRoot)
        {
            float rangeX = Random.Range(-0.5f, 0.5f);
            float rangeY = Random.Range(-0.5f, 0.5f);
            effectObj.transform.position += new Vector3(rangeX, rangeY);
        }
        if (_visualEffect.sound != string.Empty)
        {
            SoundManager.soundManager.SfxPlay(_visualEffect.sound);
        }
        Destroy(effectObj, _visualEffect.duration);
    }
    public void ChangeBackground(BackgroundType _backgroundType)
    {
        foreach (GameObject backgroundObj in backgrounds.Values)
        {
            backgroundObj.SetActive(false);
        }
        backgrounds[_backgroundType].SetActive(true);
    }


    public static List<EnemyPiece> LoadEnemiesByCase(string _caseStr)
    {
        List<EnemyPiece> enemyPieces = new();
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
            enemyObject = GameManager.gameManager.GetEnemyPrefab(id);

            EnemyInBattle enemyInBattle = enemyObject.AddComponent<EnemyInBattle>();
            enemyInBattle.InitEnemy(enemyClass, grid);
            enemies.Add(enemyInBattle);
            enemyPieces.Add(new(id, pieceForm.index));
        }
        return enemyPieces;
    }
    public static async Task LoadEnemy()
    {
        Dictionary<string, EnemyClass> enemyDict = LoadManager.loadManager.enemyiesDict;
        List<DocumentSnapshot> snapshots;
            snapshots = await DataManager.dataManager.GetDocumentSnapshots($"Progress/{GameManager.gameManager.Uid}/Enemies");
        foreach (DocumentSnapshot snapshot in snapshots)
        {
            Dictionary<string, object> dict = snapshot.ToDictionary();
            string id = (string)dict["Id"];
            int gridIndex = (int)(long)dict["GridIndex"];

            GridObject grid = EnemyGrids[gridIndex];
            GameObject enemyObject;
            EnemyClass enemyClass = enemyDict[id];
            enemyObject = GameManager.gameManager.GetEnemyPrefab(id);

            EnemyInBattle enemyScript = enemyObject.AddComponent<EnemyInBattle>();
            enemyScript.InitEnemy(enemyClass, grid);
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
    public static float CalcResist(float _resist)
    {
        if (_resist >= 0)
        {
            // 저항력이 0 이상일 때는 기존 공식 사용
            return 100f / (100f + _resist);
        }
        else
        {
            // 저항력이 음수일 때는 피해 증가 공식 사용
            return 2f - 100f / (100f - _resist);
        }
    }

    public void PassiveReconnect()
    {
        foreach (var character in characters)
        {
            if (character)
                character.PassiveReconnect();
        }
    }
    public async void GameOver()
    {
        //await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
        //{
        //    await ClearCharacterAsync();
        //    await ClearEnemyAsync();
        //    DocumentReference documentRef = FirebaseFirestore.DefaultInstance.Collection("Progress").Document(Uid);
        //    await documentRef.DeleteAsync();
        //});
        StartCoroutine(GameOverCoroutine());
        GameManager.gameManager.GameOver();
    }

    private IEnumerator GameOverCoroutine()
    {
        foreach (BaseInBattle x in enemies)
            x.StopBattle();
        yield return new WaitForSeconds(2f);
        canvasGameOver.gameObject.SetActive(true);
        canvasGameOver.SetScore(GameManager.gameManager.enemyNum, GameManager.gameManager.destinationNum, GameManager.gameManager.bossNum, GameManager.gameManager.foodNum);
        canvasBattle.gameObject.SetActive(false);
        GameManager.gameManager.canvasGrid.gameObject.SetActive(false);

    }
    public BaseInBattle GetTarget(SkillEffect effectForm, BaseInBattle _caster)
    {
        BaseInBattle effectTarget;
        switch (effectForm.isTargetEnemy)
        {
            case true:
                effectTarget = _caster.targetOpponent;
                break;
            case false:
                switch (effectForm.range)
                {
                    default:
                        effectTarget = _caster;
                        break;
                    case EffectRange.Dot:
                        effectTarget = _caster.targetAlly;
                        break;
                }
                break;
        }

        return effectTarget;
    }
}