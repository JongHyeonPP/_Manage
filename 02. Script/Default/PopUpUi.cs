using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUi : MonoBehaviour
{
    public TMP_Text textPopUp;
    public Image panel;
    private Coroutine currentCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    public IEnumerator SetContentCoroutine(string _content, string _emphasizeStr)
    {
        // 실행 중인 코루틴이 있으면 중지
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        if (_emphasizeStr != "")
        {
            // 강조할 부분의 태그 설정
            string emphasizedText = $"<color=#2F8DFF><size=120%>{_emphasizeStr}</size></color>";

            // _emphasizeStr을 emphasizedText로 대체
            string formattedContent = _content.Replace(_emphasizeStr, emphasizedText);

            // TMP_Text 컴포넌트에 설정
            textPopUp.text = formattedContent;
        }
        else
        {
            textPopUp.text = _content;
        }
        float targetPanelAlpha = 1f;
        // 텍스트와 패널의 기본 색상을 가져오고 알파값을 0으로 설정
        Color textOriginalColor = textPopUp.color;
        Color textColor = textOriginalColor;
        textColor.a = 0;
        textPopUp.color = textColor;

        Color panelOriginalColor = panel.color;
        Color panelColor = panelOriginalColor;
        panelColor.a = 0;
        panel.color = panelColor;


        // 0.3초 동안 알파값을 0에서 1로 변경
        float duration = 0.3f;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / duration);
            textColor.a = alpha;
            textPopUp.color = textColor;

            panelColor.a = Mathf.Lerp(0, targetPanelAlpha, t / duration); // 패널의 알파값은 0에서 0.4로 변경
            panel.color = panelColor;

            yield return null;
        }
        textColor.a = 1;
        textPopUp.color = textColor;

        panelColor.a = targetPanelAlpha; // 패널의 알파값을 0.4로 설정
        panel.color = panelColor;

        // 1초 동안 유지
        yield return new WaitForSeconds(1.0f);

        // 0.2초 동안 알파값을 1에서 0으로 변경
        duration = 0.2f;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / duration);
            textColor.a = alpha;
            textPopUp.color = textColor;

            panelColor.a = Mathf.Lerp(targetPanelAlpha, 0, t / duration); // 패널의 알파값은 0.4에서 0으로 변경
            panel.color = panelColor;

            yield return null;
        }
        textColor.a = 0;
        textPopUp.color = textColor;

        panelColor.a = 0;
        panel.color = panelColor;

        yield return new WaitForSeconds(1.0f);


        // 코루틴 종료
        currentCoroutine = null;
    }

    public IEnumerator SetContent(string _content, string _emphasizeStr)
    {
        gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(SetContentCoroutine(_content, _emphasizeStr));
        yield return currentCoroutine; // Wait for the coroutine to complete
        gameObject.SetActive(false);
    }
}
