using EnumCollection;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ScoreSlot : MonoBehaviour
{
    [SerializeField] ScoreType scoreType;
    [SerializeField] TMP_Text textType;
    [SerializeField] TMP_Text textScore;

    int fame;
    int num;

    public void SetScore(int _num, int _score)
    {
        textType.gameObject.SetActive(false);
        num = _num;
        fame = _score;

        string typeStr = string.Empty;
        switch (scoreType)
        {
            case ScoreType.Enemy:
                typeStr = (GameManager.language == Language.Ko) ? "처치한 적의 수" : "Enemies defeated";
                break;
            case ScoreType.Destination:
                typeStr = (GameManager.language == Language.Ko) ? "이동한 목적지" : "Destination reached";
                break;
            case ScoreType.Boss:
                typeStr = (GameManager.language == Language.Ko) ? "처치한 보스 수" : "Bosses defeated";
                break;
            case ScoreType.Food:
                typeStr = (GameManager.language == Language.Ko) ? "먹은 음식 수준" : "Quality of food eaten";
                break;
            case ScoreType.StageClear:
                typeStr = (GameManager.language == Language.Ko) ? "모든 스테이지 클리어" : "Clear All Stages";
                break;
        }
        if (_num != -1)
            typeStr += $" <size=40>({num})";
        textType.text = typeStr;
        textScore.text = fame.ToString();
    }

    public void ShowScore()
    {
        textType.gameObject.SetActive(true);
        StartCoroutine(AnimateScore(textType, true));  // 위치와 알파값 모두 애니메이션
        StartCoroutine(AnimateScore(textScore, false)); // 알파값만 애니메이션
    }




    private IEnumerator AnimateScore(TMP_Text _text, bool _animatePosition)
    {
        float animationDuration = 1f;
        float moveDistance = 30f;

        Vector3 initialPosition = _text.transform.localPosition - new Vector3(0f, moveDistance);
        Vector3 targetPosition = _text.transform.localPosition;

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



}