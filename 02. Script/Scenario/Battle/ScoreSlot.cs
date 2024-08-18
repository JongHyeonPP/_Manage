using EnumCollection;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreSlot : MonoBehaviour
{
    [SerializeField] ScoreType scoreType;
    [SerializeField] TMP_Text textType;
    [SerializeField] TMP_Text textScore;

    int score;
    int num;

    public void SetScore(int _num, int _score)
    {
        textType.gameObject.SetActive(false);
        num = _num;
        score = _score;

        string typeStr = string.Empty;
        switch (scoreType)
        {
            case ScoreType.Enemy:
                typeStr = (GameManager.language == Language.Ko) ? "처치한 적의 수" : "Number of enemies defeated";
                break;
            case ScoreType.Destination:
                typeStr = (GameManager.language == Language.Ko) ? "이동한 목적지" : "Destination reached";
                break;
            case ScoreType.Boss:
                typeStr = (GameManager.language == Language.Ko) ? "처치한 보스 수" : "Number of bosses defeated";
                break;
            case ScoreType.Food:
                typeStr = (GameManager.language == Language.Ko) ? "먹은 음식 수준" : "Quality of food eaten";
                break;
        }
        typeStr += $" ({num})";
        textType.text = typeStr;
        textScore.text = score.ToString();
    }

    public void ShowScore()
    {
        textType.gameObject.SetActive(true);
        StartCoroutine(AnimateScore(textType, true));  // 위치와 알파값 모두 애니메이션
        StartCoroutine(AnimateScore(textScore, false)); // 알파값만 애니메이션
    }

    public void SetTotal(int _score)
    {
        textType.gameObject.SetActive(false);
        score = _score;
        textType.text = (GameManager.language == Language.Ko) ? "점수" : "Score";
        textScore.text = score.ToString();
    }
    public IEnumerator ShowTotal()
    {
        textType.gameObject.SetActive(true);
        textScore.gameObject.SetActive(false);
        yield return StartCoroutine(AnimateScore(textType, false));
        yield return StartCoroutine(AnimateTotal());
    }

    private IEnumerator AnimateScore(TMP_Text _text, bool _animatePosition)
    {
        float animationSpeed = 1.5f; // 애니메이션 속도
        float moveDistance = 30.0f; // 텍스트가 위로 이동할 거리
        Vector3 initialPosition = _text.transform.localPosition;
        Vector3 targetPosition = initialPosition;

        if (_animatePosition)
        {
            // 목표 위치 아래쪽으로 초기 위치 설정
            initialPosition = targetPosition - Vector3.up * moveDistance;
            // 애니메이션 시작 위치 설정
            _text.transform.localPosition = initialPosition;
        }

        Color initialColor = _text.color;
        initialColor.a = 0;
        Color targetColor = _text.color;
        targetColor.a = 1;

        // 초기 색상 설정
        _text.color = initialColor;

        float elapsed = 0f;
        float animationDuration = 1.0f / animationSpeed; // 애니메이션 지속 시간을 속도로 설정

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // 알파값을 선형 보간
            _text.color = Color.Lerp(initialColor, targetColor, t);

            if (_animatePosition)
            {
                // 필요시 위치를 선형 보간
                _text.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            }

            yield return null;
        }

        // 최종 값이 설정되었는지 확인
        _text.color = targetColor;

        if (_animatePosition)
        {
            _text.transform.localPosition = targetPosition;
        }
    }
    private IEnumerator AnimateTotal()
    {
        float currentScore = 0;
        float duration = 1.0f; // 애니메이션을 1초 동안 실행
        float elapsed = 0f;
        textScore.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            currentScore = Mathf.Lerp(0, score, t);

            // 텍스트 업데이트
            textScore.text = Mathf.FloorToInt(currentScore).ToString();

            yield return null;
        }

        // 최종 값 설정
        textScore.text = score.ToString();
    }

}
