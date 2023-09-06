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
    public CharacterBase moveTarget;
    private BattleScenario battleScenario;
    private void Start()
    {
        botText = canvasTest.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        moveText = canvasTest.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        battleScenario = GameManager.battleScenario;
    }
    public enum TestPattern
    {
        Default, Bot, Move
    }
    public TestPattern testPattern = TestPattern.Default;
    public void BattleInit()
    {
        foreach (FriendlyScript x in GameManager.gameManager.Friendlies)
        {
            x.hp = x.maxHp;
            x.CalculateHpImage();
        }
        foreach (EnemyScript x in GameManager.gameManager.Enemies)
        {
            x.hp = x.maxHp;
            x.CalculateHpImage();
        }
    }
    public void BotTest()
    {
        SwitchTest(TestPattern.Bot);
    }

    private void SwitchTest(TestPattern _testPatern)
    {
        moveTarget = null;
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

    public void MoveTest()
    {
        SwitchTest(TestPattern.Move);
    }
    public void RefreshTest()
    {
        testPattern = TestPattern.Default;
        moveTarget = null;
        RefreshTest_Text();
    }

    private void RefreshTest_Text()
    {
        botText.text = "봇 생성";
        moveText.text = "캐릭터\n이동";
    }

    public void CreateBot(int _gridIndex, bool _isEnemy)
    {
        GameObject botObject = GameManager.gameManager.InitCharacterObject(_gridIndex, _isEnemy, "TestBot");
        CharacterBase baseScript;
        if (_isEnemy)
        {
            EnemyScript script = botObject.AddComponent<EnemyScript>();
            script.InitEnemy(null, _gridIndex, 100f, 10f, 0f);
            GameManager.gameManager.Enemies.Add(script);
            baseScript = script;
        }
        else
        {
            FriendlyScript script = botObject.AddComponent<FriendlyScript>();
            script.InitFriendly(null, null, _gridIndex, 100f, 10f, 10f, 0f, 0);
            GameManager.gameManager.Friendlies.Add(script);
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
