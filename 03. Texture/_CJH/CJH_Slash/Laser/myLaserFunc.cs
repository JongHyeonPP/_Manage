using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.ParticleSystem;

public class myLaserFunc : MonoBehaviour
{
    public LineRenderer lr;
    public Transform _src, _dst;

    public float totalTime;
    public float powerCoef;
    // Start is called before the first frame update Bolder_thickness
    [ContextMenu("test")]
    public void test()
    {
        Transform temp = lr.transform;
        temp.position = _src.position;
        temp.right = (_dst.position - _src.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            myPlay();
        }
    }

    [ContextMenu("TestPlay")]
    public void myPlay()
    {
        StartCoroutine(moveBlockTime());
    }

    private IEnumerator moveBlockTime()
    {
        LineRenderer tempLr = Instantiate(lr);
        float lifeTime = totalTime;
      
        Transform temp = tempLr.transform;
        temp.position = _src.position;
        temp.right = (_dst.position - _src.position);
        yield return null;

        float elapsedTime = 0.0f;

        while (true)
        {
            yield return null;

            if (elapsedTime > lifeTime) break;

            tempLr.startColor = (lr.startColor * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, 1) * (Mathf.Pow((elapsedTime / lifeTime), 1/powerCoef)));
            elapsedTime += Time.deltaTime;
        }
        elapsedTime = 0.0f;
        while (true)
        {
            yield return null;

            if (elapsedTime > lifeTime) break;

            tempLr.startColor = (lr.startColor * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, 1) * (1 - Mathf.Pow((elapsedTime / lifeTime), powerCoef)));
            elapsedTime += Time.deltaTime;
        }

        Destroy(tempLr.gameObject);
    }
}