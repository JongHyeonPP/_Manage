using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumCollection;
using System.Collections.Generic;
using Firebase.Firestore;
using System;
using Unity.VisualScripting;

public class StageTextUi : MonoBehaviour
{
    public Image imageBackground;
    public TMP_Text textInfo;
    public Button buttonNext;
    public TMP_Text textNext;
    readonly float showDuration = 1f;
    readonly float fadeDuration = 0.5f;
    public AllClearPanel allClearPanel;
    public void StageStart()
    {
        gameObject.SetActive(true);
        allClearPanel.gameObject.SetActive(false);
        buttonNext.gameObject.SetActive(false);
        textInfo.gameObject.SetActive(true);
        string text0 = GameManager.language == Language.Ko ? "스테이지" : "Stage";
        string text1 = GameManager.language == Language.Ko ? "시작" : "Start";
        textInfo.text = $"{text0} {StageScenarioBase.stageNum+1}\n{text1}";
        StartCoroutine(StartCoroutine());
    }
    [ContextMenu("StageClear")]
    public async void StageClear()
    {
        GameManager.gameManager.buttonInventory.enabled = false;
        GameManager.gameManager.buttonSetting.enabled = false;
        Color backgroundColor = imageBackground.color;
        backgroundColor.a = 0.7f;
        imageBackground.color = backgroundColor;

        Color textInfoColor = textInfo.color;
        textInfoColor.a = 1f;
        textInfo.color = textInfoColor;

        gameObject.SetActive(true);
        textInfo.gameObject.SetActive(true);
        allClearPanel.gameObject.SetActive(false);
        if (StageScenarioBase.stageNum == 2)
        {
            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
            {
                allClearPanel.SetScore();
                GameManager.gameManager.ResetGame();
                StartCoroutine(AllClearCoroutine());
            });
        }
        else
        {
            StartCoroutine(ClearCoroutine());
        }
    }
    private IEnumerator AllClearCoroutine()
    {
        textInfo.text = GameManager.language == Language.Ko ? "모든 스테이지\n클리어!" : "Clear all Stages!";
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.FadeUi(textInfo, 1f, false));
        allClearPanel.gameObject.SetActive(true);
        StartCoroutine(allClearPanel.AllMoveCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {

        buttonNext.gameObject.SetActive(true);
        string text0 = GameManager.language == Language.Ko ? "스테이지" : "Stage";
        string text1 = GameManager.language == Language.Ko ? "클리어" : "Clear";
        textInfo.text = $"{text0} {StageScenarioBase.stageNum + 1}\n{text1}!";
        buttonNext.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        textNext.color = new Color(1f, 1f, 1f, 0f);
        buttonNext.enabled = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.FadeUi(buttonNext.GetComponent<Image>(), 0.5f, true));
        StartCoroutine(GameManager.FadeUi(textNext, 0.5f, true));
        yield return new WaitForSeconds(0.5f);
        buttonNext.enabled = true;
    }
    public void OnNextButtonClearClick()
    {
        StageScenarioBase.nodes = new();
        Dictionary<string, object> docDict = new()
        {

            { "Nodes",  FieldValue.Delete},
            { "NodeTypes", FieldValue.Delete },
            {"StageNum",  ++StageScenarioBase.stageNum}
        };
        DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.uid);
        StageScenarioBase.phase = -1;
        StageScenarioBase.nodeTypes = new string[21];
        GameManager.gameManager.buttonInventory.enabled = true;
        GameManager.gameManager.buttonSetting.enabled = true;
        StageScenarioBase.state = StateInMap.NeedPhase;
        LoadingScenario.LoadScene("Stage"+ (StageScenarioBase.stageNum));
    }
    private IEnumerator StartCoroutine()
    {
        // 1초 동안 보여줌
        yield return new WaitForSeconds(showDuration);

        // 0.5초 동안 페이드 아웃

        float fadeTimer = 0f;

        Color imageInitialColor = imageBackground.color;
        Color textInitialColor = textInfo.color;

        float initialImageAlpha = imageBackground.color.a;
        float initialTextAlpha = textInfo.color.a;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alphaFactor = fadeTimer / fadeDuration;

            float newImageAlpha = Mathf.Lerp(initialImageAlpha, 0f, alphaFactor);
            float newTextAlpha = Mathf.Lerp(initialTextAlpha, 0f, alphaFactor);

            imageBackground.color = new Color(imageInitialColor.r, imageInitialColor.g, imageInitialColor.b, newImageAlpha);
            textInfo.color = new Color(textInitialColor.r, textInitialColor.g, textInitialColor.b, newTextAlpha);

            yield return null;
        }

        // 완전히 투명하게 설정
        imageBackground.color = new Color(imageInitialColor.r, imageInitialColor.g, imageInitialColor.b, 0f);
        textInfo.color = new Color(textInitialColor.r, textInitialColor.g, textInitialColor.b, 0f);
        gameObject.SetActive(false);
    }
}
