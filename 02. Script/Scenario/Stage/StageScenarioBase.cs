
using EnumCollection;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public abstract class StageScenarioBase : MonoBehaviour
{
    public static StateInMap state;
    public static int stageNum;

    public Volume volume;
    protected Vignette vignette;

    public static StageCanvas stageCanvas { get; set; }
    public static int phase;
    public LootExplain lootExplain;

    //public GameObject canvasPrefab;
    public static List<object> nodes { get; set; }//����� node�� �ε��� int��
    public static string[] nodeTypes = new string[21];

    [SerializeField] Camera overlayCamera;
    [SerializeField] StageTextUi stageTextUi;
    protected virtual async void Awake()
    {
        GameManager.gameManager.phaseProgressUi.SetText();
        //Ui
        lootExplain.gameObject.SetActive(false);
        stageTextUi.gameObject.SetActive(false);
        volume.gameObject.SetActive(true);
        //Vignette
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            Debug.Log("Vignette effect found!");
        }
        else
        {
            Debug.LogError("Vignette component not found in the volume profile.");
        }
        //MakeNode
        GameManager.stageScenario = this;
        if (stageCanvas == null)
        {
            phase = nodes.Where(item => item != null).Where(item => item is not string).Count() - 1;
            MakeCanvas(stageNum);

            //CanvasSet
            CanvasScaler canvasScaler = stageCanvas.GetComponent<CanvasScaler>();


            if (canvasScaler != null)
            {
                // UI Scale Mode ����
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

                // Reference Resolution ����
                canvasScaler.referenceResolution = new Vector2(1200, 600);

                // Screen Match Mode ����
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

                // Match ���� (0 = Width, 1 = Height)
                canvasScaler.matchWidthOrHeight = 0f;

                // Reference Pixels Per Unit ����
                canvasScaler.referencePixelsPerUnit = 100f;
            }
            stageCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            if (nodes.Count > 0)
            {
                stageCanvas.SetLoadedNode();

            }
            else
            {
                stageTextUi.StageStart();
            }

            stageCanvas.SetCharacterToCurrentNode();


        }
        else
            stageCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        stageCanvas.gameObject.SetActive(true);
        while (nodes.Contains(null))
            nodes.Remove(null);
        MoveMapVia(true);
        ExtendVia(true);
        if (state == StateInMap.NeedPhase)
        {
            if (stageCanvas.currentNode.imageTrigger)
                stageCanvas.currentNode.imageTrigger.SetActive(false);
            if (phase == 5)
            {
                GameManager.gameManager.bossNum++;
                Dictionary<string, object> docDict = new()
            {
                { "BossNum", GameManager.gameManager.bossNum }
            };
                DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.uid);
                stageTextUi.StageClear();
            }
            else
            {
                stageCanvas.NextDestination();
            }


        }
        if (stageCanvas.currentNode.buttonEnter)
            stageCanvas.currentNode.buttonEnter.gameObject.SetActive(state == StateInMap.NeedEnter);
    }
    private void Start()
    {
        var baseCameraData = Camera.main.GetUniversalAdditionalCameraData();

        var overlayCameraData = overlayCamera.GetUniversalAdditionalCameraData();

        // ���� ī�޶� ���ÿ��� �ش� �������� ī�޶� ����
        if (baseCameraData.cameraStack.Contains(overlayCamera))
        {
            baseCameraData.cameraStack.Remove(overlayCamera);
        }

        // �׷� ���� ī�޶� ������ ���� �������� �ٽ� �߰� (��, ���� ���� �ø�)
        baseCameraData.cameraStack.Add(overlayCamera);
    }
    public static void MakeCanvas(int stageNum)
    {
        GameObject canvasGameObj = Instantiate(GameManager.gameManager.stageBaseCanvases[stageNum]);
        stageCanvas = canvasGameObj.GetComponent<StageCanvas>();
        DontDestroyOnLoad(canvasGameObj);
    }



    public IEnumerator ExtendVignette(float targetIntensity, float targetSmoothness, float duration, float interval)
    {
        // ���� Vignette�� intensity�� smoothness ���� �����մϴ�.
        float currentIntensity = vignette.intensity.value;
        float currentSmoothness = vignette.smoothness.value;

        // ��� �ð��� �����մϴ�.
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ��� �ð��� ������Ʈ�մϴ�.
            elapsedTime += interval;

            // Lerp �Լ��� ����Ͽ� ���� ���� ��ǥ ������ ���������� �����մϴ�.
            vignette.intensity.value = Mathf.Lerp(currentIntensity, targetIntensity, elapsedTime / duration);
            vignette.smoothness.value = Mathf.Lerp(currentSmoothness, targetSmoothness, elapsedTime / duration);

            // ���� ������Ʈ���� ��ٸ��ϴ�.
            yield return new WaitForSeconds(interval);
        }

        // ���� ��ǥ ������ �����մϴ�.
        vignette.intensity.value = targetIntensity;
        vignette.smoothness.value = targetSmoothness;
    }
    public void ResetStageSetting()
    {
        
    }



    public abstract void ExtendVia(bool _isInstant);

    public abstract void MoveMapVia(bool _isInstant);
}

