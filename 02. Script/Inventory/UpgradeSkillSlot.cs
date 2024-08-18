using EnumCollection;
using ItemCollection;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSkillSlot :SlotBase
{
    public Image imageGrade;
    public Image imageSkillBook;
    public TMP_Text textAmount;
    public CountableItem ci;
    public InventorySlot targetInventorySlot;
    private new void Awake()
    {
        base.Awake();
        imageGrade.gameObject.SetActive(false);
        textAmount.gameObject.SetActive(false);
        ci = null;
    }
    public void SetCi(CountableItem _ci, InventorySlot _targetInventorySlot)
    {
        imageHighlight.gameObject.SetActive(true);
        targetInventorySlot = _targetInventorySlot;

        ci = _ci;
        Sprite gradeSprite;
        SkillAsItem skill = (SkillAsItem)ci.item;
        if (ci.amount == 1)
            textAmount.gameObject.SetActive(false);
        else
        {
            textAmount.gameObject.SetActive(true);
            textAmount.text = ci.amount.ToString();
        }
        switch (skill.itemGrade)
        {
            default:
                gradeSprite = ItemManager.itemManager.item_None;
                break;
            case ItemGrade.Normal:
                gradeSprite = ItemManager.itemManager.item_Normal;
                break;
            case ItemGrade.Rare:
                gradeSprite = ItemManager.itemManager.item_Rare;
                break;
            case ItemGrade.Unique:
                gradeSprite = ItemManager.itemManager.item_Unique;
                break;
        }
        Sprite itemSprite;
        switch (skill.categori)
        {
            default:
                itemSprite = ItemManager.itemManager.book_P;
                break;
            case SkillCategori.Util:
                itemSprite = ItemManager.itemManager.book_U;
                break;
            case SkillCategori.Sustain:
                itemSprite = ItemManager.itemManager.book_S;
                break;
        }
        imageGrade.sprite = gradeSprite;
        imageSkillBook.sprite = itemSprite;
        imageGrade.gameObject.SetActive(true);
    }
    public void AddItemAmount()
    {
        textAmount.gameObject.SetActive(true);
        ci.amount++;
        textAmount.text = ci.amount.ToString();
    }
    public void OnPointerEnter()
    {
        if (ci != null)
        {
            HighlightOn();
            float xOffset = 40f;
            float yOffset = 100f;
            ItemManager.itemManager.inventoryUi.SetTooltipAtInventory(transform ,new Vector3(xOffset, yOffset), ci.item);
        }
    }
    public void OnPointerExit()
    {
        HighlightOff();
        ItemManager.itemManager.inventoryUi.tooltip.gameObject.SetActive(false);
    }
    public void OnPointerClick()
    {
        if (ci != null)
        {
            ItemManager.itemManager.upgradeSkillUi.skillExpBar.DecreaseExpectValue(ci.item.itemGrade);
            targetInventorySlot.textAmount.text = (++targetInventorySlot.ci.amount).ToString();
            if (ci.amount == 1)
            {
                ci = null;
                ItemManager.itemManager.upgradeSkillUi.RefreshSkillUpgradeSlots();
                if (ci == null)
                    SetHighLightAlphaZero();
            }
            else
            {
                ci.amount--;
                textAmount.text = ci.amount.ToString();
            }
        }
    }
    public void ClearSlot()
    {
        imageGrade.gameObject.SetActive(false);
        textAmount.gameObject.SetActive(false);
        SetHighLightAlphaZero();
        imageHighlight.gameObject.SetActive(false);
        ci = null;
        targetInventorySlot = null;
    }
}
