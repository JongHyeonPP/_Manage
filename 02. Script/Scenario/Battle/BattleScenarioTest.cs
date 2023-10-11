using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EnumCollection;

public class BattleScenarioTest : MonoBehaviour
{
    public Transform canvasTest;
    private TMP_Text botText;
    private TMP_Text moveText;
    public int moveTargetIndex;
    private BattleScenario battleScenario;
    private void Start()
    {
        botText = canvasTest.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        moveText = canvasTest.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        battleScenario = GameManager.battleScenario;
        for (int i = 0; i < 9; i++)
        {
            int index = i;
            var friendlyGrid = GameManager.FriendlyGrids[i];
            friendlyGrid.GetComponent<Button>().onClick.AddListener(() => OnCharacterGridClicked(GameManager.FriendlyGrids[index], false));
            var enemyGrid = GameManager.EnemyGrids[i];
            enemyGrid.GetComponent<Button>().onClick.AddListener(() => OnCharacterGridClicked(GameManager.EnemyGrids[index], true));
        }

    }
    public enum TestPattern
    {
        Default, Bot, Move
    }
    public TestPattern testPattern = TestPattern.Default;
    public void BattleInit()
    {
        List<CharacterBase> allCharacters = new();
        allCharacters.AddRange(GameManager.Friendlies);
        allCharacters.AddRange(GameManager.Enemies);
        foreach (var x in allCharacters)
        {
            x.hp = x.maxHp;
            x.CalculateHpImage();
            x.StopAllCoroutines();
            x.isCasting = false;
            foreach (var stc in x.skillCors)
            {
                stc.isReady = false;
                if (stc.imageSkill == null) continue;
                Image imageCool = stc.imageSkill.transform.GetChild(0).GetComponent<Image>();
                imageCool.fillAmount = 0f;
            }
        }
        battleScenario.panelClear.SetActive(false);
        battleScenario.battlePatern = BattlePatern.OnReady;
    }
    public void BotTest()
    {
        SwitchTest(TestPattern.Bot);
    }

    private void SwitchTest(TestPattern _testPatern)
    {
        moveTargetIndex = -1;
        RefreshTest_Text();
        if (testPattern == _testPatern)
        {
            testPattern = TestPattern.Default;
        }
        else
        {
            testPattern = _testPatern;
            switch (_testPatern)
            {
                case TestPattern.Bot:
                    botText.text = "봇 생성\n취소";
                    break;
                case TestPattern.Move:
                    moveText.text = "캐릭터\n이동 취소";
                    break;
            }
            
        }
    }
    private void OnCharacterGridClicked(ObjectGrid _grid, bool _isEnemyGrid)
    {
        if (testPattern != TestPattern.Default)
        {
            switch (testPattern)
            {
                case TestPattern.Bot:
                    if (_grid.owner == null)
                    {
                        CreateBot(_grid, _isEnemyGrid);
                        _grid.GetComponent<Image>().color = BattleScenario.defaultGridColor;
                    }
                    RefreshTest();
                    break;
                case TestPattern.Move:
                    if (moveTargetIndex > -1)
                    {
                        //battleScenario.MoveCharacter(moveTargetIndex, _gridIndex, _isEnemyGrid, false);
                    }
                    else
                    {
                    }
                    break;
            }
            battleScenario.RefreshGrid(_isEnemyGrid);
            return;
        }
    }
    public void MoveTest()
    {
        SwitchTest(TestPattern.Move);
    }
    public void RefreshTest()
    {
        testPattern = TestPattern.Default;
        moveTargetIndex = -1;
        RefreshTest_Text();
    }

    private void RefreshTest_Text()
    {
        botText.text = "봇 생성";
        moveText.text = "캐릭터\n이동";
    }

    public void CreateBot(ObjectGrid _grid, bool _isEnemy)
    {
        GameObject botObject = GameManager.gameManager.InitCharacterObject(_grid, _isEnemy, "TestBot");
        CharacterBase baseScript;
        if (_isEnemy)
        {
            EnemyScript script = botObject.AddComponent<EnemyScript>();
            script.InitEnemy(null, _grid, 100f, 10f, 0f, 1f);
            GameManager.Enemies.Add(script);
            baseScript = script;
        }
        else
        {
            FriendlyScript script = botObject.AddComponent<FriendlyScript>();
            script.InitFriendly(null, null, _grid, 100f, 10f, 10f, 0f,1f ,0);
            GameManager.Friendlies.Add(script);
            baseScript = script;
        }
        battleScenario.regularEffect += baseScript.ActiveRegularEffect;

    }
    public void GameOverTest()
    {
        GameManager.gameManager.GameOver();
    }
    public void StageClearTest()
    {
        battleScenario.StageClear();
    }
}
