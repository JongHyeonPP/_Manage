using EnumCollection;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputIdUi : MonoBehaviour
{
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_InputField inputFieldId;
    [SerializeField] Image imageInputField;
    [SerializeField] Image imageBase;
    [SerializeField] AwakeScenario awakeScenario;
    [SerializeField] Image imageButton;
    [SerializeField] TMP_Text textButton;
    [SerializeField] Button buttonContinue;

    void Start()
    {
        textExplain.text = GameManager.language == Language.Ko ? "����� ���̵� �Է��ϼ���." : "Please enter your Id.";
        textButton.text = GameManager.language == Language.Ko ? "����ϱ�" : "Continue";
        TextMeshProUGUI placeholderText = inputFieldId.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholderText != null)
        {
            placeholderText.text = GameManager.language == Language.Ko ? "���̵� �Է�" : "Input Id";
        }
    }

    void Update()
    {
        // Enter Ű�� ���ȴ��� Ȯ�� (��Ŀ�� ���ο� �������)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnButtonClick();
        }
    }

    public void OnButtonClick()
    {
        bool isSuccess = IsValidId(inputFieldId.text);
        if (isSuccess)
        {
            SoundManager.SfxPlay("Pause");
            buttonContinue.enabled = false;
            StartCoroutine(FadeOutUI());
        }
    }

    private bool IsValidId(string id)
    {
        // �������� Ȯ��
        if (string.IsNullOrWhiteSpace(id))
        {
            textExplain.text = GameManager.language == Language.Ko ? "���̵� �Է����ּ���." : "Please enter Id";
            return false; // ������ ��� ��ȿ���� ����
        }

        // ������ ���ڷθ� �̷���� �ؽ�Ʈ���� Ȯ���ϴ� ���Խ�
        string pattern = "^[a-zA-Z0-9]+$";
        if (!Regex.IsMatch(id, pattern))
        {
            textExplain.text = GameManager.language == Language.Ko ? "������ ���ڷθ� �Է����ּ���." : "Please enter only letters and numbers.";
            return false;
        }
        return true;
    }

    private IEnumerator FadeOutUI()
    {
        // 1�� ���� ���� ���� 0���� ��ȭ��Ű�� �ڵ�
        float duration = 1.0f;

        // �ؽ�Ʈ�� �̹����� ���� ���� ���� ���� �����ͼ� ����
        float startAlphaText = textExplain.color.a;
        float startAlphaInput = inputFieldId.textComponent.color.a;
        float startAlphaImage = imageBase.color.a;

        // ���������� ���İ��� �ٿ����� ����
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            SetAlpha(1 - normalizedTime);
            yield return null;
        }

        // ���������� ���İ��� 0���� ����
        SetAlpha(0);
        awakeScenario.ProgressWithId(inputFieldId.text);
    }

    private void SetAlpha(float alpha)
    {
        // �ؽ�Ʈ ���� �� ����
        Color color = textExplain.color;
        color.a = alpha;
        textExplain.color = color;
        textButton.color = color;

        // �Է� �ʵ� �ؽ�Ʈ ���� �� ����
        color = inputFieldId.textComponent.color;
        color.a = alpha;
        inputFieldId.textComponent.color = color;

        // �̹��� ���� �� ����
        color = imageBase.color;
        color.a = alpha;
        imageBase.color = color;
        imageInputField.color = color;
        imageButton.color = color;
    }
}
