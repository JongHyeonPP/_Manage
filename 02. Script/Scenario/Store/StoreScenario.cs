using EnumCollection;
using ItemCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StoreScenario : MonoBehaviour
{
    public CookUi cookUi;
    public StoreUi storeUi;
    public ItemTooltip storeTooltip;
    public List<StoreSelect> storeSelects;
    public GameObject selectLight;
    public GameObject raycastBlock;
    public static readonly int ingredientPrice = 5;
    public static readonly List<int> skillPrices = new() {30,100,300 };
    public static readonly List<int> weaponPrices = new() {10, 30,100,300 };
    public static readonly List<int> foodPrices = new() {25,30,35,40,45,50,55,60,65 };
    public Camera overlayCamera;
    private void Awake()
    {
        GameManager.storeScenario = this;
        cookUi.gameObject.SetActive(false);
        selectLight.SetActive(false);
        raycastBlock.SetActive(false);
        storeUi.gameObject.SetActive(false);
        storeTooltip.gameObject.SetActive(false);
        storeUi.SetGoods();
    }
    private void Start()
    {
        var baseCameraData = Camera.main.GetUniversalAdditionalCameraData();
        var overlayCameraData = overlayCamera.GetUniversalAdditionalCameraData();

        // ���� ī�޶� ���ÿ��� �ش� �������� ī�޶� ����
        if (baseCameraData.cameraStack.Contains(overlayCamera))
        {
            baseCameraData.cameraStack.Remove(overlayCamera);
        }

        // �׷� ���� ī�޶� ������ ���� �������� �ٽ� �߰� (��, ���� ���� �ø�)
        baseCameraData.cameraStack.Add(overlayCamera);
    }
    public void NextButtonClicked()
    {
        SceneManager.LoadSceneAsync("Stage" + StageScenarioBase.stageNum);
    }

    public void OnMediumClicked(StoreCase _storeCase)
    {
        selectLight.SetActive(false);
        foreach (StoreSelect x in storeSelects)
        {
            x.imageMedium.SetActive(false);
        }
        switch (_storeCase)
        {
            case StoreCase.Cook:
                cookUi.gameObject.SetActive(true);
                break;
            case StoreCase.Store:
                storeUi.gameObject.SetActive(true);
                break;
        }
    }
    public void ActiveMediumImages()
    {
        foreach (var x in storeSelects)
        {
            x.imageMedium.SetActive(true);
        }
    }
    public void OnRaycastBlockClicked()
    {
        cookUi.gameObject.SetActive(false);
        storeUi.gameObject.SetActive(false);
    }
    public static int GetGoodsPrice(Item _item)
    {
        int price = 0;
        switch (_item.itemType)
        {
            case ItemType.Weapon:
                switch (_item.itemGrade)
                {
                    case ItemGrade.None:
                        price = weaponPrices[0];
                        break;
                    case ItemGrade.Normal:
                        price = weaponPrices[1];
                        break;
                    case ItemGrade.Rare:
                        price = weaponPrices[2];
                        break;
                    case ItemGrade.Unique:
                        price = weaponPrices[3];
                        break;
                }
                break;
            case ItemType.Skill:
                switch (_item.itemGrade)
                {
                    case ItemGrade.Normal:
                        price = skillPrices[0];
                        break;
                    case ItemGrade.Rare:
                        price = skillPrices[1];
                        break;
                    case ItemGrade.Unique:
                        price = skillPrices[2];
                        break;
                }
                break;
            case ItemType.Food:
                var food = (FoodClass)_item;
                switch (food.pokerCombination)
                {
                    case PokerCombination.HighCard:
                        price = foodPrices[0];
                        break;
                    case PokerCombination.OnePair:
                        price = foodPrices[1];
                        break;
                    case PokerCombination.TwoPair:
                        price = foodPrices[2];
                        break;
                    case PokerCombination.ThreeOfAKind:
                        price = foodPrices[3];
                        break;
                    case PokerCombination.Straight:
                        price = foodPrices[4];
                        break;
                    case PokerCombination.Flush:
                        price = foodPrices[5];
                        break;
                    case PokerCombination.FullHouse:
                        price = foodPrices[6];
                        break;
                    case PokerCombination.FourOfAKind:
                        price = foodPrices[7];
                        break;
                    case PokerCombination.StraightFlush:
                        price = foodPrices[8];
                        break;
                }
                break;
            case ItemType.Ingredient:
                price = ingredientPrice;
                break;
        }
        return price;
    }
}
