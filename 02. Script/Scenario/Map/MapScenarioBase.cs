
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

public abstract class MapScenarioBase : MonoBehaviour
{
    public static StateInMap state;
    public static int stageNum;

    public Volume volume;
    protected Vignette vignette;


    public static StageBaseCanvas stageBaseCanvas;
    public static int phase;



    public GameObject canvasPrefab;
    public static List<object> nodes;//����� node�� �ε��� int��
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
            phase = 0;
            ExtendVia(true);
            MoveCameraXVia(stageBaseCanvas.currentNode, true);
        }
        else
        {
            phase = nodes.Count - 1;
            ExtendVia(true);
            MoveCameraXVia(stageBaseCanvas.currentNode, true);
            phase++;
        }

        stageBaseCanvas.gameObject.SetActive(true);
        volume.gameObject.SetActive(true);

        if (state == StateInMap.NeedPhase)
            stageBaseCanvas.EnterPhase();

    }
    public static void MakeCanvas(int stageNum)
    {
        GameObject canvasGameObj = Instantiate(GameManager.gameManager.stageBaseCanvases[stageNum]);
        stageBaseCanvas = canvasGameObj.GetComponent<StageBaseCanvas>();
        DontDestroyOnLoad(canvasGameObj);
        if (nodes.Count > 0)
            stageBaseCanvas.SetLoadedNode();
    }


    [ContextMenu("NextPhase")]
    public void NextPhase()
    {
        phase++;
        stageBaseCanvas. EnterPhase();
    }





    public abstract void  ExtendVia(bool _isInstant);



    public abstract void MoveCameraXVia(DestinationNode _to, bool _isInstant);

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



    protected IEnumerator MoveCameraCoroutine( float targetX, float targetY, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = Camera.main.transform.position;

        while (elapsedTime < duration)
        {
            // �ð� ����� ���� ���� ���
            float t = elapsedTime / duration;

            // Lerp�� ����Ͽ� ���������� x��ǥ ����
            float newX = Mathf.Lerp(startPos.x, targetX, t);
            float newY = Mathf.Lerp(startPos.y, targetY, t);
            // ���ο� ��ġ�� �̵�
            Camera.main.transform.localPosition = new Vector3(newX,newY, Camera.main.transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ ����
        Camera.main.transform.localPosition = new Vector3(targetX,targetY, Camera.main.transform.localPosition.z);
    }
}

