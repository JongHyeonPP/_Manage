using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemCollection;
using EnumCollection;
using System;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.GridLayoutGroup;
using System.Security.Cryptography;
using System.Linq;

public class InventorySlot : SlotBase
{
    public CountableItem ci;

    public Transform panelBack;
    public Image imageGrade;
    public Image imageItem;
    public TMP_Text textAmount;
    public int slotIndex;
    public GameObject imageNoSelect;
    public TMP_Text textPokerNum;
    public bool isSelected;
    private new void Awake()
    {
        base.Awake();
        isSelected = true;
    }
    public void SetSlot(CountableItem _ci)
    {
        imageItem.gameObject.SetActive(true);
        ci = _ci;
        Sprite gradeSprite;
        if (_ci.item.itemType == ItemType.Food)
        {
            PokerCombination pokerCombination = ((FoodClass)_ci.item).pokerCombination;
            switch (pokerCombination)
            {
                default://High Level, OnePair, TwoPair
                    gradeSprite = ItemManager.itemManager.item_None;
                    break;
                case PokerCombination.ThreeOfAKind:
                case PokerCombination.Straight:
                    gradeSprite = ItemManager.itemManager.item_Normal;
                    break;
                case PokerCombination.Flush:
                case PokerCombination.FullHouse:
                    gradeSprite = ItemManager.itemManager.item_Rare;
                    break;
                case PokerCombination.FourOfAKind:
                case PokerCombination.StraightFlush:
                    gradeSprite = ItemManager.itemManager.item_Unique;
                    break;
            }
        }
        else
        {
            switch (_ci.item.itemGrade)
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
        }
        imageGrade.sprite = gradeSprite;
        Sprite itemSprite;
        switch (ci.item.itemType)
        {
            default:
                ci.item.SetSpriteToImage(imageItem);
                break;
            case ItemType.Skill:
                switch (ci.item.itemId.Split("_")[0])
                {
                    default:
                        itemSprite = ItemManager.itemManager.book_P;
                        break;
                    case "Util":
                        itemSprite = ItemManager.itemManager.book_U;
                        break;
                    case "Sustain":
                        itemSprite = ItemManager.itemManager.book_S;
                        break;
                }
                imageItem.transform.localScale = Vector2.one * 0.7f;
                imageItem.sprite = itemSprite;
                imageItem.transform.localPosition = Vector3.zero;
                break;

        }
        imageGrade.gameObject.SetActive(true);

        if (ci.amount == 1)
            textAmount.gameObject.SetActive(false);
        else
        {
            textAmount.gameObject.SetActive(true);
            textAmount.text = ci.amount.ToString();
        }

        if (ci.item.itemType == ItemType.Ingredient)
        {
            textPokerNum.transform.parent.gameObject.SetActive(true);
            IngredientClass ingredient = (IngredientClass)ci.item;
            textPokerNum.text = ingredient.pokerNum.ToString();
            textPokerNum.color = ingredient.GetPokerNumColor();
        }
        else
        {
            textPokerNum.transform.parent.gameObject.SetActive(false);
        }

    }
    public void ChangeCiAmount(int _value)
    {
        ci.amount += _value;
        if (ci.amount == 1)
            textAmount.gameObject.SetActive(false);
        else if(ci.amount == 0)
            ClearSlot();
        else
        {
            textAmount.gameObject.SetActive(true);
            textAmount.text = ci.amount.ToString();
        }
    }
    public void ClearSlot()
    {
        ci = null;
        textPokerNum.transform.parent.gameObject.SetActive(false);
        imageGrade.gameObject.SetActive(false);
        imageItem.gameObject.SetActive(false);
        textAmount.gameObject.SetActive(false);
        imageHighlight.gameObject.SetActive(true);
        imageHighlight.color = new Color(
                imageHighlight.color.r,
                imageHighlight.color.g,
                imageHighlight.color.b,
                0f
            );
    }

