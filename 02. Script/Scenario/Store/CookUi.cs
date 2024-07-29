using EnumCollection;
using HardLight2DUtil;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CookUi : MonoBehaviour
{
    public IngredientScrollView meatView;
    public IngredientScrollView fishView;
    public IngredientScrollView fruitView;
    public IngredientScrollView vegetableView;
    public GridLayoutGroup parentPokerSlot;
    public List<PokerSlot> pokerSlots;
    public TMP_Text textResult;
    public Image imageResult;
    private PokerCombination currentCombination;
    public List<GradeStar> gradeStars;
    public Image imageFire;
    public TMP_Text textName;
    private void Start()
    {
        if (!GameManager.gameManager)
            return;
        textResult.text = (GameManager.language == Language.Ko) ? "노 카드" : "No Card";
        textName.text = string.Empty;
        var inventorySlots = ItemManager.itemManager.inventoryUi.inventorySlots;
        var cies = inventorySlots
            .Where(data => data.ci != null && data.ci.item.itemType == ItemType.Ingredient)
            .Select(data => data.ci);

        SetCiesToScrollView(GetSortedItemsByType(cies, IngredientType.Meat), meatView);
        SetCiesToScrollView(GetSortedItemsByType(cies, IngredientType.Fish), fishView);
        SetCiesToScrollView(GetSortedItemsByType(cies, IngredientType.Fruit), fruitView);
        SetCiesToScrollView(GetSortedItemsByType(cies, IngredientType.Vegetable), vegetableView);
        imageFire.gameObject.SetActive(false);
    }

    private List<CountableItem> GetSortedItemsByType(IEnumerable<CountableItem> items, IngredientType type)
    {
        return items
            .Where(data => ((IngredientClass)data.item).ingredientType == type)
            .OrderBy(data => ((IngredientClass)data.item).pokerNum)
            .ToList();
    }
    private void SetCiesToScrollView(List<CountableItem> _cies, IngredientScrollView _view)
    {
        foreach (var ci in _cies)
        {
            _view.AddIngredientCi(ci);
        }
    }

    public bool OnIngredientSlotClicked(Item _item)
    {
        if (!pokerSlots.Select(data => data.ingredient).Contains(null))
            return false;
        List<IngredientClass> allIngredient = pokerSlots.Where(data => data.ingredient != null).Select(data => data.ingredient).ToList();
        allIngredient.Add((IngredientClass)_item);
        allIngredient = allIngredient.OrderBy(data => data.pokerNum).ThenBy(data => data.ingredientType).ToList();
        for (int i = 0; i < 5; i++)
        {
            if (i < allIngredient.Count)
            {
                pokerSlots[i].SetPoker(allIngredient[i]);
            }
            else
            {
                pokerSlots[i].ClearPoker();
            }
        }
        currentCombination = EvaluateHand(allIngredient);
        textResult.text = ConvertPokerCombinationToString(currentCombination);
        return true;
    }
    public void MoveIngredientToScroll(IngredientClass _ingreient)
    {
        AddIngredientToSlot(_ingreient);
        List<IngredientClass> allIngredient = pokerSlots.Where(data => data.ingredient != null).Select(data => data.ingredient).ToList();
        allIngredient = allIngredient.OrderBy(data => data.pokerNum).ThenBy(data => data.ingredientType).ToList();
        allIngredient.Remove(_ingreient);
        for (int i = 0; i < 5; i++)
        {
            if (i < allIngredient.Count)
            {
                pokerSlots[i].SetPoker(allIngredient[i]);
            }
            else
            {
                pokerSlots[i].ClearPoker();
            }
        }
        currentCombination = EvaluateHand(allIngredient);
        textResult.text = ConvertPokerCombinationToString(currentCombination);
    }

    private void AddIngredientToSlot(IngredientClass _ingreient)
    {
        IngredientScrollView currentScrollView;
        switch (_ingreient.ingredientType)
        {
            default:
                currentScrollView = meatView;
                break;
            case IngredientType.Fish:
                currentScrollView = fishView;
                break;
            case IngredientType.Fruit:
                currentScrollView = fruitView;
                break;
            case IngredientType.Vegetable:
                currentScrollView = vegetableView;
                break;
        }
        IngredientSlot targetSlot = currentScrollView.slots.Where(data => data.ci.item.itemId == _ingreient.itemId).FirstOrDefault();
        if (!targetSlot.gameObject.activeSelf)
        {
            targetSlot.gameObject.SetActive(true);
            currentScrollView.ModifySizeDelta(true);
        }
        targetSlot.ci.amount++;
        targetSlot.textAmount.text = targetSlot.ci.amount.ToString();
    }

    public PokerCombination EvaluateHand(List<IngredientClass> _hand)
    {
        if (_hand == null || _hand.Count == 0)
            return PokerCombination.NoCard;

        // 손에 있는 카드를 정렬
        var sortedHand = _hand.OrderBy(card => card.pokerNum).ToList();

        // 각 포커 핸드의 체크 로직
        bool isFlush = sortedHand.Count >= 5 && IsFlush(sortedHand);
        bool isStraight = sortedHand.Count >= 5 && IsStraight(sortedHand);
        bool isStraightFlush = isFlush && isStraight;
        bool isFourOfAKind = IsFourOfAKind(sortedHand);
        bool isFullHouse = IsFullHouse(sortedHand);
        bool isThreeOfAKind = IsThreeOfAKind(sortedHand);
        bool isTwoPair = IsTwoPair(sortedHand);
        bool isOnePair = IsOnePair(sortedHand);
        PokerCombination pokerCombination;
        // 포커 핸드의 우선순위에 따라 결과를 반환
        if (isStraightFlush)
            pokerCombination = PokerCombination.StraightFlush;
        else if (isFourOfAKind)
            pokerCombination = PokerCombination.FourOfAKind;
        else if (isFullHouse)
            pokerCombination = PokerCombination.FullHouse;
        else if (isFlush)
            pokerCombination = PokerCombination.Flush;
        else if (isStraight)
            pokerCombination = PokerCombination.Straight;
        else if (isThreeOfAKind)
            pokerCombination = PokerCombination.ThreeOfAKind;
        else if (isTwoPair)
            pokerCombination = PokerCombination.TwoPair;
        else if (isOnePair)
            pokerCombination = PokerCombination.OnePair;
        else
            pokerCombination = PokerCombination.HighCard;
        SetGradeStar(pokerCombination);
        if (pokerCombination == PokerCombination.NoCard)
        {
            imageResult.gameObject.SetActive(false);
        }
        else
        {
            imageResult.gameObject.SetActive(true);
            imageResult.sprite = LoadManager.loadManager.foodDict.Values.Where(data => data.pokerCombination == pokerCombination).FirstOrDefault().sprite;
            imageResult.color = Color.black;
        }
            
        
        return pokerCombination;
    }



    private bool IsFlush(List<IngredientClass> hand)
    {
        return hand.All(card => card.ingredientType == hand[0].ingredientType);
    }

    private bool IsStraight(List<IngredientClass> hand)
    {
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (hand[i].pokerNum + 1 != hand[i + 1].pokerNum)
                return false;
        }
        return true;
    }

    private bool IsFourOfAKind(List<IngredientClass> hand)
    {
        var groupedByNum = hand.GroupBy(card => card.pokerNum);
        return groupedByNum.Any(group => group.Count() >= 4);
    }

    private bool IsFullHouse(List<IngredientClass> hand)
    {
        var groupedByNum = hand.GroupBy(card => card.pokerNum);
        return groupedByNum.Count() == 2 && groupedByNum.Any(group => group.Count() == 3);
    }

    private bool IsThreeOfAKind(List<IngredientClass> hand)
    {
        var groupedByNum = hand.GroupBy(card => card.pokerNum);
        return groupedByNum.Any(group => group.Count() == 3);
    }

    private bool IsTwoPair(List<IngredientClass> hand)
    {
        var groupedByNum = hand.GroupBy(card => card.pokerNum);
        return groupedByNum.Count(group => group.Count() == 2) == 2;
    }

    private bool IsOnePair(List<IngredientClass> hand)
    {
        var groupedByNum = hand.GroupBy(card => card.pokerNum);
        return groupedByNum.Any(group => group.Count() == 2);
    }
    private string ConvertPokerCombinationToString(PokerCombination _pokerCombination)
    {
        switch (_pokerCombination)
        {
            default:
                return (GameManager.language == Language.Ko) ? "노 카드" : "No Card";
            case PokerCombination.HighCard:
                return (GameManager.language == Language.Ko) ? "하이 카드" : "High Card";
            case PokerCombination.OnePair:
                return (GameManager.language == Language.Ko) ? "원 페어" : "One Pair";
            case PokerCombination.TwoPair:
                return (GameManager.language == Language.Ko) ? "투 페어" : "Two Pair";
            case PokerCombination.ThreeOfAKind:
                return (GameManager.language == Language.Ko) ? "트리플" : "Three of a Kind";
            case PokerCombination.Straight:
                return (GameManager.language == Language.Ko) ? "스트레이트" : "Straight";
            case PokerCombination.Flush:
                return (GameManager.language == Language.Ko) ? "플러시" : "Flush";
            case PokerCombination.FullHouse:
                return (GameManager.language == Language.Ko) ? "풀 하우스" : "Full House";
            case PokerCombination.FourOfAKind:
                return (GameManager.language == Language.Ko) ? "포카드" : "Four of a Kind";
            case PokerCombination.StraightFlush:
                return (GameManager.language == Language.Ko) ? "스트레이트 플러시" : "Straight Flush";
        }
    }
    private void SetGradeStar(PokerCombination _pokerCombination)
    {
        int starNum;
        bool isHalfFill;
        switch (_pokerCombination)
        {
            default:
                starNum = 0;
                isHalfFill = false;
                break;
            case PokerCombination.HighCard:
                starNum = 1;
                isHalfFill = false;
                break;
            case PokerCombination.OnePair:
                starNum = 1;
                isHalfFill = true;
                break;
            case PokerCombination.TwoPair:
                starNum = 2;
                isHalfFill = false;
                break;
            case PokerCombination.ThreeOfAKind:
                starNum = 2;
                isHalfFill = true;
                break;
            case PokerCombination.Straight:
                starNum = 3;
                isHalfFill = false;
                break;
            case PokerCombination.Flush:
                starNum = 3;
                isHalfFill = true;
                break;
            case PokerCombination.FullHouse:
                starNum = 4;
                isHalfFill = false;
                break;
            case PokerCombination.FourOfAKind:
                starNum = 4;
                isHalfFill = true;
                break;
            case PokerCombination.StraightFlush:
                starNum = 5;
                isHalfFill = false;
                break;
        }
        int i;
        for (i = 0; i < starNum; i++)
        {
            gradeStars[i].imageFill.fillAmount = 1f;
        }
        if (isHalfFill)
        {
            gradeStars[i++].imageFill.fillAmount = 0.5f;
        }
        for (int j = i; j < gradeStars.Count; j++)
        {
            gradeStars[j].imageFill.fillAmount = 0f;
        }
    }
    public void ResetPoker()
    {
        foreach (PokerSlot slot in pokerSlots)
        {
            if (slot.ingredient == null)
                continue;
            AddIngredientToSlot(slot.ingredient);
            slot.ClearPoker();
        }
        textResult.text = (GameManager.language == Language.Ko) ? "노 카드" : "No Card";
        foreach (GradeStar gradeStar in gradeStars)
        {
            gradeStar.imageFill.fillAmount = 0f;
        }
        imageResult.gameObject.SetActive(false);
    }
    public void CreateButtonClicked()
    {
        if (!pokerSlots.Select(data => data.ingredient).Contains(null))
        {
            StartCoroutine(CreateCoroutine());
        }
        else
        {
            GameManager.gameManager.SetPopUp("재료를 5개 선택해야합니다.","5개");
        }
    }
    public IEnumerator CreateCoroutine()
    {
        Debug.Log(ItemManager.itemManager.inventoryUi.inventorySlots);
        imageResult.gameObject.SetActive(false);
        parentPokerSlot.enabled = false;
        RectTransform rect = imageResult.GetComponent<RectTransform>();
        foreach (PokerSlot slot in pokerSlots)
        {
            StartCoroutine(slot.MoveToObject(rect, 1f));
            yield return new WaitForSeconds(0.5f);
        }
        imageFire.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInOut(imageFire, 0.5f, true));
        yield return new WaitForSeconds(2f);
        parentPokerSlot.enabled = true;
        yield return StartCoroutine(FadeInOut(imageFire, 1f, false));
        imageFire.gameObject.SetActive(false);
        foreach (PokerSlot slot in pokerSlots)
        {
            slot.ResetAlpha();
        }
        List<FoodClass> ableFoods = LoadManager.loadManager.foodDict.Values.Where(data => data.pokerCombination == currentCombination).ToList();
        FoodClass selecedFood = ableFoods[Random.Range(0, ableFoods.Count)];
        imageResult.color = new Color(1, 1, 1, 0);
        imageResult.gameObject.SetActive(true);
        StartCoroutine(FadeInOut(imageResult, 1f, true));
        ItemManager.itemManager.SetItemToAbleIndex(new(selecedFood));
        //Clear
        foreach (var x in pokerSlots)
        {
            x.ClearPoker();
        }
        foreach (var x in ItemManager.itemManager.inventoryUi.inventorySlots.Where(data=>data.ci!=null))
        {
            x.ChangeCiAmount(0);
        }
        textName.text = selecedFood.name[GameManager.language];

        textResult.text = "";
    }
    private IEnumerator FadeInOut(Image _image, float _duration, bool _isFadeIn)
    {
        float elapsedTime = 0f;
        Color color = _image.color;
        color.a = _isFadeIn ? 0f : 1f; // 초기 알파값 설정
        _image.color = color;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(_isFadeIn ? elapsedTime / _duration : 1 - elapsedTime / _duration); // 알파값 계산
            _image.color = color;
            yield return null;
        }

        // 최종 알파값 설정
        color.a = _isFadeIn ? 1f : 0f;
        _image.color = color;
    }
    private void OnDisable()
    {
        ResetPoker();
    }
}