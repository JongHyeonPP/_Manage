using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScenario : MonoBehaviour
{
    [SerializeField] Image imageLoadingBar;
    [SerializeField] TMP_Text textLoading;
    static string nextScene;


    // Start is called before the first frame update
    async void Start()
    {
        if (nextScene.Contains("Stage"))
        {
            if (StageScenarioBase.stageCanvas)
            {
                if (StageScenarioBase.stageCanvas.stageNum != StageScenarioBase.stageNum)
                {
                    Destroy(StageScenarioBase.stageCanvas.gameObject);
                    StageScenarioBase.stageCanvas = null;
                }
            }
        }
        StartCoroutine(LoadSceneProcess());
        if (nextScene == "Start")
        {
            await GameManager.gameManager.LoadProgressDoc();
            if (StageScenarioBase.stageCanvas)
            {
                Destroy(StageScenarioBase.stageCanvas.gameObject);
                StageScenarioBase.stageCanvas = null;
            }
        }
    }

    public static void LoadScene(string _sceneName)
    {
        nextScene = _sceneName;
        SceneManager.LoadSceneAsync("Loading");
    }
    private IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0f;
        float minimumLoadTime = 1f;

        bool needDbLoad = !LoadManager.loadManager.isInit;
        bool needBattleInit = nextScene == "Battle";

        LoadData(needDbLoad, needBattleInit);

        // 최소 로딩 시간 동안 0%에서 90%까지 진행
        while (timer < minimumLoadTime)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Lerp(0f, 0.9f, timer / minimumLoadTime);
            imageLoadingBar.fillAmount = progress;
            textLoading.text = (progress * 100).ToString("F1") + "%";
            yield return null;
        }

        // 로딩 진행 상태를 계산하는 함수
        float CalculateTotalProgress(float sceneProgress)
        {
            float combinedProgress = 0f;

            if (needDbLoad && needBattleInit)
            {
                combinedProgress = Mathf.Clamp01(LoadManager.loadManager.dbProgress * 0.5f + BattleScenario.battleProgress * 0.5f);
            }
            else if (needDbLoad)
            {
                combinedProgress = Mathf.Clamp01(LoadManager.loadManager.dbProgress);
            }
            else if (needBattleInit)
            {
                combinedProgress = Mathf.Clamp01(BattleScenario.battleProgress);
            }
            else
            {
                // DB 로딩과 배틀 초기화가 필요 없는 경우
                combinedProgress = sceneProgress;
            }

            // 씬 로딩의 가중치를 높게 설정
            return Mathf.Clamp01(combinedProgress * 0.5f + sceneProgress * 0.5f);
        }

        // 씬 로딩 진행
        bool progressBeyond90Percent = false;
        float post90PercentTimer = 0f;
        while (!op.isDone)
        {
            float sceneProgress = Mathf.Clamp01(op.progress / 0.9f);
            float totalProgress = CalculateTotalProgress(sceneProgress);

            if (sceneProgress < 1f)
            {
                imageLoadingBar.fillAmount = totalProgress;
                textLoading.text = (totalProgress * 100).ToString("F1") + "%";
            }
            else if (!progressBeyond90Percent)
            {
                progressBeyond90Percent = true;
                post90PercentTimer = 0f;
            }

            if (progressBeyond90Percent)
            {
                post90PercentTimer += Time.unscaledDeltaTime;
                float progress = Mathf.Lerp(0.9f, 1f, post90PercentTimer);
                totalProgress = CalculateTotalProgress(progress);

                imageLoadingBar.fillAmount = totalProgress;
                textLoading.text = (totalProgress * 100).ToString("F1") + "%";

                if (totalProgress >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }

            yield return null;
        }

        async void LoadData(bool needDbLoad, bool needBattleInit)
        {
            if (needDbLoad)
            {
                // DB 초기화 진행
                await LoadManager.loadManager.LoadDbBaseData();
            }

            if (needBattleInit)
            {
                // 배틀 초기화 진행
                BattleScenario.Init_BattleSetAsync();
            }
        }
    }

}
