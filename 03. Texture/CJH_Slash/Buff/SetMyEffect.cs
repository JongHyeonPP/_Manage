using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMyEffect : MonoBehaviour
{
    public GameObject _effect;
    public Transform _src, _dst;
    public float totalTime;
    public float coef;
    [ContextMenu("TestPlay")]
    public void myPlay()
    {
        StartCoroutine(moveBlockTime());
    }

    private IEnumerator moveBlockTime()
    {
        Transform tempLr = Instantiate(_effect, _src.position,Quaternion.identity).transform;
        float lifeTime = totalTime;

        yield return null;

        float elapsedTime = 0.0f;

        while (true)
        {
            yield return null;

            if (elapsedTime > lifeTime) break;

            tempLr.position = Vector3.Lerp(_src.position, _dst.position, Mathf.Pow((elapsedTime / lifeTime), coef));
            elapsedTime += Time.deltaTime;
        }
    }
}
