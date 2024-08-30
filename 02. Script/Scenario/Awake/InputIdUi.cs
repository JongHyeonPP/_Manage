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
        textExplain.text = GameManager.language == Language.Ko ? "사용할 아이디를 입력하세요." : "Please enter your Id.";
        textButton.text = GameManager.language == Language.Ko ? "계속하기" : "Continue";
        TextMeshProUGUI placeholderText = inputFieldId.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholderText != null)
        {
            placeholderText.text = GameManager.language == Language.Ko ? "아이디 입력" : "Input Id";
        }
    }

    void Update()
    {
        // Enter 키가 눌렸는지 확인 (포커스 여부와 관계없이)
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
        // 공백인지 확인
        if (string.IsNullOrWhiteSpace(id))
        {
            textExplain.text = GameManager.language == Language.Ko ? "아이디를 입력해주세요." : "Please enter Id";
            return false; // 공백일 경우 유효하지 않음
        }

        // 영문과 숫자로만 이루어진 텍스트인지 확인하는 정규식
        string pattern = "^[a-zA-Z0-9]+$";
        if (!Regex.IsMatch(id, pattern))
        {
            textExplain.text = GameManager.language == Language.Ko ? "영문과 숫자로만 입력해주세요." : "Please enter only letters and numbers.";
            return false;
        }
        return true;
    }

    private IEnumerator FadeOutUI()
    {
        // 1초 동안 알파 값을 0으로 변화시키는 코드
        float duration = 1.0f;

        // 텍스트와 이미지가 가진 현재 알파 값을 가져와서 저장
        float startAlphaText = textExplain.color.a;
        float startAlphaInput = inputFieldId.textComponent.color.a;
        float startAlphaImage = imageBase.color.a;

        // 점진적으로 알파값을 줄여가는 루프
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            SetAlpha(1 - normalizedTime);
            yield return null;
        }

        // 최종적으로 알파값을 0으로 설정
        SetAlpha(0);
        awakeScenario.ProgressWithId(inputFieldId.text);
    }

    private void SetAlpha(float alpha)
    {
        // 텍스트 알파 값 변경
        Color color = textExplain.color;
        color.a = alpha;
        textExplain.color = color;
        textButton.color = color;

        // 입력 필드 텍스트 알파 값 변경
        color = inputFieldId.textComponent.color;
        color.a = alpha;
        inputFieldId.textComponent.color = color;

        // 이미지 알파 값 변경
        color = imageBase.color;
        color.a = alpha;
        imageBase.color = color;
        imageInputField.color = color;
        imageButton.color = color;
    }
}