    public void OnBeginDrag()
    {
        if (ItemManager.itemManager.isUpgradeCase || !isSelected)
            return;
        if (ci == null) return;
        ItemManager.itemManager.inventoryUi.throwReady = false;
        imageGrade.raycastTarget = false;
        imageItem.raycastTarget = false;
        imageHighlight.raycastTarget=false;
        ItemManager.itemManager.inventoryUi.draggingSlot = this;
    }

    public void OnDrag()
    {
        if (ItemManager.itemManager.isUpgradeCase || !isSelected)
            return;

        // 부모를 ItemManager.itemManager.inventoryUi로 변경
        imageGrade.transform.SetParent(ItemManager.itemManager.inventoryUi.transform, true);

        // 마우스의 현재 위치를 기준으로 이동
        Vector3 mousePosition = Input.mousePosition;

        // ItemManager.itemManager.inventoryUi의 RectTransform을 기준으로 로컬 좌표 계산
        RectTransform inventoryRectTransform = ItemManager.itemManager.inventoryUi.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryRectTransform,
            mousePosition,
            GameManager.gameManager.uiCamera,
            out localPoint
        );

        // 이미지의 로컬 좌표를 설정
        imageGrade.transform.localPosition = localPoint;
    }

    public void OnEndDrag()
    {
        if (ItemManager.itemManager.isUpgradeCase || !isSelected)
            return;
        //인벤토리에서 교환
        if (ItemManager.itemManager.inventoryUi.targetInventorySlot)
        {
            InventorySlot targetSlot = ItemManager.itemManager.inventoryUi.targetInventorySlot;
            CountableItem curCi = ci;

            if (targetSlot.ci != null)//교체하기
            {
                SetSlot(targetSlot.ci);
            }
            else//이동만 하기
            {
                ClearSlot();
            }
            targetSlot.SetSlot(curCi);
            targetSlot.HightlightOff();
        }
        //장비칸과 교환
        else if (ItemManager.itemManager.inventoryUi.targetEquipSlot)
        {
            SwapWithEquip();
        }
        //버리기
        else if (ItemManager.itemManager.inventoryUi.throwReady)
        {
            ItemManager.itemManager.inventoryUi.throwSlot = ItemManager.itemManager.inventoryUi.draggingSlot;
            ItemManager.itemManager.inventoryUi.throwReady = false;
            ItemManager.itemManager.inventoryUi.panelThrow.gameObject.SetActive(true);
            ItemManager.itemManager.inventoryUi.statusExplain.gameObject.SetActive(false);
        }
        ItemManager.itemManager.inventoryUi.draggingSlot = null;

        ItemManager.itemManager.inventoryUi.targetInventorySlot = null;
        ItemManager.itemManager.inventoryUi.targetEquipSlot = null;
        SetOriginLocation();
    }

    private void SwapWithEquip()
    {
        EquipSlot targetSlot = ItemManager.itemManager.inventoryUi.targetEquipSlot;
        if (targetSlot.item != null)
            if (targetSlot.item.itemId == ci.item.itemId)
            {
                SetOriginLocation();
                return;
            }
        CountableItem curCi = ci;
        if (ci.item.itemType == ItemType.Weapon)
        {
            SetSlot( new(targetSlot.item));
        }
        else if (ci.item.itemType == ItemType.Skill)
        {
            if (targetSlot.item != null)//교체하기
            {
                InventorySlot existingSlot = ItemManager.itemManager.GetExistingSlot(targetSlot.item);
                    ChangeCiAmount(-1);
                if (existingSlot == null)
                {
                    if (ci == null)
                    {
                        SetSlot(new(targetSlot.item));
                    }
                    else
                    {
                        ItemManager.itemManager.SetItemToAbleIndex(new CountableItem(targetSlot.item));
                    }
                }
                else
                    existingSlot.ChangeCiAmount(1);
            }
            else
            {
                    ChangeCiAmount(-1);
            }
        }

        ItemType itemType = targetSlot.itemType;



        targetSlot.SetSlot(curCi.item);
        CharacterData targetCharacter = ItemManager.itemManager.selectedCharacter;
        switch (itemType)
        {
            case ItemType.Weapon:
                WeaponClass newWeapon = curCi.item as WeaponClass;
                WeaponClass previousWeapon = targetCharacter.weapon;
                float maxHp = newWeapon.hp - previousWeapon.hp;
                float ability = newWeapon.ability - previousWeapon.ability;
                float resist = newWeapon.resist - previousWeapon.resist;
                float speed = newWeapon.speed - previousWeapon.resist;
                ItemManager.itemManager.inventoryUi.parentStatusUp.StartShowTextsStatus(0f, maxHp, ability, resist, speed);
                targetCharacter.ChangeWeapon(curCi.item as WeaponClass);
                break;
            case ItemType.Skill:
                targetCharacter.skillAsItems[targetSlot.index] = curCi.item as SkillAsItem;
                if (!targetCharacter.skillAsItems.Contains(null))
                {
                    ItemManager.itemManager.inventoryUi.jobSlot.buttonExclaim.SetActive(true);
                }
                break;
        }

        targetSlot.HightlightOff();

    }

    private void SetOriginLocation()
    {
        imageGrade.transform.SetParent(panelBack);
        imageGrade.transform.localPosition = Vector3.zero;
    }

    public void OnPointerEnter()
    {
        if (!isSelected)
            return;
        HightlightOn();
        if (ItemManager.itemManager.inventoryUi.draggingSlot != null)
        {
            ItemManager.itemManager.inventoryUi.targetInventorySlot = this;
        }
        else
        {
            ItemManager.itemManager.inventoryUi.targetInventorySlot = null;
        }
        int row = slotIndex / 6;

        int column = slotIndex % 6;
        float xOffset = 0f;
        switch (column)
        {
            case 4:
                xOffset -= 40f;
                break;
            case 5:
                xOffset -= 80f;
                break;
        }

        if (ci != null && !ItemManager.itemManager.inventoryUi.draggingSlot)
        {

            ItemManager.itemManager.inventoryUi.SetTooltipAtInventory(transform, new Vector2(xOffset, 40f), ci.item);
        }
    }



    public void OnPointerExit()
    {
        if (!isSelected)
            return;
        HightlightOff();
        ItemManager.itemManager.inventoryUi.tooltip.gameObject.SetActive(false);
    }
    public void SetSelected(bool _isSelect)
    {
        isSelected = _isSelect;
        imageNoSelect.SetActive(!isSelected);
    }
    public void OnPointerClick(BaseEventData data)
    {
        if (ItemManager.itemManager.isUpgradeCase)
        {
            SelectInUpgradeCase();
        }
        else
        {
            if (!isSelected || ci == null)
                return;
            PointerEventData pointerData = (PointerEventData)data;

            // 우클릭 감지
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                switch (ci.item.itemType)
                {
                    case ItemType.Weapon:
                        ItemManager.itemManager.inventoryUi.targetEquipSlot = ItemManager.itemManager.inventoryUi.equipSlots[2];
                        break;
                    case ItemType.Skill:
                        if (ItemManager.itemManager.inventoryUi.equipSlots[0].item == null)
                        {
                            ItemManager.itemManager.inventoryUi.targetEquipSlot = ItemManager.itemManager.inventoryUi.equipSlots[0];
                        }
                        else if (ItemManager.itemManager.inventoryUi.equipSlots[1].item == null)
                        {
                            ItemManager.itemManager.inventoryUi.targetEquipSlot = ItemManager.itemManager.inventoryUi.equipSlots[1];
                        }
                        break;
                    case ItemType.Food:
                        Debug.Log("요리 클릭클릭");
                        break;
                }
                if (ItemManager.itemManager.inventoryUi.targetEquipSlot)
                {
                    SwapWithEquip();
                    ItemManager.itemManager.inventoryUi.targetEquipSlot = null;
                }
            }
        }

    }

    private void SelectInUpgradeCase()
    {
        if (ci.amount == 0)
            return;
        if (!ItemManager.itemManager.upgradeSkillUi.SetItemToUpgradeSlot(ci.item, this))
            return;
        ci.amount--;
        textAmount.text = ci.amount.ToString();
    }
}
