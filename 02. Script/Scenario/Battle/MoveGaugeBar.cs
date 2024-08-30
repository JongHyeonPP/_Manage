using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveGaugeBar : MonoBehaviour
{
    public Image imageBar;
    public Image imageFill;
    public Image imageFrame;
    public float moveGuage;
    readonly float guageFillTime = 5f;
    private bool isFadingOut = false; // FadeOut 상태를 추적하는 플래그

    public void StartFillGauge()
    {
        StopAllCoroutines();
        StartCoroutine(FillGaugeCoroutine());
    }

    private IEnumerator FillGaugeCoroutine()
    {
        // UI 요소들의 페이드 인을 시작합니다.
        StartCoroutine(GameManager.FadeUi(imageBar, 0.5f, true, 0.7f));
        StartCoroutine(GameManager.FadeUi(imageFill, 0.5f, true, 0.7f));
        StartCoroutine(GameManager.FadeUi(imageFrame, 0.5f, true, 0.7f));

        // fillAmount를 1초 동안 0에서 1로 증가시키는 부분입니다.
        float duration = guageFillTime;
        float elapsed = 0f;

        // 시작 시 fillAmount를 0으로 설정합니다.
        imageFill.fillAmount = moveGuage = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            imageFill.fillAmount = moveGuage = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // 마지막으로 fillAmount를 1로 설정해 1초 뒤에 완전히 채워지도록 합니다.
        imageFill.fillAmount = moveGuage = 1f;
        yield return new WaitForSeconds(0.5f);
        FadeOut();
    }

    public void FadeOut()
    {
        if (isFadingOut) return; // 이미 FadeOut이 진행 중이라면 중단

        isFadingOut = true; // FadeOut이 시작됨을 표시
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        StartCoroutine(GameManager.FadeUi(imageBar, 0.5f, false));
        StartCoroutine(GameManager.FadeUi(imageFill, 0.5f, false));
        StartCoroutine(GameManager.FadeUi(imageFrame, 0.5f, false));

        // FadeOut이 끝난 후 플래그를 초기화
        yield return new WaitForSeconds(0.5f);
        isFadingOut = false;
    }
}