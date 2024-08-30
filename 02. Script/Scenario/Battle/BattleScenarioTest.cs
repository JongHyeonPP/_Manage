using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EnumCollection;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.TextCore.Text;

public class BattleScenarioTest : MonoBehaviour
{
    public Transform canvasTest;
    public int moveTargetIndex;
    private BattleScenario battleScenario;
    private void Awake()
    {
        if (!GameManager.gameManager)
            return;
        battleScenario = GameManager.battleScenario;
    }
    public enum TestPattern
    {
        Default, Bot, Move
    }
    public TestPattern testPattern = TestPattern.Default;
    public void RefreshTest()
    {
        testPattern = TestPattern.Default;
        moveTargetIndex = -1;
    }

    public async void StageClearTest()
    {
        foreach (BaseInBattle x in BattleScenario.enemies)
        {
            GameManager.gameManager.enemyNum++;
        }
        BattleScenario.RefreshGrid();
        foreach (BaseInBattle x in BattleScenario.characters)
        {
            if (x)
            {
                x.StopBattle();
                x.StopAllCoroutines();
            }
        }
        foreach (var x in BattleScenario.enemies)
        {
            x.StopAllCoroutines();
        }
        await battleScenario.StageClearAsync();
    }
    public void GameOverTest()
    {
        GameManager.gameManager.GameOver();
    }
}
