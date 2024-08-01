using EnumCollection;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillExpBar : MonoBehaviour
{
    public Image imageBase;
    private RectTransform rectTransform;
    public Image imageExpect;
    public Image imageExpOver;
    private int settedExp;
    private int overExp;
    private ItemGrade currentGrade;
    public bool isOperating;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetBaseValue(int _baseExp, ItemGrade _itemGrade)
    {
        currentGrade = _itemGrade;
        settedExp = _baseExp;
        imageBase.fillAmount = (float)settedExp / ItemManager.itemManager.GetNeedExp(currentGrade);
        
        float width;
        switch (_itemGrade)
        {
            default:
                width = 150f;
                break;
            case ItemGrade.Normal:
                width = 100f;
                break;
        }
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        if (currentGrade == ItemGrade.Unique)
            imageExpect.fillAmount = 1f;
        else
            imageExpect.fillAmount = 0f;
    }
    public bool IncreaseExpectValue(ItemGrade _itemGrade)
    {
        if (currentGrade == ItemGrade.Unique)
            return false;
        if (overExp >= ItemManager.itemManager.GetNeedExp(ItemGrade.Rare))
            return false;
        if (currentGrade == ItemGrade.Rare && settedExp == ItemManager.itemManager.GetNeedExp(ItemGrade.Rare))
            return false;
        int needExp = ItemManager.itemManager.GetNeedExp(currentGrade);
        int expValue = 0;
        switch (_itemGrade)
        {
            case ItemGrade.Normal:
                expValue = 1;
                break;
            case ItemGrade.Rare:
                expValue = 5;
                break;
        }


        settedExp += expValue;

        if (settedExp >= needExp)
        {
            if (currentGrade == ItemGrade.Normal)
            {

                overExp = settedExp - needExp;
                int overNeedExp = ItemManager.itemManager.GetNeedExp(ItemGrade.Rare);
                imageExpOver.fillAmount = (float)overExp / overNeedExp;
            }
            else if (settedExp == needExp)
            {
                //비운채로 두면 됨
            }
            else
            {
                return false;
            }
        }
        imageExpect.fillAmount = (float)settedExp / needExp;
        if (overExp > 0)
        {
            rectTransform.sizeDelta = new Vector2(150f, rectTransform.sizeDelta.y);
        }
        return true;
    }
    public void DecreaseExpectValue(ItemGrade _itemGrade)
    {
        int expValue = 0;
        int needExp = ItemManager.itemManager.GetNeedExp(currentGrade);
        switch (_itemGrade)
        {
            case ItemGrade.Normal:
                expValue = 1;
                break;
            case ItemGrade.Rare:
                expValue = 5;
                break;
        }
        settedExp -= expValue;

        if (overExp > 0)
        {
            overExp -= expValue;

            if (overExp < 0)
                overExp = 0;
            if (overExp == 0)
                rectTransform.sizeDelta = new Vector2(100f, rectTransform.sizeDelta.y);
        }
        imageExpOver.fillAmount = (float)overExp / ItemManager.itemManager.GetNeedExp(ItemGrade.Rare);
        imageExpect.fillAmount = (float)settedExp / needExp;
    }
    public bool CalcGradeExp(out ItemGrade _itemGrade, out int _exp)
    {
        if (currentGrade == ItemGrade.Normal)
        {
            rectTransform.sizeDelta = new Vector2(100f, rectTransform.sizeDelta.y);
        }
        bool isUpgraded = false;
        imageExpOver.fillAmount = imageExpect.fillAmount = 0f;
        if (overExp == ItemManager.itemManager.GetNeedExp(ItemGrade.Rare))//이단업
        {
            currentGrade = ItemGrade.Unique;
            settedExp = 0;
            isUpgraded = true;
            StartCoroutine(FillCoroutine(1f, true));
        }
        else
        {
            int needExp = ItemManager.itemManager.GetNeedExp(currentGrade);
            if (settedExp >= needExp)
            {
                settedExp -= needExp;
                if (currentGrade == ItemGrade.Normal)
                    currentGrade = ItemGrade.Rare;
                else if (currentGrade == ItemGrade.Rare)
                    currentGrade = ItemGrade.Unique;
                isUpgraded = true;
            }
            StartCoroutine(FillCoroutine((float)settedExp / ItemManager.itemManager.GetNeedExp(currentGrade), isUpgraded));

        }
        _itemGrade = currentGrade;
        _exp = settedExp;
        overExp = 0;

        return isUpgraded;
    }
    private IEnumerator FillBaseCoroutine(float _targetFillAmount)
    {
        float fillSpeed = 0.5f; // fillAmount가 증가하는 속도
        float initialFillAmount = imageBase.fillAmount;
        float totalChange = _targetFillAmount - initialFillAmount;
        float duration = Mathf.Abs(totalChange) / fillSpeed; // 전체 변화량을 속도로 나눠서 걸리는 시간 계산
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            imageBase.fillAmount = Mathf.Lerp(initialFillAmount, _targetFillAmount, elapsedTime / duration);

            yield return null;
        }
        imageBase.fillAmount = _targetFillAmount; // 최종적으로 정확한 값 설정
    }
    private IEnumerator FillCoroutine(float _targetFillAmount, bool _isRepeat)
    {
        isOperating = true;
        if (_isRepeat)
        {
            yield return StartCoroutine(FillBaseCoroutine(1f));
            yield return new WaitForSeconds(0.5f);
            rectTransform.sizeDelta = new Vector2(150f, rectTransform.sizeDelta.y);
            imageBase.fillAmount = 0f;
        }

        if (_targetFillAmount > 0)
        {
            yield return StartCoroutine(FillBaseCoroutine(_targetFillAmount));
        }

        if (currentGrade == ItemGrade.Unique)
        {
            imageExpect.fillAmount = 1f;
        }
        StartCoroutine(ItemManager.itemManager.upgradeSkillUi.UpdateSkillGrade(currentGrade));
        if (_targetFillAmount == 1f || (_isRepeat && _targetFillAmount == 0))
        {
            yield return new WaitForSeconds(0.5f);
            imageBase.fillAmount = 0f;
        }
        yield return new WaitForSeconds(1f);
        isOperating = false;
    }
}
