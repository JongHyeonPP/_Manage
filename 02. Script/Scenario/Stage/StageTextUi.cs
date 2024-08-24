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
        // 1초 동안 보여줌
        yield return new WaitForSeconds(showDuration);

        // 0.5초 동안 페이드 아웃

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

        // 완전히 투명하게 설정
        imageBackground.color = new Color(imageInitialColor.r, imageInitialColor.g, imageInitialColor.b, 0f);
        textInfo.color = new Color(textInitialColor.r, textInitialColor.g, textInitialColor.b, 0f);
        gameObject.SetActive(false);
    }
}
