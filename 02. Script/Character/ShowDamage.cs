using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    public TMP_Text damageTextPrefab; // 텍스트 프리팹
    private float moveSpeed = 3f;
    private List<IEnumerator> coroutineQueue = new List<IEnumerator>();
    private Queue<TMP_Text> textPool = new Queue<TMP_Text>(); // TMP_Text 오브젝트 풀

    private void Awake()
    {
        transform.localPosition = new Vector3(0f, 60f);
        // 초기 텍스트 오브젝트 생성
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
        yield return new WaitForSeconds(0.6f); // 텍스트가 순차적으로 올라가도록 시간차를 줍니다.
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
            // 풀에 텍스트가 없다면 새로 생성
            return CreateNewText();
        }
    }

    private IEnumerator MoveTextStatus(TMP_Text _textStatus)
    {
        _textStatus.transform.localPosition = Vector3.zero;

        float duration = 4f; // 전체 애니메이션 지속 시간
        float elapsedTime = 0f;

        float moveDistance = 1.5f;

        Color originalColor = _textStatus.color;
        Vector3 originalPosition = _textStatus.rectTransform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 알파 값 감소 (변화 곡선: 중간에 더 천천히 감소)
            float alphaProgress = progress * progress * (3f - 2f * progress); // SmootherStep과 유사한 효과
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, alphaProgress);
            _textStatus.color = newColor;

            // 이동 거리 조정
            _textStatus.rectTransform.position = originalPosition + Vector3.up * moveDistance * progress;

            yield return null;
        }

        // 애니메이션이 끝난 후에 텍스트 비활성화하고 풀에 반환
        _textStatus.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        _textStatus.gameObject.SetActive(false);
        textPool.Enqueue(_textStatus); // 사용이 끝난 텍스트를 풀에 반환
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
        // 새 코루틴을 대기열에 추가
        coroutineQueue.Add(ShowText(message, color));

        // 대기열이 비어있지 않으면 코루틴 실행
        if (coroutineQueue.Count == 1)
        {
            StartCoroutine(CoroutineTrigger());
        }
    }

}
