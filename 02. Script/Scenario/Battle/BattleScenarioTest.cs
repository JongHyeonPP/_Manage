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
        battleScenario = GameManager.battleScenario;
        for (int i = 0; i < 9; i++)
        {
            int index = i;
            var friendlyGrid = BattleScenario.FriendlyGrids[i];
            friendlyGrid.GetComponent<Button>().onClick.AddListener(() => OnCharacterGridClicked(BattleScenario.FriendlyGrids[index], false));
            var enemyGrid = BattleScenario.EnemyGrids[i];
            enemyGrid.GetComponent<Button>().onClick.AddListener(() => OnCharacterGridClicked(BattleScenario.EnemyGrids[index], true));
        }

    }
    public enum TestPattern
    {
        Default, Bot, Move
    }
    public TestPattern testPattern = TestPattern.Default;
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
                    botText.text = "�� ����\n���";
                    break;
                case TestPattern.Move:
                    moveText.text = "ĳ����\n�̵� ���";
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
        botText.text = "�� ����";
        moveText.text = "ĳ����\n�̵�";
    }
    public void StageClearTest()
    {
        battleScenario.StageClear();
    }
    public void GameOverTest()
    {
        GameManager.gameManager.GameOver();
    }
}
