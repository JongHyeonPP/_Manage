using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveGaugeBar : MonoBehaviour
{
    public Image imageBar;
    public Image imageFill;
    public Image imageFrame;
    public float moveGuage;
    readonly float guageFillTime= 10f;
    public void StartFillGauge()
    {
        StopAllCoroutines();
        StartCoroutine(FillGaugeCoroutine());
    }

    private IEnumerator FillGaugeCoroutine()
    {
        // UI ��ҵ��� ���̵� ���� �����մϴ�.
        StartCoroutine(GameManager.FadeUi(imageBar, 0.5f, true, 0.7f));
        StartCoroutine(GameManager.FadeUi(imageFill, 0.5f, true, 0.7f));
        StartCoroutine(GameManager.FadeUi(imageFrame, 0.5f, true, 0.7f));

        // fillAmount�� 1�� ���� 0���� 1�� ������Ű�� �κ��Դϴ�.
        float duration = guageFillTime;
        float elapsed = 0f;

        // ���� �� fillAmount�� 0���� �����մϴ�.
        imageFill.fillAmount = moveGuage = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            imageFill.fillAmount= moveGuage = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // ���������� fillAmount�� 1�� ������ 1�� �ڿ� ������ ä�������� �մϴ�.
        imageFill.fillAmount = moveGuage = 1f;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GameManager.FadeUi(imageBar, 0.5f, false));
        StartCoroutine(GameManager.FadeUi(imageFill, 0.5f, false));
        StartCoroutine(GameManager.FadeUi(imageFrame, 0.5f, false));
    }
}
