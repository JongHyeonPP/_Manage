using EnumCollection;
using Firebase.Firestore;
using BattleCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class BattleScenario : MonoBehaviour
{
    public Action regularEffect;
    public BattleDifficulty battleDifficulty;
    public static List<CharacterBase> enemies;
    public static List<CharacterBase> friendlies;
    private static List<ObjectGrid> friendlyGrids;
    private static List<ObjectGrid> enemyGrids;
    public ObjectGrid gridOnPointer;
    public bool isDragging = false;
    #region UI
    public Transform canvasBattle;
    public Transform canvasTest;
    public Transform panelClear;
    public Transform panelGameOver;
    public static readonly Color defaultGridColor = new(1f, 1f, 1f, 0.4f);
    public static readonly Color enemyGridColor = Color.red;
    public static readonly Color friendlyColor = Color.blue;
    public static readonly Color TestColor = Color.green;
    #endregion
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    private BattleScenarioTest battleScenarioTest;
    public RectTransform rectFriendlyGroup;
    public bool isInFriendly;
    public static List<EffectType> buffOrDebuff;
    public static BattlePatern battlePatern;
    public float moveGauge;
    private Coroutine regularEffectCor;
    public static List<ObjectGrid> FriendlyGrids { get; private set; } = new();
    public static List<ObjectGrid> EnemyGrids { get; private set; } = new();
    public float RewardAscend = 0;
    public static readonly float gridCorrection = 20f;
    public Transform prefabSet;
    private Dictionary<BackgroundType, GameObject> backgrounds = new();
    private async void Awake()
    {
        friendlyGrids = FriendlyGrids;
        enemyGrids = EnemyGrids;

        GameManager.gameManager.canvasGrid.gameObject.SetActive(true);
        panelClear = canvasBattle.GetChild(1);
        panelClear.gameObject.SetActive(false);
        panelGameOver = canvasBattle.GetChild(2);
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
        SettingManager.onLanguageChange += LanguageChange;
        LanguageChange(GameManager.language);

        
        foreach (var x in friendlies)
        {
            regularEffect += x.ActiveRegularEffect;
        }
        foreach (var x in enemies)
        {
            regularEffect += x.ActiveRegularEffect;
        }
        battleScenarioTest = GetComponent<BattleScenarioTest>();
        if (battleScenarioTest)
            canvasTest.gameObject.SetActive(true);
        rectFriendlyGroup = GameManager.gameManager.canvasGrid.GetChild(0).GetComponent<RectTransform>();
        GameManager.gameManager.uiCamera.SetActive(true);

        FriendlyDataInit();//Data->Base

        if (enemies.Count == 0)
        {
            string selectedCase = await MakeEnemies(GameManager.gameManager.nodeLevel);
            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction => 
            {
                GameManager.gameManager.InitSeed();
                DataManager.dataManager.SetDocumentData("EnemyCase", selectedCase, "Progress", GameManager.gameManager.Uid);
                DataManager.dataManager.SetDocumentData("Scene", "Battle", "Progress", GameManager.gameManager.Uid);
                return Task.CompletedTask;
            });
        }
        foreach (CharacterBase x in friendlies)
        {
            if (x.Hp == 0)
            {
                x.Hp = 1f;
                x.gameObject.SetActive(true);
                x.isDead = false;
            }
        }
        backgrounds[BackgroundType.Plains] = prefabSet.GetChild(0).gameObject;
        backgrounds[BackgroundType.Beach] = prefabSet.GetChild(1).gameObject;
        backgrounds[BackgroundType.Ruins] = prefabSet.GetChild(2).gameObject;
        backgrounds[BackgroundType.Cave] = prefabSet.GetChild(3).gameObject;
        backgrounds[BackgroundType.Desert] = prefabSet.GetChild(4).gameObject;
        backgrounds[BackgroundType.Forest] = prefabSet.GetChild(5).gameObject;
        backgrounds[BackgroundType.Lava] = prefabSet.GetChild(6).gameObject;
        backgrounds[BackgroundType.IceField] = prefabSet.GetChild(7).gameObject;
        backgrounds[BackgroundType.Swamp] = prefabSet.GetChild(8).gameObject;
    }
    public void OnGridPointerDown()
    {
        GameManager.battleScenario.isDragging = true;
        GameManager.IsPaused = true;

    }
    public void MoveCharacterByGrid(ObjectGrid _startGrid, ObjectGrid _targetGrid)
    {
        CharacterBase targetCharacter = null;
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

    internal void FriendlyDataInit()
    {
        List<CharacterData> characterDataList = CharacterManager.characterManager.GetCharacters();
        foreach (CharacterBase x in friendlies)
        {
            CharacterData characterData = characterDataList.FirstOrDefault(item => item.docId == x.documentId);
            x.maxHp = x.maxHpInBattle = characterData.maxHp;//Hp만 우선적 init 나머지는 CharacterBase.SetSkillsAndStart()에서
            x.ability = characterData.ability;
            x.speed = characterData.speed;
            x.resist = characterData.resist;
            x.Hp = characterData.hp;
            x.skills = new();
            x.job = LoadManager.loadManager.jobsDict[characterData.jobId];
            x.skills.Add(new(x.job.effects));
            foreach (string skillName in characterData.skillNames)
            {
                if (skillName.Length > 0)
                    x.skills.Add(GameManager.LocalizeSkill(skillName));
            }
            //Weapon
            for (int i = 0; i < 2; i++)
            {
                LoadManager.loadManager.weaponDict.TryGetValue(characterData.weaponIds[i], out WeaponClass weaponClass);
                if (characterData.weaponCurs[i] != characterData.weaponIds[i])//최근 무기와 일치하지 않다면
                {
                    x.shieldRenderer[i].sprite = x.weaponRenderer[i].sprite = null;//기존 무기를 해제한다.
                    if (weaponClass != null)//새로 장착할 무기가 있다면
                    {
                        if (weaponClass.type == WeaponType.Shield)//방패 장착
                        {
                            x.shieldRenderer[i].sprite = weaponClass.sprite;
                        }
                        else//무기 장착
                        {
                            x.weaponRenderer[i].sprite = weaponClass.sprite;
                        }
                        
                    }
                    characterData.weaponCurs[i] = characterData.weaponIds[i];//갱신
                }
                if (weaponClass != null)
                {
                    x.ability += weaponClass.ability;
                    x.resist += weaponClass.resist;
                    x.maxHp += weaponClass.hp;
                    x.speed += weaponClass.speed;
                    if (weaponClass.effects != null)
                        x.skills.Add(new(weaponClass.effects));
                }
            }
            x.grid = FriendlyGrids[characterData.index];
            x.MoveToTargetGrid(x.grid, true);
            x.grid.owner = x;
        }
        battlePatern = BattlePatern.OnReady;
    }

    public void OnGridPointerEnter(ObjectGrid _grid)
    {
        Image gridImage = _grid.GetComponent<Image>();
        //봇 생성 모드라면
        if (battleScenarioTest)
        {
            switch (battleScenarioTest.testPattern)
            {
                case BattleScenarioTest.TestPattern.Bot:
                    if (_grid.owner == null)
                        gridImage.color = TestColor;
                    return;
                case BattleScenarioTest.TestPattern.Move:
                    gridImage.color = TestColor;
                    return;
            }
        }
    }
    public void RefreshGrid(bool _isEnemyGrid)
    {
        foreach (var x in _isEnemyGrid ? enemyGrids : friendlyGrids)
        {
            x.GetComponent<Image>().color = defaultGridColor;
        }
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
            foreach (EnemyScript x in enemies)
            {
                indexes.Add(x.grid.index);
            }
        }
        else
        {
            foreach (FriendlyScript x in friendlies)
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

    private void LanguageChange(Language _language)
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[_language];
        }
    }
    public IEnumerator ActiveRegualrEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            regularEffect();
        }
    }

    public void StageClear()
    {
        Debug.Log("StageClear");
        panelClear.gameObject.SetActive(true);
        StartCoroutine(StopCoroutineInSecond());
        List<CharacterData> characters = CharacterManager.characterManager.GetCharacters();
        foreach (CharacterData x in characters)
        {
            x.hp = friendlies.Where(item => item.documentId == x.docId).FirstOrDefault().Hp;
        }

        foreach (CharacterBase x in enemies)
        {
            Destroy(x.gameObject, 1f);
        }
        FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
        {
            foreach (CharacterBase x in friendlies)
            {
                DataManager.dataManager.SetDocumentData("Hp", string.Format("{0}/{1}", Mathf.Max(x.Hp, 1), x.maxHp), string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Friendlies"), x.documentId);
                x.grid.owner = null;
            }
            DataManager.dataManager.SetDocumentData("Scene", "Stage0", "Progress", GameManager.gameManager.Uid);
            return Task.CompletedTask;
        });
        ClearEnemy();

        IEnumerator StopCoroutineInSecond()
        {
            yield return new WaitForSeconds(1f);
            foreach (var x in friendlies)
                x.StopAllCoroutines();
        }
    }
    public void ToMap()
    {
        foreach (var x in friendlies)
        {
            x.InBattleFieldZero();
        }
        GameManager.gameManager.canvasGrid.gameObject.SetActive(false);
        SceneManager.LoadScene("Stage0");

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
        foreach (var x in friendlies)
        {
            x.StartBattle();
        }
        canvasBattle.GetChild(0).gameObject.SetActive(false);
        regularEffectCor = StartCoroutine(ActiveRegualrEffect());
        battlePatern = BattlePatern.Battle;
        StartCoroutine(MoveGaugeCor());
    }
    private async Task<string> MakeEnemies(int _nodeLevel)
    {
        var values = LoadManager.loadManager.enemyCaseDict;
        List<string> ableCases = values.Where(item => item.Value.levelRange.Contains(_nodeLevel))
                              .Select(item => item.Key)
                              .ToList();
        string selectedCase = ableCases[UnityEngine.Random.Range(0, ableCases.Count)];
        await GameManager.gameManager.LoadEnemies(selectedCase);
        return selectedCase;
    }
    public static void ClearEnemy()
    {
        foreach (var x in enemies)
        {
            Destroy(x.gameObject);
        }
        enemies.Clear();
        DataManager.dataManager.SetDocumentData("EnemyCase", FieldValue.Delete, "Progress", GameManager.gameManager.Uid);
    }
    public static async void ClearFriendly()
    {
        foreach (var x in friendlies)
        {
            Destroy(x.gameObject);
        }
        friendlies.Clear();
        string collectionRef = "Progress/" + GameManager.gameManager.Uid + "/Friendlies";
        List<DocumentSnapshot> result = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Friendlies"));
        foreach (DocumentSnapshot doc in result)
        {
            await doc.Reference.DeleteAsync();
        }
    }
    public void DestoyByDocId(string _docId)
    {
        CharacterBase x = friendlies.Where(item => item.documentId == _docId).FirstOrDefault();
        friendlies.Remove(x);
        Destroy(x.gameObject);
    }
    public void GoToStart() => SceneManager.LoadScene("Start");
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
}