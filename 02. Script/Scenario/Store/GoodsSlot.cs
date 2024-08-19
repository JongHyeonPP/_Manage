using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsSlot : SlotBase
{
    private Item item;
    private int price;
    [SerializeField] Image imageGrade;
    [SerializeField] Image imageItem;
    [SerializeField] TMP_Text textPrice;
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
        HighlightOn();
    }
    public void OnPointerExit()
    {
        HighlightOff();
    }
    public void OnButtonClick()
    {
        GameManager.storeScenario.storeUi.OnGoodsSlotClick(item, price);
    }
}
