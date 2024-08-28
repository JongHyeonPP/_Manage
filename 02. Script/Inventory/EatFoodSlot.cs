using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EatFoodSlot : MonoBehaviour
{
    [SerializeField] CharacterHierarchy ch;
    [SerializeField] GameObject lightObject;
    [SerializeField] Image imageFill;
    [SerializeField] Image imageExpect;
    [SerializeField] ParentStatusUp parentStatusUp;
    public bool isSelected;
    private FoodClass food;
    private CharacterData data;
    private void OnEnable()
    {
        imageExpect.gameObject.SetActive(true);
    }
    public void SetSlot(CharacterData _data, FoodClass _food)
    {
        food = _food;
        data = _data;
        ch.CopyHierarchySprite(_data.characterHierarchy);
        imageFill.fillAmount = _data.hp / _data.maxHp;
        if (food.foodEffect.targetRange == FoodTargetRange.All)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }
    public void OnPointerEnter()
    {
        if (food.foodEffect.targetRange == FoodTargetRange.All)
            return;
        lightObject.SetActive(true);
        imageExpect.fillAmount = imageFill.fillAmount + food.foodEffect.healAmount;
    }
    public void OnPointerExit()
    {
        if (food.foodEffect.targetRange == FoodTargetRange.All)
            return;
        if (!isSelected)
        {
            lightObject.SetActive(false);
            imageExpect.fillAmount = 0f;
        }
    }
    public void OnPointerClick()
    {
        if (food.foodEffect.targetRange == FoodTargetRange.All)
            return;
        if (isSelected)
            Deselect();
        else
        {
            Select();
            if (food.foodEffect.targetRange == FoodTargetRange.One)
            {
                ItemManager.itemManager.eatFoodUi.DeselectOthers(this);
            }
        }
    }
    public void Select()
    {
        isSelected = true;
        lightObject.SetActive(true);
        imageExpect.fillAmount = imageFill.fillAmount + food.foodEffect.healAmount;
    }
    public void Deselect()
    {
        isSelected = false;
        lightObject.SetActive(false);
        imageExpect.fillAmount = 0f;
    }
    public void ApplyFoodEffect()
    {
        data.hp = Mathf.Min(data.maxHp, data.hp + data.maxHp * food.foodEffect.healAmount);
        imageExpect.gameObject.SetActive(false);
        StartCoroutine(FillAmountOverTime());
        Dictionary<StatusType, float> statusValue = food.foodEffect.statusValue;
        switch (food.statusType)
        {
            case StatusType.HpMax:
                parentStatusUp.StartShowTextsStatus(0f, statusValue[StatusType.HpMax], 0f, 0f, 0f);
                data.maxHp += statusValue[StatusType.HpMax];
                break;
            case StatusType.Ability:
                parentStatusUp.StartShowTextsStatus(0f, 0f, statusValue[StatusType.Ability], 0f, 0f);
                data.ability += statusValue[StatusType.Ability];
                break;
            case StatusType.Resist:
                parentStatusUp.StartShowTextsStatus(0f, 0f, 0f, statusValue[StatusType.Resist], 0f);
                data.resist += statusValue[StatusType.Resist];
                break;
            case StatusType.Speed:
                parentStatusUp.StartShowTextsStatus(0f, 0f, 0f, 0f, statusValue[StatusType.Speed]);
                data.speed += statusValue[StatusType.Speed];
                break;
        }
        if (data == ItemManager.itemManager.selectedCharacter)
        {
            ItemManager.itemManager.inventoryUi.SetStatusSlotText();
        }
    }
    private IEnumerator FillAmountOverTime()
    {
        float duration = 1.0f; // 1 second duration
        float startFillAmount = imageFill.fillAmount;
        float targetFillAmount = imageExpect.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            imageFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            yield return null;
        }

        // Ensure the final fill amount is set correctly
        imageFill.fillAmount = targetFillAmount;
    }
}
