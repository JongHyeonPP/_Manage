using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationNode : MonoBehaviour
{
    public Transform nodeDot;
    public int index { get; set; }
    Image[] images;
    TMP_Text text;

    public void HideAlpha()
    {
        images = GetComponentsInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
        foreach (var x in images)
        {
            x.color = new Color(1f, 1f, 1f, 0f);
        }
        if (text)
            text.color = new Color(1f, 1f, 1f, 0f);
    }

    public IEnumerator GraduallyAscendAlpha()
    {
        float curAlpha = 0f;
        float totalTime = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            curAlpha = Mathf.Lerp(0f, 1f, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            foreach (var x in images)
            {
                x.color = new Color(1f, 1f, 1f, curAlpha);
            }
            if (text)
                text.color = new Color(1f, 1f, 1f, curAlpha);
            yield return null; // 한 프레임 대기
        }
        curAlpha = 1f;
        foreach (var x in images)
        {
            x.color = new Color(1f, 1f, 1f, curAlpha);
        }
        if (text)
            text.color = new Color(1f, 1f, 1f, curAlpha);
    }
    public void OnNodeClicked()
    {
        if (GameManager.mapScenario.isMove)
            return;
        GameManager.mapScenario.CharacterMove(this);
        GameManager.mapScenario.currentNode = this;
        GameManager.mapScenario.isMove = true;
    }
}
