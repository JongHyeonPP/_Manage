using EnumCollection;
using Firebase.Firestore;
using StructCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleScenario : MonoBehaviour
{
    public delegate void RegularEffectHandler();
    public RegularEffectHandler regularEffect;
    public BattleDifficulty battleDifficulty;
    CharacterBase focusedCharacter = null;
    public BattlePatern battlePatern;
    private List<CharacterBase> enemies;
    private List<CharacterBase> friendlies;
    private List<ObjectGrid> friendlyGrids;
    private List<ObjectGrid> enemyGrids;
    public ObjectGrid gridOnPointer;
    public ObjectThing dragedThing;
    public bool isDragging = false;
    #region UI
    public Transform canvasBattle;
    public Transform canvasTest;
    public GameObject panelClear;
    private TMP_Text textBattlePatern;
    public static readonly Color defaultGridColor = new(1f, 1f, 1f, 0.4f);
    public static readonly Color enemyGridColor = Color.red;
    public static readonly Color friendlyColor = Color.blue;
    public static readonly Color TestColor = Color.green;
    #endregion
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    private BattleScenarioTest battleScenarioTest;
    public RectTransform rectFriendlyGroup;
    private void Awake()
    {
        friendlies = GameManager.Friendlies;
        enemies = GameManager.Enemies;
        friendlyGrids = GameManager.FriendlyGrids;
        enemyGrids = GameManager.EnemyGrids;
        GameManager.gameManager.canvasGrid.gameObject.SetActive(true);
        textBattlePatern = canvasTest.GetChild(1).GetComponent<TMP_Text>();
        panelClear = canvasBattle.GetChild(0).gameObject;
        panelClear.SetActive(false);
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
        focusedCharacter = friendlies[0];
        battleScenarioTest = GetComponent<BattleScenarioTest>();
        if (battleScenarioTest)
            canvasTest.gameObject.SetActive(true);
        rectFriendlyGroup = GameManager.gameManager.canvasGrid.GetChild(0).GetComponent<RectTransform>();
    }
    private void Start()
    {
        SetBattlePatern(BattlePatern.OnReady);
    }
    public static int GetTargetIndex(Skill _skill)
    {
        //0은 아군에 대해서, 1은 적에 대해서, 2는 모두에 대해서 사용 가능
        int targetIndex = -1;
        foreach (var effect in _skill.effects)
        {
            if (effect.type == EffectType.Damage ||
                        effect.type == EffectType.AttDebuff ||
                        effect.type == EffectType.DefDebuff ||
                        effect.type == EffectType.Paralyze ||
                        effect.type == EffectType.Bleed ||
                        effect.type == EffectType.ArmorAtt ||
                        effect.type == EffectType.BleedTransfer
                )
            {
                if (targetIndex == 0)
                {
                    targetIndex = 2;
                    break;
                }
                else
                    targetIndex = 1;
            }
            else
            {
                if (targetIndex == 1)
                {
                    targetIndex = 2;
                    break;
                }
                else
                    targetIndex = 0;
            }
        }
        return targetIndex;
    }


    public void OnGridPointerDown(ObjectGrid _grid)
    {
        EventSystem.current.SetSelectedGameObject(null);
        dragedThing = _grid.owner;
    }
    public void MoveCharacterByGrid(ObjectGrid _startGrid, ObjectGrid _targetGrid, bool _isInstant)
    {
        ObjectThing targetCharacter = null;
        if (_targetGrid.owner)
            targetCharacter = _targetGrid.owner;
        _startGrid.owner.MoveToTargetGrid(_targetGrid, _isInstant);
        if (targetCharacter)
        {
            targetCharacter.MoveToTargetGrid(_startGrid, _isInstant);
        }
        else
        {
            _startGrid.owner = null;
        }
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
    public void OnGridPointerExit() => gridOnPointer = null;

    public void RefreshSkill()
    {
        if (battlePatern != BattlePatern.OnReady)
        foreach (var x in enemies)
        {
            x.gridPatern = GridPatern.Deactive;
        }
        foreach (var x in friendlies)
        {
            x.gridPatern = GridPatern.Deactive;
        }
        if (focusedCharacter)
            focusedCharacter.transform.GetChild(0).GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
        focusedCharacter = null;
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
            switch (battlePatern)
            {
                case BattlePatern.Default:
                    if (GameManager.IsPaused)
                    {
                        RefreshSkill();
                        GameManager.IsPaused = false;
                    }
                    else
                    {
                        GameManager.IsPaused = true;
                        battlePatern = BattlePatern.Pause;
                    }
                    break;
                case BattlePatern.Pause:
                    RefreshSkill();
                    GameManager.IsPaused = false;
                    battlePatern = BattlePatern.Default;
                    break;
            }
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

    private void SetBattlePatern(BattlePatern _battlePatern)
    {
        battlePatern = _battlePatern;
        if(battleScenarioTest)
            switch (battlePatern)
            {
                case BattlePatern.Default:
                    textBattlePatern.text = "Default";
                    break;
                case BattlePatern.OnReady:
                    textBattlePatern.text = "OnReady";
                    break;
                case BattlePatern.Done:
                    textBattlePatern.text = "Done";
                    break;
                case BattlePatern.Pause:
                    textBattlePatern.text = "Pause";
                    break;
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
        battlePatern = BattlePatern.Done;
        panelClear.SetActive(true);
        foreach (var x in friendlies)
            x.StopAllCoroutines();
        foreach (var x in enemies)
            x.StopAllCoroutines();
    }
    public void ToMap()
    {
        foreach (var x in enemies)
        {
            Destroy(x.gameObject);
        }
        enemies.Clear();
        RefreshFriendlyStc();
        GameManager.gameManager.canvasGrid.gameObject.SetActive(false);
        SceneManager.LoadScene("Map");
    }
    private void RefreshFriendlyStc()
    {
        foreach (var friendly in friendlies)
        {
            friendly.isCasting = false;
            foreach (var x in friendly.skillCors)
            {
                x.isReady = false;
            }
        }
    }
    public void StartBattle()
    {
        foreach (var x in friendlies)
        {
            x.StartSkillCor();
        }
        //foreach (var x in enemies)
        //{
        //    x.StartSkillCor();
        //}
        canvasBattle.GetChild(1).gameObject.SetActive(false);
    }
}
