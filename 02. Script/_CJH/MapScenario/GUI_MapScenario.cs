using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GUI_MapScenario : MonoBehaviour
{
    public Transform CameraTrans;
    public float reachTime;
    public float coef_spd;
    public float timer;
    public Vector3 stdCamPosV3;
    public Material _ArriveMaterial, _ExitMaterial;
    public GameObject ClosedScene;
    public GameObject _ArriveMapScearioEffect;
    public GameObject _ExitMapScearioEffect;

    private void Awake()
    {
        stdCamPosV3 = CameraTrans.position;
    }

    public void SetInitState()
    {
        StartCoroutine(enterSceneCor());
    }

    public delegate void ProgressMap_preInput();

    public void moveCamFunc(Vector3 dstV3, ProgressMap_preInput progFunc)
    {
        dstV3.z = dstV3.y;
        dstV3 += stdCamPosV3;

        StartCoroutine(moveCam(dstV3, progFunc));
    }

    IEnumerator moveCam(Vector3 dstV3, ProgressMap_preInput func)
    {
        _ExitMaterial.SetFloat("_RevertCoex", 0);
        _ExitMaterial.SetFloat("_Seed", Random.Range(0.0f, 1.0f));
        _ExitMapScearioEffect.SetActive(true);
        ClosedScene.SetActive(false);

        Vector3 src = CameraTrans.position;
        timer = 0;
        while (true)
        {
            timer += Time.deltaTime;

            coef_spd = Mathf.Sin((timer / reachTime) * (Mathf.PI / 2));
            CameraTrans.position = Vector3.Lerp(src, dstV3, Mathf.Sin((timer / reachTime) * (Mathf.PI / 2)));
            _ExitMaterial.SetFloat("_RevertCoex", timer / reachTime);
            //Debug.Log(darkFog.GetFloat("_RevertCoex"));
            if (timer > reachTime)
                break;

            yield return null;
        }


        _ExitMapScearioEffect.SetActive(false);
        ClosedScene.SetActive(true);

        func();

        yield return null;
    }

    IEnumerator enterSceneCor()
    {
        // coef = 0

        timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
                break;

            yield return null;
        }
        _ArriveMaterial.SetFloat("_Seed", Random.Range(0.0f, 1.0f));
        _ArriveMaterial.SetFloat("_RevertCoex", 0);
        _ArriveMapScearioEffect.SetActive(true);
        ClosedScene.SetActive(false);

        timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            _ArriveMaterial.SetFloat("_RevertCoex", timer / reachTime);
            if (timer > reachTime)
                break;

            yield return null;
        }


        _ArriveMapScearioEffect.SetActive(false);

        // coef = 0
        yield return null;
    }
}
