
using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class StageScenarioBase : MonoBehaviour
{
    public static StateInMap state;
    public static int stageNum;

    public Volume volume;
    protected Vignette vignette;


    public static StageBaseCanvas stageBaseCanvas;
    public static int phase;



    public GameObject canvasPrefab;
    public static List<object> nodes;//멤버는 node의 인덱스 int값
    public static string[] nodeTypes = new string[21];
    protected static float targetIntensity;
    protected static float targetSmoothness;
    protected virtual void Awake()
    {
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            Debug.Log("Vignette effect found!");
        }
        else
        {
            Debug.LogError("Vignette component not found in the volume profile.");
        }
        GameManager.mapScenario = this;
        GameObject remainCanvas = GameObject.FindWithTag("REMAINCANVAS");
        Destroy(remainCanvas);
        if (stageBaseCanvas == null)
        {
            MakeCanvas(stageNum);
        }
        if (nodes.Count == 0)
        {
            stageBaseCanvas.currentNode = stageBaseCanvas.startNode;
            phase = -1;
            ExtendVia(true);
            MoveCameraXVia(stageBaseCanvas.currentNode, true);
        }
        else
        {
            phase = nodes.Where(item=>item !=null).ToList().Count - 1;
            ExtendVia(true);
            MoveCameraXVia(stageBaseCanvas.currentNode, true);

        }

        stageBaseCanvas.gameObject.SetActive(true);
        volume.gameObject.SetActive(true);
        if (nodes.Contains(null))
        {
            nodes.RemoveAll(item => item == null);
            DataManager.dataManager.SetDocumentData("Nodes", nodes, "Progress", GameManager.gameManager.Uid);
           state = StateInMap.NeedPhase;
        }
        if (state == StateInMap.NeedPhase)
        {
            phase++;
            stageBaseCanvas.EnterPhase();
        }
        if (stageBaseCanvas.currentNode.buttonEnter)
            stageBaseCanvas.currentNode.buttonEnter.gameObject.SetActive(state == StateInMap.NeedEnter);
    }
    public static StageBaseCanvas MakeCanvas(int stageNum)
    {
        GameObject canvasGameObj = Instantiate(GameManager.gameManager.stageBaseCanvases[stageNum]);
        stageBaseCanvas = canvasGameObj.GetComponent<StageBaseCanvas>();
        DontDestroyOnLoad(canvasGameObj);
        if (nodes.Count > 0)
            stageBaseCanvas.SetLoadedNode();
        return stageBaseCanvas;
    }

    [ContextMenu("NextPhase")]
    public void NextPhase()
    {
        phase++;
        stageBaseCanvas.EnterPhase();
    }

    public abstract void  ExtendVia(bool _isInstant);

    public abstract void MoveCameraXVia(DestinationNode _to, bool _isInstant);

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



    protected IEnumerator MoveCameraCoroutine( float targetX, float targetY, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = Camera.main.transform.position;

        while (elapsedTime < duration)
        {
            // 시간 경과에 따른 비율 계산
            float t = elapsedTime / duration;

            // Lerp를 사용하여 점진적으로 x좌표 변경
            float newX = Mathf.Lerp(startPos.x, targetX, t);
            float newY = Mathf.Lerp(startPos.y, targetY, t);
            // 새로운 위치로 이동
            Camera.main.transform.localPosition = new Vector3(newX,newY, Camera.main.transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 설정
        Camera.main.transform.localPosition = new Vector3(targetX,targetY, Camera.main.transform.localPosition.z);
    }
}

