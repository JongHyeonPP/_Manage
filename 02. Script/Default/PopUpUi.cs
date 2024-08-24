using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUi : MonoBehaviour
{
    public TMP_Text textPopUp;
    public Image panel;
    private Coroutine currentCoroutine; // ���� ���� ���� �ڷ�ƾ�� ������ ����

    public IEnumerator SetContentCoroutine(string _content, string _emphasizeStr)
    {
        // ���� ���� �ڷ�ƾ�� ������ ����
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        if (_emphasizeStr != "")
        {
            // ������ �κ��� �±� ����
            string emphasizedText = $"<color=#2F8DFF><size=120%>{_emphasizeStr}</size></color>";

            // _emphasizeStr�� emphasizedText�� ��ü
            string formattedContent = _content.Replace(_emphasizeStr, emphasizedText);

            // TMP_Text ������Ʈ�� ����
            textPopUp.text = formattedContent;
        }
        else
        {
            textPopUp.text = _content;
        }
        float targetPanelAlpha = 1f;
        // �ؽ�Ʈ�� �г��� �⺻ ������ �������� ���İ��� 0���� ����
        Color textOriginalColor = textPopUp.color;
        Color textColor = textOriginalColor;
        textColor.a = 0;
        textPopUp.color = textColor;

        Color panelOriginalColor = panel.color;
        Color panelColor = panelOriginalColor;
        panelColor.a = 0;
        panel.color = panelColor;


        // 0.3�� ���� ���İ��� 0���� 1�� ����
        float duration = 0.3f;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / duration);
            textColor.a = alpha;
            textPopUp.color = textColor;

            panelColor.a = Mathf.Lerp(0, targetPanelAlpha, t / duration); // �г��� ���İ��� 0���� 0.4�� ����
            panel.color = panelColor;

            yield return null;
        }
        textColor.a = 1;
        textPopUp.color = textColor;

        panelColor.a = targetPanelAlpha; // �г��� ���İ��� 0.4�� ����
        panel.color = panelColor;

        // 1�� ���� ����
        yield return new WaitForSeconds(1.0f);

        // 0.2�� ���� ���İ��� 1���� 0���� ����
        duration = 0.2f;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / duration);
            textColor.a = alpha;
            textPopUp.color = textColor;

            panelColor.a = Mathf.Lerp(targetPanelAlpha, 0, t / duration); // �г��� ���İ��� 0.4���� 0���� ����
            panel.color = panelColor;

            yield return null;
        }
        textColor.a = 0;
        textPopUp.color = textColor;

        panelColor.a = 0;
        panel.color = panelColor;

        yield return new WaitForSeconds(1.0f);


        // �ڷ�ƾ ����
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
