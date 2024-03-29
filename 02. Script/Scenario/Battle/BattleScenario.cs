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
    public static List<BaseAtBattle> enemies;
    public static List<BaseAtBattle> characters;
    public GridObject gridOnPointer;
    public bool isDragging = false;
    #region UI
    public Transform canvasBattle;
    public Transform canvasTest;
    public Transform panelClear;
    public Transform panelGameOver;
    #endregion
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    private BattleScenarioTest battleScenarioTest;
    public RectTransform rectCharacterGroup;
    public bool isInCharacter;
    public static List<EffectType> buffOrDebuff;
    public static BattlePatern battlePatern;
    public float moveGauge;
    private Coroutine regularEffectCor;
    public static List<GridObject> CharacterGrids { get; private set; } = new();
    public static List<GridObject> EnemyGrids { get; private set; } = new();
    public float RewardAscend = 0;
    public static readonly float gridCorrection = 20f;
    public Transform prefabSet;
    private Dictionary<BackgroundType, GameObject> backgrounds = new();
    private async void Awake()
    {
        GameManager.battleScenario = this;

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
        SettingManager.LanguageChangeEvent += LanguageChange;
        LanguageChange(GameManager.language);

        
        foreach (var x in characters)
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
        rectCharacterGroup = GameManager.gameManager.canvasGrid.GetChild(0).GetComponent<RectTransform>();
        GameManager.gameManager.uiCamera.SetActive(true);

        CharacterDataInit();//Data->Base

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
        foreach (BaseAtBattle x in characters)
        {
            if (x.Hp == 0)
            {
                x.Hp = 1f;
                x.gameObject.SetActive(true);
                x.isDead = false;
            }
        }
        //stage 0
        backgrounds[BackgroundType.Plains] = prefabSet.GetChild(0).gameObject;
        backgrounds[BackgroundType.Forest] = prefabSet.GetChild(5).gameObject;
        backgrounds[BackgroundType.Ruins] = prefabSet.GetChild(2).gameObject;
        //Stage1
        backgrounds[BackgroundType.Beach] = prefabSet.GetChild(1).gameObject;
        backgrounds[BackgroundType.Swamp] = prefabSet.GetChild(8).gameObject;
        backgrounds[BackgroundType.Cave] = prefabSet.GetChild(3).gameObject;
        //Stage2
        backgrounds[BackgroundType.Desert] = prefabSet.GetChild(4).gameObject;
        backgrounds[BackgroundType.Lava] = prefabSet.GetChild(6).gameObject;
        backgrounds[BackgroundType.IceField] = prefabSet.GetChild(7).gameObject;

    }
    public void OnGridPointerDown()
    {
        GameManager.battleScenario.isDragging = true;
        GameManager.IsPaused = true;

    }
    public void MoveCharacterByGrid(GridObject _startGrid, GridObject _targetGrid)
    {
        BaseAtBattle targetCharacter = null;
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

    internal void CharacterDataInit()
    {
        List<CharacterData> characterDataList = CharacterManager.characterManager.GetCharacters();
        foreach (BaseAtBattle x in characters)
        {
            CharacterData characterData = characterDataList.FirstOrDefault(item => item.docId == x.documentId);
            x.maxHp = x.maxHpInBattle = characterData.maxHp;//Hp만 우선적 init 나머지는 CharacterBase.SetSkillsAndStart()에서
            x.ability = characterData.ability;
            x.speed = characterData.speed;
            x.resist = characterData.resist;
            x.Hp = characterData.hp;
            x.skills = new();
            x.job = LoadManager.loadManager.jobsDict[characterData.jobId];
            if (x.job.effects != null)
                x.skills.Add(new(x.job.effects));
            foreach (string skillName in characterData.skillNames)
            {
                if (string.IsNullOrEmpty(skillName))
                    continue;
                    x.skills.Add(GameManager.LocalizeSkill(skillName));
            }

            //무기해야함

            x.grid = CharacterGrids[characterData.index];
            x.MoveToTargetGrid(x.grid, true);
            x.grid.owner = x;
            
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
            foreach (EnemyAtBattle x in enemies)
            {
                indexes.Add(x.grid.index);
            }
        }
        else
        {
            foreach (CharacterAtBattle x in characters)
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
        List<CharacterData> datas = CharacterManager.characterManager.GetCharacters();
        foreach (CharacterData x in datas)
        {
            x.hp = characters.Where(item => item.documentId == x.docId).FirstOrDefault().Hp;
        }

        foreach (BaseAtBattle x in enemies)
        {
            Destroy(x.gameObject, 1f);
        }
        FirebaseFirestore.DefaultInstance.RunTransactionAsync(Transaction =>
        {
            foreach (BaseAtBattle x in characters)
            {
                DataManager.dataManager.SetDocumentData("Hp", string.Format("{0}/{1}", Mathf.Max(x.Hp, 1), x.maxHp), string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Characters"), x.documentId);

            }
            DataManager.dataManager.SetDocumentData("Scene", "stage 0", "Progress", GameManager.gameManager.Uid);
            return Task.CompletedTask;
        });
        foreach (BaseAtBattle x in characters)
        {
            x.StopBattle();
        }
        ClearEnemy();
    }
    public void ToMap()
    {
        foreach (var x in characters)
        {
            x.InBattleFieldZero();
        }
        GameManager.gameManager.canvasGrid.gameObject.SetActive(false);
        SceneManager.LoadScene("stage 0");

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
    public static async void ClearCharacter()
    {
        foreach (var x in characters)
        {
            Destroy(x.gameObject);
        }
        characters.Clear();
        string collectionRef = "Progress/" + GameManager.gameManager.Uid + "/Characters";
        List<DocumentSnapshot> result = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Progress", GameManager.gameManager.Uid, "Characters"));
        foreach (DocumentSnapshot doc in result)
        {
            await doc.Reference.DeleteAsync();
        }
    }
    public void DestoyByDocId(string _docId)
    {
        BaseAtBattle x = characters.Where(item => item.documentId == _docId).FirstOrDefault();
        characters.Remove(x);
        Destroy(x.gameObject);
    }
    public void GoToStart() => SceneManager.LoadScene("Start");
    public void CreateVisualEffect(VisualEffect _visualEffect, BaseAtBattle _character, bool _isSkillVe)
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
        BackgroundType[] enumValues = (BackgroundType[])Enum.GetValues(typeof(BackgroundType));
        ChangeMap(enumValues[UnityEngine.Random.Range(0, enumValues.Length)]);
    }
    public string visualEffectStr;
    public float visualEffectDur;
    [ContextMenu("VisualEffetTest")]
    public void VisualEffectTest()
    {
        List<BaseAtBattle> temp = new(enemies);
        temp.AddRange(characters);
        foreach (BaseAtBattle x in temp)
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
}