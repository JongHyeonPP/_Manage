using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class StageScenario_1 : StageScenarioBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void ExtendVia(bool _isInstant)
    {
        float targetIntensity;
        float targetSmoothness;
        switch (phase)
        {
            default:
                targetIntensity = 0.478f;
                targetSmoothness = 0.489f;
                break;
            case 0:
                targetIntensity = 0.393f;
                targetSmoothness = 0.489f;
                break;
            case 1:
                targetIntensity = 0.32f;
                targetSmoothness = 0.32f;
                break;
            case 2:
                targetIntensity = 0.281f;
                targetSmoothness = 0.32f;
                break;
            case 3:
                targetIntensity = 0.295f;
                targetSmoothness = 0.303f;
                break;
            case 4:
                targetIntensity = 0.262f;
                targetSmoothness = 0.303f;
                break;
            case 5:
                targetIntensity = 0.222f;
                targetSmoothness = 0.3f;
                break;

        }
        if (_isInstant)
        {
            vignette.intensity.value = targetIntensity;
            vignette.smoothness.value = targetSmoothness;
        }
        else
            StartCoroutine(ExtendVignette(targetIntensity, targetSmoothness, 1f, Time.deltaTime));
    }

    public override void MoveMapVia(bool _isInstant)
    {
        float x = 0f;
        float y = 0f;
        switch (phase)
        {
            default:
                x = -217f;
                y = 220f;
                return;
            case -1:
                x = -200;
                y = 204f;
                break;
            case 0:
                x = -100f;
                y = 135.45f;
                break;
            case 1:
                x = 0f;
                y = 94.1f;
                break;
            case 2:
                x = 35.8f;
                y = 58.9f;
                break;
            case 3:
                x = 200;
                y = -115f;
                break;
            case 4:
                x = 299.7f;
                y = -225.73f;
                break;
            case 5:
                x = 299.7f;
                y = -236.2f;
                break;
        }
        if (_isInstant)
            stageCanvas.panelMap.transform.localPosition = new Vector3(x, y);
        else
            StartCoroutine(stageCanvas.MoveMapCoroutine(x, y, 2f));
    }

}
