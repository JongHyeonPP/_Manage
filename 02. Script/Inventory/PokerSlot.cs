using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PokerSlot : SlotBase
{
    private CookUi cookUi;
    public IngredientClass ingredient;
    private Image imageSlot;
    public Image imageIngredient;
    public Image imagePokerNum;
    public TMP_Text pokerNum;
     
    private void Awake()
    {
        base.Awake();
        ClearPoker();
    }
    private void Start()
    {
        cookUi = GameManager.storeScenario.cookUi;
        imageSlot = GetComponent<Image>();
    }
    public void SetPoker(IngredientClass _ingredient)
    {
        ingredient = _ingredient;
        imageIngredient.gameObject.SetActive(true);
        imagePokerNum.gameObject.SetActive(true);
        imageIngredient.sprite = ingredient.sprite;
        pokerNum.text = ingredient.pokerNum.ToString();
        pokerNum.color = ingredient.GetPokerNumColor();
    }
    public void ClearPoker()
    {
        ingredient = null;
        imageIngredient.gameObject.SetActive(false);
        imagePokerNum.gameObject.SetActive(false);
    }
    public void OnButtonClicked()
    {
        if (ingredient == null)
            return;
            cookUi.MoveIngredientToScroll(ingredient);
        if (ingredient == null)
            SetHighLightAlphaZero();
    }
    [ContextMenu("MoveTest")]
    public void MoveTest()
    {
        StartCoroutine(MoveToObject(cookUi.imageResult.GetComponent<RectTransform>(), 1f));
    }
    // �̵��ϴ� �ڷ�ƾ
    public IEnumerator MoveToObject(RectTransform target, float duration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.position;
        Vector3 endPosition = target.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // ��ġ ����
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

            // ���İ� ����
            float alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            SetAlpha(alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ ����
        rectTransform.position = endPosition;

        // ���� ���İ� ����
        SetAlpha(0);
    }
    public void ResetAlpha()
    {
        SetAlpha(1f);
    }
    // ���İ� ���� �Լ�
    private void SetAlpha(float alpha)
    {
        // �� UI ����� ���İ��� ����
        imageSlot.color = new Color(imageSlot.color.r, imageSlot.color.g, imageSlot.color.b, alpha);
        imageIngredient.color = new Color(imageIngredient.color.r, imageIngredient.color.g, imageIngredient.color.b, alpha);
        imagePokerNum.color = new Color(imagePokerNum.color.r, imagePokerNum.color.g, imagePokerNum.color.b, alpha);
        pokerNum.color = new Color(pokerNum.color.r, pokerNum.color.g, pokerNum.color.b, alpha);
    }
    public void OnPointerEnter()
    {
        if (ingredient != null)
            HightlightOn();
    }
    public void OnPointerExit()
    {
        if (ingredient == null)
            SetHighLightAlphaZero();
        else
            HightlightOff();
    }
}
