using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AllClearPanel : MonoBehaviour
{
    [SerializeField] TMP_Text textTitle;
    [SerializeField] ScoreSlot scoreSlotEnemy;
    [SerializeField] ScoreSlot scoreSlotDestination;
    [SerializeField] ScoreSlot scoreSlotBoss;
    [SerializeField] ScoreSlot scoreSlotFood;
    [SerializeField] ScoreSlot scoreSlotAllClear;
    [SerializeField] TMP_Text textFame;
    [SerializeField] TMP_Text textFameScore;
    [SerializeField] TMP_Text textFameAscend;
    [SerializeField] Image imageButton;
    [SerializeField] TMP_Text textButton;

    private int fameScore;
    public async void SetScore()
    {
        int enemyNum = GameManager.gameManager.enemyNum;
        int destinationNum = GameManager.gameManager.destinationNum;
        int bossNum = GameManager.gameManager.bossNum;
        int foodNum = GameManager.gameManager.foodNum;
        imageButton.gameObject.SetActive(false);
        int enemyScore = GetScore(ScoreType.Enemy, enemyNum);
        int destinationScore = GetScore(ScoreType.Destination, destinationNum);
        int bossScore = GetScore(ScoreType.Boss, bossNum);
        int foodScore = GetScore(ScoreType.Food, foodNum);
        int allClearScore = 100;
        fameScore = enemyScore + destinationScore + bossScore + foodScore + allClearScore;
        scoreSlotEnemy.SetScore(enemyNum, enemyScore);
        scoreSlotDestination.SetScore(destinationNum, destinationScore);
        scoreSlotBoss.SetScore(bossNum, bossScore);
        scoreSlotFood.SetScore(foodNum, foodScore);
        scoreSlotAllClear.SetScore(-1, allClearScore);

        await SetFame(fameScore);
    }
    public IEnumerator AllMoveCoroutine()
    {
        scoreSlotEnemy.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotDestination.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotBoss.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotFood.ShowScore();
        yield return new WaitForSeconds(0.5f);
        scoreSlotAllClear.ShowScore();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(ShowFame());
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
    private async Task SetFame(int _fame)
    {
        textFame.gameObject.SetActive(false);
        textFameScore.gameObject.SetActive(false);
        textFameAscend.gameObject.SetActive(false);
        int fameAscend = Mathf.RoundToInt(_fame * GameManager.gameManager.upgradeValueDict[UpgradeEffectType.FameUp]);
        GameManager.gameManager.fame += fameAscend;
        textFame.text = (GameManager.language == Language.Ko) ? "획득한 명성" : "Gain Reputation";
        textFameAscend.gameObject.SetActive(false);
        await DataManager.dataManager.SetDocumentData("Fame", GameManager.gameManager.fame, "User", GameManager.gameManager.Uid);
        GameManager.gameManager.textFame.text = GameManager.gameManager.fame.ToString();
    }
    public IEnumerator ShowFame()
    {
        yield return StartCoroutine(GameManager.FadeUi(textFame, 1f, true));
        yield return StartCoroutine(AnimateFame());
    }
    public void OnNextButtonClick()
    {
        gameObject.SetActive(false);
        LoadingScenario.LoadScene("Start");
    }
    public void ClearTest()
    {
        gameObject.SetActive(true);
        SetScore();
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
    private IEnumerator AnimateFame()
    {
        float currentScore = 0;
        float duration = 1.0f;
        float elapsed = 0f;
        textFameScore.gameObject.SetActive(true);
        int fameAscend = Mathf.RoundToInt(fameScore * GameManager.gameManager.upgradeValueDict[UpgradeEffectType.FameUp]);
        if (fameAscend > 0)
        {
            textFameAscend.gameObject.SetActive(true);
            textFameAscend.text = string.Empty;
        }
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            currentScore = Mathf.Lerp(0, fameScore, t);

            textFameScore.text = Mathf.FloorToInt(currentScore).ToString();

            yield return null;
        }
        textFameScore.text = fameScore.ToString();
        yield return new WaitForSeconds(0.5f);
        currentScore = 0;
        elapsed = 0f;

        if (fameAscend > 0)
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                currentScore = Mathf.Lerp(0, fameAscend, t);

                // 텍스트 업데이트
                textFameAscend.text = $" (+{Mathf.FloorToInt(currentScore)})";

                yield return null;
            }
    }
}