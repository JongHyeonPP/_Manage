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

public class BattleScenario : MonoBehaviour
{
    public delegate void RegularEffectHandler();
    public RegularEffectHandler regularEffect;
    public BattleDifficulty battleDifficulty;
    CharacterBase focusedCharacter = null;
    public BattlePatern battlePatern;
    private bool onskill = true;
    private List<CharacterBase> enemies;
    private List<CharacterBase> friendlies;
    private List<GameObject> friendlyGrids;
    private List<GameObject> enemyGrids;
    public CharacterBase.SkillTargetCor SelectedStc { get; private set; }
    #region UI
    public Transform canvasBattle;
    private Transform characterUI;
    private TMP_Text textBattlePatern;
    public static readonly Color defaultGridColor = new(1f, 1f, 1f, 0.4f);
    public static readonly Color opponentGridColor = Color.red;
    public static readonly Color allyGridColor = Color.blue;
    public static readonly Color TestColor = Color.green;
    #endregion
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    private BattleScenarioTest battleScenarioTest;

    private void Awake()
    {
        friendlies = GameManager.gameManager.Friendlies;
        enemies = GameManager.gameManager.Enemies;
        friendlyGrids = GameManager.gameManager.FriendlyGrids;
        enemyGrids = GameManager.gameManager.EnemyGrids;
        GameManager.gameManager.canvasGrid.gameObject.SetActive(true);
        textBattlePatern = canvasBattle.GetChild(0).GetComponent<TMP_Text>();
        characterUI = canvasBattle.GetChild(1);
        for (int i = 0; i < friendlies.Count; i++)
        {
            var friendly = friendlies[i] as FriendlyScript;
            for (int j = 0; j < friendly.skillTargetCor.Length; j++)
            {
                Transform skillTransform = characterUI.GetChild(i).GetChild(j + 2);
                skillTransform.gameObject.SetActive(true);
                var skillImage = skillTransform.GetComponent<Image>();
                friendly.skillImages.Add(skillImage);
                friendly.skillTargetCor[j].imageSkill = skillTransform.GetComponent<Image>();
                skillTransform.GetChild(1).GetComponent<TMP_Text>().text = friendly.skillTargetCor[j].skill.name[GameManager.language];
                skillTransform.GetChild(0).GetComponent<Image>().fillAmount = 0f;
            }
        }
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

        SetBattlePatern(BattlePatern.OnReady);
        foreach (var x in friendlies)
        {
            regularEffect += x.ActiveRegularEffect;
        }
        foreach (var x in enemies)
        {
            regularEffect += x.ActiveRegularEffect;
        }
        focusedCharacter = friendlies[0];
        SetSelectedStc(0, 0);
        ActiveCampByStc();
        battleScenarioTest = GetComponent<BattleScenarioTest>();
    }

