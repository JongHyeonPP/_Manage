using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private async void Awake()
    {
        GameManager.storeScenario = this;
        cookUi.gameObject.SetActive(false);
        selectLight.SetActive(false);
        raycastBlock.SetActive(false);
        storeUi.gameObject.SetActive(false);
        storeTooltip.gameObject.SetActive(false);
        Dictionary<string, object> goodsDoc = await DataManager.dataManager.GetField($"Progress/{GameManager.gameManager.Uid}/Store","Data");
        if (goodsDoc == null)
        {
            storeUi.SetNewGoods();
            SetStoreAtDb();
        }
        else
        {
            storeUi.SetExistingSlot(goodsDoc);
        }
        
    }
    private void Start()
    {
        UniversalAdditionalCameraData baseCameraData = Camera.main.GetUniversalAdditionalCameraData();

        // 먼저 카메라 스택에서 해당 오버레이 카메라를 제거
        if (baseCameraData.cameraStack.Contains(overlayCamera))
        {
            baseCameraData.cameraStack.Remove(overlayCamera);
        }
        // 그런 다음 카메라 스택의 가장 마지막에 다시 추가 (즉, 가장 위로 올림)
        baseCameraData.cameraStack.Add(overlayCamera);
        // 먼저 카메라 스택에서 해당 오버레이 카메라를 제거
        if (baseCameraData.cameraStack.Contains(GameManager.gameManager.popUpCamera))
        {
            baseCameraData.cameraStack.Remove(GameManager.gameManager.popUpCamera);
        }
        // 그런 다음 카메라 스택의 가장 마지막에 다시 추가 (즉, 가장 위로 올림)
        baseCameraData.cameraStack.Add(GameManager.gameManager.popUpCamera);
    }
    public async void NextButtonClicked()
    {
        await ClearStoreAtDb();
        GameManager.storeScenario = null;
        StageScenarioBase.state = StateInMap.NeedPhase;
        SceneManager.LoadSceneAsync("Stage" + StageScenarioBase.stageNum);
    }
    private async Task ClearStoreAtDb()
    {
        DocumentReference reference = DataManager.dataManager.GetDocumentReference($"Progress/{GameManager.gameManager.Uid}/Store/Data");
        await reference.DeleteAsync();
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
        if (!cookUi.isOperating)
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
    public async void SetStoreAtDb()
    {

        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
        {
            for (int i = 0; i < 3; i++)
                storeUi.goodsPanels[i].SetGoodsAtDb();
        });
    }
}
