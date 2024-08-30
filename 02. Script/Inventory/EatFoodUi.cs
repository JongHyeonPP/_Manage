using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EatFoodUi : MonoBehaviour
{
    [SerializeField] TMP_Text textExplain;
    [SerializeField] List<EatFoodSlot> slots;
    [SerializeField] GameObject parentButton;
    private InventorySlot currentSlot;
    public void SetEatFoodUi(InventorySlot _slot)
    {
        currentSlot = _slot;
        for (int i = 0; i < 3; i++)
        {
            slots[i].SetSlot(GameManager.gameManager.characterList[i], (FoodClass)_slot.ci.item);
        }
        
    }
    private void OnEnable()
    {
        parentButton.SetActive(true);
        textExplain.text = GameManager.language == Language.Ko ? "음식을 먹을 캐릭터를 선택하세요." : "Choose a character to eat the food.";
        ItemManager.itemManager.InventoryRayBlock.SetActive(true);
    }
    private void OnDisable()
    {
        SoundManager.SfxPlay("WoodenClick_0");
        ItemManager.itemManager.InventoryRayBlock.SetActive(false);
        foreach (EatFoodSlot x in slots)
            x.Deselect();
    }

    public void DeselectOthers(EatFoodSlot _eatFoodSlot)
    {
        foreach (EatFoodSlot x in slots)
        {
            if (x != _eatFoodSlot)
            {
                x.Deselect();
            }
        }
    }
    public void OnEatButtonClick()
    {
        SoundManager.SfxPlay("Decline");
        StartCoroutine(ApplyValuesCoroutine());
    }

    private IEnumerator ApplyValuesCoroutine()
    {
        switch (((FoodClass)currentSlot.ci.item).foodEffect.targetRange)
        {
            case FoodTargetRange.One:
                EatFoodSlot target = slots.Where(item => item.isSelected).FirstOrDefault();
                if (target == null)
                {
                    string text = GameManager.language == Language.Ko ? "대상을 선택해야 합니다." : "You need to select a target.";
                    GameManager.gameManager.SetPopUp(text);
                    yield break;
                }
                parentButton.SetActive(false);
                target.ApplyFoodEffect();
                break;
            case FoodTargetRange.All:
                parentButton.SetActive(false);
                foreach (var x in slots)
                    x.ApplyFoodEffect();
                break;
        }
        FoodClass food = (FoodClass)currentSlot.ci.item;
        GameManager.gameManager.foodNum += (int)food.pokerCombination;
        currentSlot.ChangeCiAmount(-1);
        textExplain.text = GameManager.language == Language.Ko ? "아주 맛있어!" : "Delicious!";

        SetFoodAtDb();
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);

        async void SetFoodAtDb()
        {
            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
            {
                Dictionary<string, object> docDict = new()
            {
                { "FoodNum", GameManager.gameManager.foodNum }
            };
                DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.uid);
                ItemManager.itemManager.SetInventoryAtDb();
                return Task.CompletedTask;
            });
        }
    }
}
