using EnumCollection;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

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
                typeStr = (GameManager.language == Language.Ko) ? "óġ�� ���� ��" : "Number of enemies defeated";
                break;
            case ScoreType.Destination:
                typeStr = (GameManager.language == Language.Ko) ? "�̵��� ������" : "Destination reached";
                break;
            case ScoreType.Boss:
                typeStr = (GameManager.language == Language.Ko) ? "óġ�� ���� ��" : "Number of bosses defeated";
                break;
            case ScoreType.Food:
                typeStr = (GameManager.language == Language.Ko) ? "���� ���� ����" : "Quality of food eaten";
                break;
        }
        typeStr += $" ({num})";
        textType.text = typeStr;
        textScore.text = fame.ToString();
    }

    public void ShowScore()
    {
        textType.gameObject.SetActive(true);
        StartCoroutine(AnimateScore(textType, true));  // ��ġ�� ���İ� ��� �ִϸ��̼�
        StartCoroutine(AnimateScore(textScore, false)); // ���İ��� �ִϸ��̼�
    }

    public async Task SetFame(int _fame)
    {
        textType.gameObject.SetActive(false);
        fame = _fame;
        int fameAscend = GameManager.battleScenario.fameAscend
        textType.text = (GameManager.language == Language.Ko) ? "ȹ���� ��" : "Gain Reputation";
        textScore.text = fame.ToString();
        GameManager.gameManager.fame += fame;
        await DataManager.dataManager.SetDocumentData("Fame", GameManager.gameManager.fame, "User", GameManager.gameManager.Uid);
        GameManager.gameManager.textFame.text = GameManager.gameManager.fame.ToString();
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
        float animationSpeed = 1.5f; // �ִϸ��̼� �ӵ�
        float moveDistance = 30.0f; // �ؽ�Ʈ�� ���� �̵��� �Ÿ�
        Vector3 initialPosition = _text.transform.localPosition;
        Vector3 targetPosition = initialPosition;

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
        float animationDuration = 1.0f / animationSpeed; // �ִϸ��̼� ���� �ð��� �ӵ��� ����

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
    private IEnumerator AnimateTotal()
    {
        float currentScore = 0;
        float duration = 1.0f; // �ִϸ��̼��� 1�� ���� ����
        float elapsed = 0f;
        textScore.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            currentScore = Mathf.Lerp(0, fame, t);

            // �ؽ�Ʈ ������Ʈ
            textScore.text = Mathf.FloorToInt(currentScore).ToString();

            yield return null;
        }

        // ���� �� ����
        textScore.text = fame.ToString();
    }

}
