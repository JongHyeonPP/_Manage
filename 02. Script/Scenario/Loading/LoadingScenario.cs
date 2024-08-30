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

        // �ּ� �ε� �ð� ���� 0%���� 90%���� ����
        while (timer < minimumLoadTime)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Lerp(0f, 0.9f, timer / minimumLoadTime);
            imageLoadingBar.fillAmount = progress;
            textLoading.text = (progress * 100).ToString("F1") + "%";
            yield return null;
        }

        // �ε� ���� ���¸� ����ϴ� �Լ�
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
                // DB �ε��� ��Ʋ �ʱ�ȭ�� �ʿ� ���� ���
                combinedProgress = sceneProgress;
            }

            // �� �ε��� ����ġ�� ���� ����
            return Mathf.Clamp01(combinedProgress * 0.5f + sceneProgress * 0.5f);
        }

        // �� �ε� ����
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
                // DB �ʱ�ȭ ����
                await LoadManager.loadManager.LoadDbBaseData();
            }

            if (needBattleInit)
            {
                // ��Ʋ �ʱ�ȭ ����
                BattleScenario.Init_BattleSetAsync();
            }
        }
    }

}
