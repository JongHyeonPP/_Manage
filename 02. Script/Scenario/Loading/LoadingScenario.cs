using System;
using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    public static void LoadScene(string _sceneName)
    {
        nextScene = _sceneName;
        SceneManager.LoadScene ("Loading");
    }
    private IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0f;
        float initProgress = 0f;
        float minimumLoadTime = 1f;
        if (!LoadManager.loadManager.isInit)
            LoadManager.loadManager.LoadDbBaseData();

        if (nextScene == "Battle")
        {
            var progressReporter = new Progress<float>(progress =>
            {
                initProgress = progress * 0.5f; // �ʱ�ȭ �۾��� ��ü�� 50%�� ����
            });

            var task = BattleScenario.Init_BattleSetAsync(progressReporter);
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }

        // �ּ� �ε� �ð� ���� 0%���� 90%���� ����
        while (timer < minimumLoadTime)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Lerp(0f, 0.9f, timer / minimumLoadTime);
            imageLoadingBar.fillAmount = progress;
            textLoading.text = (progress * 100).ToString("F1") + "%";

            yield return null;
        }

        // �ʱ�ȭ �۾��� �����Ͽ� ���� �ε� ����
        while (!(op.isDone && LoadManager.loadManager.isInit))
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                float totalProgress = (op.progress * 0.5f) + initProgress; // �ε��� �ʱ�ȭ�� 50%������ �ջ�
                imageLoadingBar.fillAmount = totalProgress;
                textLoading.text = (totalProgress * 100).ToString("F1") + "%";
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                float progress = Mathf.Lerp(0.9f, 1f, timer);
                imageLoadingBar.fillAmount = progress;
                textLoading.text = (progress * 100).ToString("F1") + "%";

                if (imageLoadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }
}
