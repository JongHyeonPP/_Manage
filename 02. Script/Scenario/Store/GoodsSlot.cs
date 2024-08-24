using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsSlot : SlotBase
{
    public Item item;
    public int price;
    public bool isSoldOut;
    [SerializeField] Image imageGrade;
    [SerializeField] Image imageItem;
    public GameObject imageSoldOut;
    [SerializeField] TMP_Text textPrice;
    private ItemTooltip tooltip;
    private RectTransform rectTransform;


    private void Start()
    {
        tooltip = GameManager.storeScenario.storeTooltip;
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetItem(Item _item, int _price)
    {
        item = _item;
        price = _price;
        Sprite gradeSprite;
        switch (_item.itemGrade)
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
        if (_item.itemType == ItemType.Skill)
        {
            Sprite skillBookSprite;
            switch (((SkillAsItem)_item).categori)
            {
                default:
                    skillBookSprite = ItemManager.itemManager.book_P;
                    break;
                case SkillCategori.Util:
                    skillBookSprite = ItemManager.itemManager.book_U;
                    break;
                case SkillCategori.Sustain:
                    skillBookSprite = ItemManager.itemManager.book_S;
                    break;
            }
            imageItem.sprite = skillBookSprite;
            imageItem.transform.localScale = Vector3.one * 0.6f;
        }
        else
        {

            _item.SetSpriteToImage(imageItem);
        }
        
        imageGrade.sprite = gradeSprite;
        textPrice.text = _price.ToString();
    }
    public void OnPointerEnter()
    {
        tooltip.rectTransform.anchorMin = new Vector2(0f, 0.5f);
        tooltip.rectTransform.anchorMax = new Vector2(0f, 0.5f);
        tooltip.rectTransform.pivot = new Vector2(0f, 0.5f);
        tooltip.transform.position = transform.position;
        tooltip.transform.localPosition += new Vector3(rectTransform.sizeDelta.x / 2f + 3f, 0f);
        tooltip.SetTooltipInfo(item);
        tooltip.gameObject.SetActive(true);
        HighlightOn();

    }
    public void OnPointerExit()
    {
        tooltip.gameObject.SetActive(false);
        HighlightOff();
    }
    public void OnPointerClick()
    {
        if (isSoldOut)
            return;
        if (GameManager.gameManager.gold < price)
        {
            string popUpMessage = GameManager.language == Language.Ko ? "골드가 부족합니다." : "Not enough gold.";
            GameManager.gameManager.SetPopUp(popUpMessage);
        }
        else
        {
            GameManager.storeScenario.storeUi.OnGoodsSlotClick(this);
            tooltip.gameObject.SetActive(false);
        }
    }
    public void SoldOut()
    {
        isSoldOut = true;
        imageSoldOut.SetActive(true);
    }
}
