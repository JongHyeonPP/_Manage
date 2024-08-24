using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumCollection;

public class BattleTimer : MonoBehaviour
{
    [SerializeField] Image imageFill;
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textDamage;
    [SerializeField] Image imageArrowUp;

    private void Start()
    {
        SettingManager.LanguageChangeEvent += OnLanguageChange;
        OnLanguageChange();
        textExplain.gameObject.SetActive(false);
        textDamage.gameObject.SetActive(false);
        imageArrowUp.gameObject.SetActive(false);
    }

    public void TimerStart()
    {
        StartCoroutine(ReduceFillAmount());
    }

    public void OnLanguageChange()
    {
        textExplain.text = GameManager.language == Language.Ko ? "모든 피해량" : "All Damage";
    }

    IEnumerator ReduceFillAmount()
    {
        float duration = BattleScenario.battleTime;
        float oneThirdDuration = duration / 3f;
        float twoThirdDuration = 2 * oneThirdDuration;
        float elapsed = 0f;

        bool oneThirdLogged = false;
        bool twoThirdLogged = false;

        Color initialColor = new Color(0, 1, 0, imageFill.color.a); // 기존 alpha 값을 유지
        imageFill.color = initialColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            imageFill.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);

            if (elapsed <= duration / 2f)
            {
                // 알파 값을 유지하면서 색상 변경
                imageFill.color = Color.Lerp(new Color(0, 1, 0, imageFill.color.a), new Color(1, 1, 0, imageFill.color.a), elapsed / (duration / 2f));
            }
            else
            {
                float secondHalfElapsed = elapsed - (duration / 2f);
                // 알파 값을 유지하면서 색상 변경
                imageFill.color = Color.Lerp(new Color(1, 1, 0, imageFill.color.a), new Color(1, 0, 0, imageFill.color.a), secondHalfElapsed / (duration / 2f));
            }

            if (!oneThirdLogged && elapsed >= oneThirdDuration)
            {
                oneThirdLogged = true;
                PhaseOne();
            }

            if (!twoThirdLogged && elapsed >= twoThirdDuration)
            {
                twoThirdLogged = true;
                StartCoroutine(PhaseTwo());
            }

            yield return null;
        }

        StartCoroutine(PhaseLast());
        imageFill.fillAmount = 0f;
        imageFill.color = new Color(1, 0, 0, imageFill.color.a); // 마지막에 alpha를 유지
    }

    private void PhaseOne()
    {
        BattleScenario.damageRatio = 1.2f;
        textDamage.text = "20%";
        textDamage.color = new Color(1f, 1f, 0f);
        StartCoroutine(GameManager.FadeUi(textExplain, 0.5f, true));
        StartCoroutine(GameManager.FadeUi(textDamage, 0.5f, true));
        StartCoroutine(GameManager.FadeUi(imageArrowUp, 0.5f, true));
    }

    private IEnumerator PhaseTwo()
    {
        BattleScenario.damageRatio = 1.5f;
        yield return StartCoroutine(GameManager.FadeUi(textDamage, 0.5f, false));
        textDamage.text = "50%";
        textDamage.color = new Color(0.9f, 0.5f, 0f);
        StartCoroutine(GameManager.FadeUi(textDamage, 0.5f, true));
    }

    private IEnumerator PhaseLast()
    {
        BattleScenario.damageRatio = 2f;
        yield return StartCoroutine(GameManager.FadeUi(textDamage, 0.5f, false));
        textDamage.text = "100%";
        textDamage.color = new Color(1f, 0f, 0f);
        StartCoroutine(GameManager.FadeUi(textDamage, 0.5f, true));
    }
}
