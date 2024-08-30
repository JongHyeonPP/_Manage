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
                typeStr = (GameManager.language == Language.Ko) ? "óġ�� ���� ��" : "Enemies defeated";
                break;
            case ScoreType.Destination:
                typeStr = (GameManager.language == Language.Ko) ? "�̵��� ������" : "Destination reached";
                break;
            case ScoreType.Boss:
                typeStr = (GameManager.language == Language.Ko) ? "óġ�� ���� ��" : "Bosses defeated";
                break;
            case ScoreType.Food:
                typeStr = (GameManager.language == Language.Ko) ? "���� ���� ����" : "Quality of food eaten";
                break;
            case ScoreType.StageClear:
                typeStr = (GameManager.language == Language.Ko) ? "��� �������� Ŭ����" : "Clear All Stages";
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
        StartCoroutine(AnimateScore(textType, true));  // ��ġ�� ���İ� ��� �ִϸ��̼�
        StartCoroutine(AnimateScore(textScore, false)); // ���İ��� �ִϸ��̼�
    }




    private IEnumerator AnimateScore(TMP_Text _text, bool _animatePosition)
    {
        float animationDuration = 1f;
        float moveDistance = 30f;

        Vector3 initialPosition = _text.transform.localPosition - new Vector3(0f, moveDistance);
        Vector3 targetPosition = _text.transform.localPosition;

        if (_animatePosition)
        {
            // ��ǥ ��ġ �Ʒ������� �ʱ� ��ġ ����
            initialPosition = targetPosition - Vector3.up * moveDistance;
            // �ִϸ��̼� ���� ��ġ ����
            _text.transform.localPosition = initialPosition;
        }

        Color initialColor = _text.color;
        initialColor.a = 0;
        Color targetColor = _text.color;
        targetColor.a = 1;

        // �ʱ� ���� ����
        _text.color = initialColor;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // ���İ��� ���� ����
            _text.color = Color.Lerp(initialColor, targetColor, t);

            if (_animatePosition)
            {
                // �ʿ�� ��ġ�� ���� ����
                _text.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            }

            yield return null;
        }

        // ���� ���� �����Ǿ����� Ȯ��
        _text.color = targetColor;

        if (_animatePosition)
        {
            _text.transform.localPosition = targetPosition;
        }
    }



}