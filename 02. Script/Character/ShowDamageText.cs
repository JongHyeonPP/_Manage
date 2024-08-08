using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowDamageText : MonoBehaviour
{
    public TMP_Text damageTextPrefab; // �ؽ�Ʈ ������
    private float moveSpeed = 3f;
    private List<IEnumerator> coroutineQueue = new List<IEnumerator>();
    private Queue<TMP_Text> textPool = new Queue<TMP_Text>(); // TMP_Text ������Ʈ Ǯ
    Transform parentLocation;

    private void Awake()
    {
        damageTextPrefab = GameManager.battleScenario.damagePrefab;
        parentLocation = new GameObject("DamageTextParent").transform;
        parentLocation.parent = transform;
        parentLocation.localScale = Vector2.one;
        parentLocation.localPosition = Vector2.up * 60f;
        // �ʱ� �ؽ�Ʈ ������Ʈ ����
        for (int i = 0; i < 10; i++)
        {
            CreateNewText();
        }
    }

    private TMP_Text CreateNewText()
    {
        TMP_Text newText = Instantiate(damageTextPrefab, parentLocation);
        newText.gameObject.SetActive(false);
        textPool.Enqueue(newText);
        return newText;
    }

    public void StartShowTextsStatus(float _hp, float _maxHp, float _ability, float _resist, float _speed)
    {
        // �� �ڷ�ƾ�� ��⿭�� �߰�
        coroutineQueue.Add(ShowTextsStatus(_hp, _maxHp, _ability, _resist, _speed));

        // ��⿭�� ������� ������ �ڷ�ƾ ����
        if (coroutineQueue.Count == 1)
        {
            StartCoroutine(CoroutineTrigger());
        }
    }

    private IEnumerator CoroutineTrigger()
    {
        yield return StartCoroutine(coroutineQueue[0]);
        coroutineQueue.RemoveAt(0);
        if (coroutineQueue.Count > 0)
            StartCoroutine(CoroutineTrigger());
    }

    private IEnumerator ShowTextsStatus(float _hp, float _maxHp, float _ability, float _resist, float _speed)
    {
        StatusType[] statusTypes = { StatusType.Hp, StatusType.HpMax, StatusType.Ability, StatusType.Resist, StatusType.Speed };
        float[] values = { _hp, _maxHp, _ability, _resist, _speed };

        for (int i = 0; i < statusTypes.Length; i++)
        {
            if (values[i] == 0)
                continue; // ���� 0�̸� �ǳʶݴϴ�.

            TMP_Text textStatus = GetTextFromPool();
            textStatus.text = GetStatusStr(statusTypes[i], values[i]);
            textStatus.color = GetStatusColor(statusTypes[i]);
            StartCoroutine(MoveTextStatus(textStatus));
            yield return new WaitForSeconds(0.6f); // �ؽ�Ʈ�� ���������� �ö󰡵��� �ð����� �ݴϴ�.
        }
        yield return new WaitForSeconds(0.4f); // �ؽ�Ʈ�� ���������� �ö󰡵��� �ð����� �ݴϴ�.
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

            // ���� �̵� (���� �̵�)
            _textStatus.rectTransform.position = originalPosition + Vector3.up * moveSpeed * progress;

            yield return null;
        }

        // �ִϸ��̼��� ���� �Ŀ� �ؽ�Ʈ ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ
        _textStatus.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        _textStatus.gameObject.SetActive(false);
        textPool.Enqueue(_textStatus); // ����� ���� �ؽ�Ʈ�� Ǯ�� ��ȯ
    }

    private string GetStatusStr(StatusType _statusType, float _statusValue)
    {
        string statusStr;

        switch (_statusType)
        {
            default:
                if (GameManager.language == Language.Ko)
                    statusStr = "ü��";
                else
                    statusStr = "Hp";
                break;
            case StatusType.HpMax:
                if (GameManager.language == Language.Ko)
                    statusStr = "�ִ� ü��";
                else
                    statusStr = "Max Hp";
                break;
            case StatusType.Ability:
                if (GameManager.language == Language.Ko)
                    statusStr = "�ɷ�";
                else
                    statusStr = "Ability";
                break;
            case StatusType.Resist:
                if (GameManager.language == Language.Ko)
                    statusStr = "���׷�";
                else
                    statusStr = "Resist";
                break;
            case StatusType.Speed:
                if (GameManager.language == Language.Ko)
                    statusStr = "�ӵ�";
                else
                    statusStr = "Speed";
                break;
        }
        string returnValue = $"{statusStr} ";
        returnValue += "<size=130%>";
        if (_statusValue > 0)
            returnValue += '+';
        returnValue += _statusValue;
        return returnValue;
    }
    private Color GetStatusColor(StatusType _statusType)
    {
        switch (_statusType)
        {
            default:
                return new Color(1f, 0.27f, 0.27f);//FF4545
            case StatusType.HpMax:
                return new Color(1f, 0.54f, 0.27f);//FF8A45
            case StatusType.Ability:
                return new Color(0.98f, 0.97f, 0.36f);//FAF75C
            case StatusType.Resist:
                return new Color(0.27f, 1f, 0.39f);//45FF63
            case StatusType.Speed:
                return new Color(0.08f, 0.16f, 0.9f);//1429E6
        }
    }
    [ContextMenu("Test")]
    public void TestMethod()
    {
        StartShowTextsStatus(3f, 3f, 3f, 3f, -3f);
    }
}