    public void OnGridPointEnter(int _gridIndex, bool _isEnemyGrid)
    {
        CharacterBase character = GetCharacter(_gridIndex, _isEnemyGrid);
        //봇 생성 모드라면
        if (battleScenarioTest)
        {
            var grid = GetCharacterGrids(_isEnemyGrid)[_gridIndex];
            switch (battleScenarioTest.testPattern)
            {
                case BattleScenarioTest.TestPattern.Bot:
                    if (character == null)
                        grid.GetComponent<Image>().color = TestColor;
                    return;
                case BattleScenarioTest.TestPattern.Move:
                    if ((battleScenarioTest.moveTarget && character == null) || (!battleScenarioTest.moveTarget && character != null))
                    {
                        grid.GetComponent<Image>().color = TestColor;
                    }
                    return;
            }
        }
        if (onskill)
        {
            if (SelectedStc.skill.target == SkillTarget.Nontarget)
            {
                foreach (var x in enemies)
                {
                    if (x.gridIndex == _gridIndex - 1 || x.gridIndex == _gridIndex - 2)
                        return;
                }
            }
            if (character == null || !(character.gridPatern == GridPatern.Interactable))
                return;
            SetGridColorByStc(_gridIndex, _isEnemyGrid);
        }
    }
    public void OnGridPointExit(int _gridIndex, bool _isEnemyGrid)
    {
        var grids = GetCharacterGrids(_isEnemyGrid);
        if (battleScenarioTest&& battleScenarioTest.testPattern!=BattleScenarioTest.TestPattern.Default)
        {
            grids[_gridIndex].GetComponent<Image>().color = defaultGridColor;
        }
        if (onskill)
        {
            foreach (var x in grids)
            {
                x.GetComponent<Image>().color = defaultGridColor;
            }
        }
    }
    public void ActiveCampByStc()
    {
        //0은 아군, 1은 적, 2는 둘 다 

        short targetIndex = -1;
        foreach (var effect in SelectedStc.skill.effects)
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
        if (targetIndex != 0)
        {
            foreach (var x in enemies)
            {
                x.gridPatern = GridPatern.Interactable;
            }
        }
        if (targetIndex != 1)
        {
            foreach (var x in friendlies)
            {
                x.gridPatern = GridPatern.Interactable;
            }
        }
    }
    public void OnCharacterGridClicked(int _gridIndex, bool _isEnemyGrid)
    {
        EventSystem.current.SetSelectedGameObject(null);
        var character = GetCharacter(_gridIndex, _isEnemyGrid);
        //Test Logic
        if (battleScenarioTest&&battleScenarioTest.testPattern != BattleScenarioTest.TestPattern.Default)
        {
            switch (battleScenarioTest.testPattern)
            {
                case BattleScenarioTest.TestPattern.Bot:
                    if (!character)
                    {
                        battleScenarioTest.CreateBot(_gridIndex, _isEnemyGrid);
                        GetCharacterGrids(_isEnemyGrid)[_gridIndex].GetComponent<Image>().color = defaultGridColor;
                    }
                    battleScenarioTest.RefreshTest();
                    RefreshGrid(_isEnemyGrid);
                    SetGridColorByStc(_gridIndex, _isEnemyGrid);
                    break;
                case BattleScenarioTest.TestPattern.Move:
                    if (battleScenarioTest.moveTarget)
                    {
                        if (battleScenarioTest.moveTarget.gridIndex == _gridIndex) return;
                        if (character)//자리에 캐릭터가 있다면 위치 스왑
                        {
                            character.MoveCharacter(battleScenarioTest.moveTarget.gridIndex, _isEnemyGrid);
                        }
                        battleScenarioTest.moveTarget.MoveCharacter(_gridIndex, _isEnemyGrid);
                        battleScenarioTest.RefreshTest();
                        battleScenarioTest.moveTarget = null;
                        battleScenarioTest.RefreshTest();
                        SetGridColorByStc(_gridIndex, _isEnemyGrid);
                    }
                    else
                    {
                        battleScenarioTest.moveTarget = character;
                        RefreshGrid(_isEnemyGrid);
                    }
                    break;
            }
            if (SelectedStc != null)
            {
                GameManager.battleScenario.ActiveCampByStc();
            }
            
            return;
        }
        //Default Logic
        if (!character || character.gridPatern == GridPatern.Deactive) return;
        //논타겟 연산
        if (SelectedStc.skill.target == SkillTarget.Nontarget)
        {
            foreach (var x in enemies)
            {
                if (x.gridIndex == _gridIndex - 1 || x.gridIndex == _gridIndex - 2)
                    return;
            }
        }
        //IsSelf인데 자신을 타겟하지 않았으면 리턴
        if (SelectedStc.skill.isSelf && !(_gridIndex == focusedCharacter.gridIndex && !_isEnemyGrid)) return;
        SelectedStc.target = character;
        SelectedStc.isTargetEnemy = _isEnemyGrid;
        RefreshGrid(_isEnemyGrid);
        switch (battlePatern)//후속 작업
        {
            case BattlePatern.Default:
                RefreshSkill();
                GameManager.IsPaused = false;
                onskill = false;
                break;
            case BattlePatern.OnReady:
                SelectedStc.isReady = true;
                RefreshSkill();
                SetNextStcIndex();
                if (SelectedStc != null)
                {
                    ActiveCampByStc();
                    SetGridColorByStc(_gridIndex, _isEnemyGrid);
                }
                break;
            default:
                RefreshSkill();
                onskill = false;
                break;
        }
    }
    public void SetGridColorByStc(int _gridIndex, bool _isEnemyGrid)
    {
        if (SelectedStc == null || (SelectedStc.skill.isSelf && !(_gridIndex == focusedCharacter.gridIndex && !_isEnemyGrid))) return;
        foreach (var x in SelectedStc.skill.effects)
        {
            foreach (var x0 in GetTargets(_gridIndex, _isEnemyGrid, x))
            {
                if (x0.gridPatern == GridPatern.Interactable)
                    x0.grid.GetComponent<Image>().color = _isEnemyGrid ? opponentGridColor : allyGridColor;
            }
        }
    }
    public void RefreshSkill()
    {
        if (battlePatern != BattlePatern.OnReady)
            onskill = false;
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
        if (SelectedStc != null)
            SelectedStc.imageSkill.color = Color.white;
        SelectedStc = null;
        focusedCharacter = null;
    }
    public void RefreshGrid(bool _isEnemyGrid)
    {
        foreach(var x in _isEnemyGrid?enemyGrids:friendlyGrids)
        {
            x.GetComponent<Image>().color = defaultGridColor;
        }
    }

