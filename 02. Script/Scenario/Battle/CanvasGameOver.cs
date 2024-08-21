using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameOver : MonoBehaviour
{
    [SerializeField] TMP_Text textTitle;
    [SerializeField]ScoreSlot scoreSlotEnemy;
    [SerializeField]ScoreSlot scoreSlotDestination;
    [SerializeField]ScoreSlot scoreSlotBoss;
    [SerializeField]ScoreSlot scoreSlotFood;
    [SerializeField]ScoreSlot scoreSlotTotal;
    [SerializeField]Image imageButton;
    [SerializeField]TMP_Text textButton;
    public async void SetScore(int _enemyNum, int _destinationNum, int _bossNum, int _foodNum)
    {
        imageButton.gameObject.SetActive(false);
        int enemyScore = GetScore(ScoreType.Enemy, _enemyNum);
        int destinationScore = GetScore(ScoreType.Destination, _destinationNum);
        int bossScore = GetScore(ScoreType.Boss, _bossNum);
        int foodScore = GetScore(ScoreType.Food, _foodNum);
        int fameScore = enemyScore + destinationScore + bossScore + foodScore;
        scoreSlotEnemy.SetScore(_enemyNum, enemyScore);
        scoreSlotDestination.SetScore(_destinationNum, destinationScore);
        scoreSlotBoss.SetScore(_bossNum, bossScore);
        scoreSlotFood.SetScore(_foodNum, foodScore);
        await scoreSlotTotal.SetFame(fameScore);
        StartCoroutine(AllMoveCoroutine());

    }
    private IEnumerator AllMoveCoroutine()
    {
        scoreSlotEnemy.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotDestination.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotBoss.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotFood.ShowScore();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(scoreSlotTotal.ShowTotal());
        StartCoroutine(FadeInButton(0.5f));

    }
    private int GetScore(ScoreType _scoreType, int _num)
    {
        switch (_scoreType)
        {
            default:
                return _num * 2;
            case ScoreType.Destination:
                return _num * 5;
            case ScoreType.Boss:
                return _num * 10;
            case ScoreType.Food:
                return _num * 1;

        }
    }
    public void OnNextButtonClick()
    {
        LoadingScenario.LoadScene("Start");
    }
    [ContextMenu("GameOverTest")]
    public void GameOverTest()
    {
        BattleScenario battleScenario = FindAnyObjectByType<BattleScenario>();
        battleScenario.canvasBattle.gameObject.SetActive(false);
        gameObject.SetActive(true);
        SetScore(3, 3, 3, 3);
    }
    private IEnumerator FadeInButton(float duration)
    {
        imageButton.gameObject.SetActive(true);
        float elapsed = 0f;

        // 초기 색상 가져오기
        Color imageColor = imageButton.color;
        Color textColor = textButton.color;

        // 알파값을 0으로 설정
        imageColor.a = 0;
        textColor.a = 0;

        // 초기 알파값 설정
        imageButton.color = imageColor;
        textButton.color = textColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 알파값을 선형 보간하여 동시에 증가
            imageColor.a = t;
            textColor.a = t;

            imageButton.color = imageColor;
            textButton.color = textColor;

            yield return null;
        }

        // 최종 알파값 1로 설정
        imageColor.a = 1;
        textColor.a = 1;

        imageButton.color = imageColor;
        textButton.color = textColor;
    }

}