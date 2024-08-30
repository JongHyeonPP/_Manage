using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownSlot : SlotBase
{
    [SerializeField] Image imageSkillIcon;
    [SerializeField] Image imageCooldown;
    [SerializeField] TMP_Text textCooldown;
    private SkillAsItem currentSkill;
    private void Awake()
    {
        imageCooldown.fillAmount = 0f;
    }
    internal void InitSkill(SkillAsItem _skillAsItem)
    {
        imageSkillIcon.sprite = _skillAsItem.sprite;
        currentSkill = _skillAsItem;
    }
    public void StartCooldown(float cooldownTime)
    {
        imageCooldown.fillAmount = 1f;
        textCooldown.gameObject.SetActive(true);
        StartCoroutine(CooldownCoroutine(cooldownTime));
    }

    IEnumerator CooldownCoroutine(float cooldownTime)
    {
        float startTime = Time.time;

        while (Time.time - startTime < cooldownTime)
        {
            float elapsedTime = Time.time - startTime;
            imageCooldown.fillAmount = 1 - (elapsedTime / cooldownTime);

            textCooldown.text =(cooldownTime - elapsedTime).ToString("F0");
            yield return null; // 대기 없이 다음 프레임으로 진행
        }

        // 쿨다운이 완료되면 이미지를 다시 초기화하거나 다른 작업을 수행할 수 있습니다.
        imageCooldown.fillAmount = 0f;
        // 쿨다운이 완료된 후에 다른 작업을 추가하려면 이 부분에 코드를 추가하세요.
        textCooldown.gameObject.SetActive(false);
    }
    public void OnPointerEnter()
    {
        HighlightOn();

        ItemTooltip battleTooltip = GameManager.battleScenario.battleTooltip;
        battleTooltip.transform.SetParent(transform);
        battleTooltip.transform.localScale = new Vector2(1f, 1f);
        battleTooltip.rectTransform.anchorMin = new Vector2(0.5f,0f);
        battleTooltip.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        battleTooltip.rectTransform.pivot = new Vector2(0.5f,0f);
        battleTooltip.transform.localPosition = new Vector2(0f, 30f);
        battleTooltip.SetTooltipInfo(currentSkill);

        battleTooltip.gameObject.SetActive(true);
    }
    public void OnPointerExit()
    {
        HighlightOff();
        GameManager.battleScenario.battleTooltip.gameObject.SetActive(false);
    }
}
