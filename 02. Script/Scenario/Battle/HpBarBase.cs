using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumCollection;

public class HpBarBase : MonoBehaviour
{
    public Image hpBar;
    public Image armorBar;
    public Image hpBar_Back;
    private float backSpeed = 0.3f;
    public void SetHp(float _hp, float _armor, float _maxHp)
    {
        float hpUpper = (_hp + _armor > _maxHp) ? _hp + _armor : _maxHp;
        
        armorBar.fillAmount = (_hp + _armor) / hpUpper;
        if (BattleScenario.battlePatern == BattlePatern.Battle)
        {
            if (gameObject.activeSelf)
                StartCoroutine(GradualDown(hpBar, hpBar.fillAmount, _hp / hpUpper));
        }
        else
        {
            hpBar.fillAmount = _hp / hpUpper;
        }
    }

    IEnumerator GradualDown(Image _bar, float _startAmount, float _endAmount)
    {
        float duration = 0.15f; // �ִϸ��̼� ���� �ð� (��)
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // ������ �ð� ���� (0~1)
            _bar.fillAmount = Mathf.Lerp(_startAmount, _endAmount, t); // �����Ͽ� fillAmount ����

            yield return null;
        }

        // �ִϸ��̼��� ������ ���� ������ ����
        _bar.fillAmount = _endAmount;
    }
    private void Update()
    {
        switch (BattleScenario.battlePatern)
        {
            case BattlePatern.Battle:
                if (hpBar_Back)
                    if (hpBar.fillAmount < hpBar_Back.fillAmount)
                    {
                        hpBar_Back.fillAmount -= Time.deltaTime * backSpeed;
                    }
                break;
            case BattlePatern.OnReady:
                if (hpBar_Back)
                    hpBar_Back.fillAmount = hpBar.fillAmount;
                break;
        }

    }
}