using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhaseProgressUi : MonoBehaviour
{
    [SerializeField] TMP_Text textStage;
    public TMP_Text textProgress_0;
    private void Start()
    {
        SetText();
        SettingManager.LanguageChangeEvent += SetText;
    }
    public void SetText()
    {
        textStage.text = GameManager.language == EnumCollection.Language.Ko ? "스테이지 " : "Stage ";
        textStage.text += StageScenarioBase.stageNum + 1;
        textProgress_0.text = (StageScenarioBase.phase + 1).ToString();
        Color color = textProgress_0.color;
        color.a = 1;
        textProgress_0.color = color;
    }
    public IEnumerator FadeOutInText()
    {
        textStage.text = GameManager.language == EnumCollection.Language.Ko ? "스테이지 " : "Stage ";
        textStage.text += StageScenarioBase.stageNum + 1;
        textProgress_0.text = StageScenarioBase.phase.ToString();
        yield return StartCoroutine(GameManager.FadeUi(textProgress_0, 0.5f, false));
        textProgress_0.text = (StageScenarioBase.phase+1).ToString();
        StartCoroutine(GameManager.FadeUi(textProgress_0, 0.5f, true));
    }
}
