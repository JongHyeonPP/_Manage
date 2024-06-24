using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MapScenario_2 : MapScenarioBase
{
    public int phaseNum;
    protected override void InitComposition()
    {
        Camera.main.transform.position = new Vector3(2.18f, -0.38f, -10f);
        Camera.main.orthographicSize = 4.26f;

    }
    [ContextMenu("Phase1")]
    public void Phase1()
    {
        EnterPhase(nodePhase_1);
    }
    [ContextMenu("Phase2")]
    public void Phase2()
    {
        EnterPhase(nodePhase_2);
    }
    [ContextMenu("Phase3")]
    public void Phase3()
    {
        EnterPhase(nodePhase_3);
    }

    protected override Coroutine NextVignette(int _phase)
    {
        float vignetteFrom = 0f;
        float vignetteTo = 0f;
        float smoothness = 0f;
        switch (_phase)
        {
            case 1:
                vignetteFrom = 0.4f;
                vignetteTo = 0.43f;
                smoothness = 0.6f;
                break;
            case 2:
                vignetteFrom = 0.35f;
                vignetteTo = 0.36f;
                smoothness = 0.375f;
                break;
        }
        return StartCoroutine(OscillateVignetteIntensity(vignetteFrom, vignetteTo, 2f,smoothness));
    }
}
