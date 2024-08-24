
using EnumCollection;
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
    public static List<object> nodes { get; set; }//멤버는 node의 인덱스 int값
    public static string[] nodeTypes = new string[21];
    protected static float targetIntensity;
    protected static float targetSmoothness;
    [SerializeField] Camera overlayCamera;
    [SerializeField] StageTextUi stageTextUi;

    protected virtual void Awake()
    {

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
            ///PhaseSet
            if (nodes.Count == 0)
            {
                phase = -1;

            }
            else
            {
                phase = nodes.Where(item => item != null).Where(item=>item is not string).Count() - 1;
            }
            MakeCanvas(stageNum);

            //CanvasSet
            CanvasScaler canvasScaler = stageCanvas.GetComponent<CanvasScaler>();


            if (canvasScaler != null)
            {
                // UI Scale Mode 설정
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

                // Reference Resolution 설정
                canvasScaler.referenceResolution = new Vector2(1200, 600);

                // Screen Match Mode 설정
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

                // Match 설정 (0 = Width, 1 = Height)
                canvasScaler.matchWidthOrHeight = 0f;

                // Reference Pixels Per Unit 설정
                canvasScaler.referencePixelsPerUnit = 100f;
            }
            stageCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            if (nodes.Count > 0)
            {
                stageCanvas.SetLoadedNode();

            }
            else
            {
                stageTextUi.gameObject.SetActive(true);
                stageTextUi.StageStart();
            }
            stageCanvas.SetCharacterToCurrentNode();


        }
        else
            stageCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        stageCanvas.gameObject.SetActive(true);


        //if (nodes.Contains(null))
        //{
        //    nodes.RemoveAll(item => item == null);
        //    DataManager.dataManager.SetDocumentData("Nodes", nodes, "Progress", GameManager.gameManager.Uid);
        //    state = StateInMap.NeedPhase;
        //}
        MoveMapVia(true);
        ExtendVia(true);
        if (state == StateInMap.NeedPhase)
        {
            if (phase == 5)
            {
                Debug.Log("Stage Clear");
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

        // 먼저 카메라 스택에서 해당 오버레이 카메라를 제거
        if (baseCameraData.cameraStack.Contains(overlayCamera))
        {
            baseCameraData.cameraStack.Remove(overlayCamera);
        }

        // 그런 다음 카메라 스택의 가장 마지막에 다시 추가 (즉, 가장 위로 올림)
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
        // 현재 Vignette의 intensity와 smoothness 값을 저장합니다.
        float currentIntensity = vignette.intensity.value;
        float currentSmoothness = vignette.smoothness.value;

        // 경과 시간을 추적합니다.
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 경과 시간을 업데이트합니다.
            elapsedTime += interval;

            // Lerp 함수를 사용하여 현재 값을 목표 값으로 점진적으로 변경합니다.
            vignette.intensity.value = Mathf.Lerp(currentIntensity, targetIntensity, elapsedTime / duration);
            vignette.smoothness.value = Mathf.Lerp(currentSmoothness, targetSmoothness, elapsedTime / duration);

            // 다음 업데이트까지 기다립니다.
            yield return new WaitForSeconds(interval);
        }

        // 최종 목표 값으로 설정합니다.
        vignette.intensity.value = targetIntensity;
        vignette.smoothness.value = targetSmoothness;
    }




    public abstract void ExtendVia(bool _isInstant);

    public abstract void MoveMapVia(bool _isInstant);
}

