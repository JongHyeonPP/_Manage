using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class MapScenario_2 : MapScenarioBase
{
    private void Update()
    {
        
    }
    protected override void Awake()
    {
        base.Awake();
        stageNum = 0;
    }

    public override void ExtendVia(bool _isInstant)
    {
        switch (phase)
        {
            default:
                targetIntensity = 0.526f;
                targetSmoothness = 0.639f;
                break;
            case 1:
                targetIntensity = 0.446f;
                targetSmoothness = 0.261f;
                break;
            case 2:
                targetIntensity = 0.4f;
                targetSmoothness = 0.248f;
                break;
            case 3:
                targetIntensity = 0.371f;
                targetSmoothness = 0.195f;
                break;
            case 4:
                targetIntensity = 0.295f;
                targetSmoothness = 0.303f;
                break;
            case 5:
                targetIntensity = 0.262f;
                targetSmoothness = 0.303f;
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

    public override void MoveCameraXVia(DestinationNode _to, bool _isInstant)
    {
        float x;
        float y = _to.transform.position.y / 4;
        switch(_to.phaseNum)
        {
            default:
                return;
            case 0:
                x = 2f;
                
                break;
            case 1:
                x = 0.62f;
                break;
            case 2:
                x = -1.5f;
                break;
            case 3:
                x = -3.5f;
                break;
            case 4:
                x = -4.5f;
                break;
            case 5:
                x = -5.5f;
                break;
            case 6:
                x = -6.5f;
                break;
        }
        if(!_isInstant)
        StartCoroutine(MoveCameraCoroutine(x, y, 2f));
        else
            Camera.main.transform.localPosition = new Vector3(x, y, Camera.main.transform.localPosition.z);
    }
}
