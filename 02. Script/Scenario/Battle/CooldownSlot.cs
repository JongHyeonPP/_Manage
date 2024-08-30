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
            yield return null; // ��� ���� ���� ���������� ����
        }

        // ��ٿ��� �Ϸ�Ǹ� �̹����� �ٽ� �ʱ�ȭ�ϰų� �ٸ� �۾��� ������ �� �ֽ��ϴ�.
        imageCooldown.fillAmount = 0f;
        // ��ٿ��� �Ϸ�� �Ŀ� �ٸ� �۾��� �߰��Ϸ��� �� �κп� �ڵ带 �߰��ϼ���.
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