    public List<CharacterBase> GetTargets(int _gridIndex, bool _isEnemyGrid, SkillEffect _effect)
    {
        List<CharacterBase> enemyOrFriendly = _isEnemyGrid ? enemies : friendlies;
        List<CharacterBase> returnValue = new();
        foreach (var character in enemyOrFriendly)
        {
            int absValue = Mathf.Abs(character.gridIndex - _gridIndex);
            switch (_effect.range)
            {
                case EffectRange.Dot:
                    if (character.gridIndex == _gridIndex)
                    {
                        returnValue.Add(character);
                    }
                    break;
                case EffectRange.Cross:
                    if (absValue == 3 || absValue == 1 || absValue == 0)
                        returnValue.Add(character);
                    break;
                case EffectRange.Neighbors:
                    if (absValue == 3 || absValue == 1)
                        returnValue.Add(character);
                    break;
            }
        }
        return returnValue;
    }



    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            switch(battlePatern)
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
                indexes.Add(x.gridIndex);
            }
        }
        else
        {
            foreach (FriendlyScript x in friendlies)
            {
                indexes.Add(x.gridIndex);
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
    private CharacterBase GetCharacter(int _gridIndex, bool _isEnemyGrid)
    {
        List<CharacterBase> enemyOrFrinedly = _isEnemyGrid ? enemies : friendlies;
        foreach (var x in enemyOrFrinedly)
        {
            if (x.gridIndex == _gridIndex)
                return x;
        }
        return null;
    }
    private void SetBattlePatern(BattlePatern _battlePatern)
    {
        battlePatern = _battlePatern;
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
    public void OnSkillClicked(int _characterIndex, int _skillIndex)
    {
        EventSystem.current.SetSelectedGameObject(null);
        RefreshSkill();
        SetSelectedStc(_characterIndex, _skillIndex);
        onskill = true;
        if (battlePatern == BattlePatern.Default)
            GameManager.IsPaused = true;
    }

    private void SetSelectedStc(int _characterIndex, int _skillIndex)
    {
        focusedCharacter = friendlies[_characterIndex];
        SelectedStc = focusedCharacter.skillTargetCor[_skillIndex];
        SelectedStc.imageSkill.color = Color.red;
        focusedCharacter.transform.GetChild(0).GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        ActiveCampByStc();
    }
    private void SetNextStcIndex()
    {
        for (short i = 0; i < friendlies.Count; i++)
        {
            if (friendlies[i].skillTargetCor == null) continue;
            for (short j = 0; j < friendlies[i].skillTargetCor.Length; j++)
            {
                CharacterBase.SkillTargetCor x1 = friendlies[i].skillTargetCor[j];
                if (x1 != null && x1.isReady == false)
                {
                    SetSelectedStc(i, j);
                    return;
                }
            }
        }
        //모든 스킬의 타겟이 배정된 이후, 전투 시작
        battlePatern = BattlePatern.Default;
        foreach (var x in friendlies)
        {
            foreach (var x0 in x.skillTargetCor)
            {
                x0.StartSkillCoroutine();
            }
        }
        onskill = false;
    }
    private List<GameObject> GetCharacterGrids( bool _isEnemyGrid) => _isEnemyGrid ? enemyGrids : friendlyGrids;
}
