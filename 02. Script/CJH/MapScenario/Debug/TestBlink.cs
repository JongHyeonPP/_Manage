using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class TestBlink : MonoBehaviour
{
    public List<Transform> childList = new();
    public List<Transform> controlling = new();
    public Transform RoadObj;
    public int count;
    public float stdSize;
    public float gap;
    private IEnumerator coroutine;

    private void Start()
    {
        int childCount = RoadObj.childCount;

        // 모든 직계 자식의 Transform을 리스트에 추가
        for (int i = 0; i < childCount; i++)
        {
            childList.Add(RoadObj.GetChild(i).GetChild(1));
            childList[i].localScale *= 0;
        }
    }

    public void click()
    {
        // 코루틴 실행
        StartCoroutine(HelloCoroutine(0));
    }
    public void click2()
    {
        // 코루틴 실행
        StartCoroutine(HelloCoroutine2(5));
    }

    private IEnumerator HelloCoroutine(int index)
    {
        { 

            Debug.Log("Clicked - " + index);
            Transform myTarget = childList[index];
            bool isOver = false;
            while (true)
            {
                if (myTarget.localScale.x < stdSize)
                    myTarget.localScale += Vector3.one * Time.deltaTime * count;
                else
                {
                    break;
                }

                if (!isOver && myTarget.localScale.x > gap)
                {
                    StartCoroutine(HelloCoroutine(index + 1));
                    isOver = true;
                }

                // 3초 대기
                yield return new WaitForSeconds(0.05f);
            }

            Debug.Log("Yes - " + index);
        }
    }
    private IEnumerator HelloCoroutine2(int index)
    {

        {

            Debug.Log("Clicked - " + index);
            Transform myTarget = childList[index];
            bool isOver = false;
            while (true)
            {
                if (myTarget.localScale.x < stdSize)
                    myTarget.localScale += Vector3.one * Time.deltaTime * count;
                else
                {
                    break;
                }

                if (!isOver && myTarget.localScale.x > gap)
                {
                    StartCoroutine(HelloCoroutine(index + 1));
                    isOver = true;
                }

                // 3초 대기
                yield return new WaitForSeconds(0.05f);
            }

            Debug.Log("Yes - " + index);
        }
    }
}
