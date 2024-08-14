using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    public TMP_Text damageTextPrefab; // �ؽ�Ʈ ������
    private float moveSpeed = 3f;
    private List<IEnumerator> coroutineQueue = new List<IEnumerator>();
    private Queue<TMP_Text> textPool = new Queue<TMP_Text>(); // TMP_Text ������Ʈ Ǯ

    private void Awake()
    {
        transform.localPosition = new Vector3(0f, 60f);
        // �ʱ� �ؽ�Ʈ ������Ʈ ����
        for (int i = 0; i < 10; i++)
        {
            CreateNewText();
        }
    }

    private TMP_Text CreateNewText()
    {
        TMP_Text newText = Instantiate(damageTextPrefab, transform);
        newText.gameObject.SetActive(false);
        textPool.Enqueue(newText);
        return newText;
    }

    public void StartShowTextsStatus(float _value, bool _isDamage)
    {
        string message = (_isDamage ? "-" : "+") + _value.ToString("F0");
        Color color = GetStatusColor(_isDamage);
        StartShowText(message, color);
    }

    private IEnumerator CoroutineTrigger()
    {
        yield return StartCoroutine(coroutineQueue[0]);
        coroutineQueue.RemoveAt(0);
        if (coroutineQueue.Count > 0)
            StartCoroutine(CoroutineTrigger());
    }

    private IEnumerator ShowText(string message, Color color)
    {
        TMP_Text textStatus = GetTextFromPool();
        textStatus.text = message;
        textStatus.color = color;
        StartCoroutine(MoveTextStatus(textStatus));
        yield return new WaitForSeconds(0.6f); // �ؽ�Ʈ�� ���������� �ö󰡵��� �ð����� �ݴϴ�.
    }

    private TMP_Text GetTextFromPool()
    {
        if (textPool.Count > 0)
        {
            TMP_Text text = textPool.Dequeue();
            text.gameObject.SetActive(true);
            return text;
        }
        else
        {
            // Ǯ�� �ؽ�Ʈ�� ���ٸ� ���� ����
            return CreateNewText();
        }
    }

    private IEnumerator MoveTextStatus(TMP_Text _textStatus)
    {
        _textStatus.transform.localPosition = Vector3.zero;

        float duration = 4f; // ��ü �ִϸ��̼� ���� �ð�
        float elapsedTime = 0f;

        float moveDistance = 1.5f;

        Color originalColor = _textStatus.color;
        Vector3 originalPosition = _textStatus.rectTransform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // ���� �� ���� (��ȭ �: �߰��� �� õõ�� ����)
            float alphaProgress = progress * progress * (3f - 2f * progress); // SmootherStep�� ������ ȿ��
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, alphaProgress);
            _textStatus.color = newColor;

            // �̵� �Ÿ� ����
            _textStatus.rectTransform.position = originalPosition + Vector3.up * moveDistance * progress;

            yield return null;
        }

        // �ִϸ��̼��� ���� �Ŀ� �ؽ�Ʈ ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ
        _textStatus.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        _textStatus.gameObject.SetActive(false);
        textPool.Enqueue(_textStatus); // ����� ���� �ؽ�Ʈ�� Ǯ�� ��ȯ
    }

    private Color GetStatusColor(bool _isDamage)
    {
        switch (_isDamage)
        {
            default:
                return new Color(1f, 0.27f, 0.27f);//FF4545
            case false:
                return new Color(0.27f, 1f, 0.39f);//45FF63
        }
    }
    public void StartShowText(string message, Color color)
    {
        // �� �ڷ�ƾ�� ��⿭�� �߰�
        coroutineQueue.Add(ShowText(message, color));

        // ��⿭�� ������� ������ �ڷ�ƾ ����
        if (coroutineQueue.Count == 1)
        {
            StartCoroutine(CoroutineTrigger());
        }
    }

}
