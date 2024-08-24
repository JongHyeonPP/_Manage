using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageTextUi : MonoBehaviour
{
    public Image imageBackground;
    public TMP_Text textInfo;
    readonly float showDuration = 1f;
    readonly float fadeDuration = 0.5f;

    public void StageStart()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // 1�� ���� ������
        yield return new WaitForSeconds(showDuration);

        // 0.5�� ���� ���̵� �ƿ�

        float fadeTimer = 0f;

        Color imageInitialColor = imageBackground.color;
        Color textInitialColor = textInfo.color;

        float initialImageAlpha = imageBackground.color.a;
        float initialTextAlpha = textInfo.color.a;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alphaFactor = fadeTimer / fadeDuration;

            float newImageAlpha = Mathf.Lerp(initialImageAlpha, 0f, alphaFactor);
            float newTextAlpha = Mathf.Lerp(initialTextAlpha, 0f, alphaFactor);

            imageBackground.color = new Color(imageInitialColor.r, imageInitialColor.g, imageInitialColor.b, newImageAlpha);
            textInfo.color = new Color(textInitialColor.r, textInitialColor.g, textInitialColor.b, newTextAlpha);

            yield return null;
        }

        // ������ �����ϰ� ����
        imageBackground.color = new Color(imageInitialColor.r, imageInitialColor.g, imageInitialColor.b, 0f);
        textInfo.color = new Color(textInitialColor.r, textInitialColor.g, textInitialColor.b, 0f);
        gameObject.SetActive(false);
    }
}
