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
    public TMP_Text textNum;
    public int slotIndex;
    public GameObject imageNoSelect;
    public TMP_Text textPokerNum;
    private bool isSelected;
    private void Awake()
    {
        textPokerNum.transform.parent.gameObject.SetActive(false);
        isSelected = true;
    }
    public void SetSlot(CountableItem _ci)
    {
        imageItem.gameObject.SetActive(true);
        ci = _ci;
        Sprite gradeMat;
        switch (_ci.item.itemGrade)
        {
            default:
                gradeMat = ItemManager.itemManager.item_None;
                break;
            case ItemGrade.Normal:
                gradeMat = ItemManager.itemManager.item_Normal;
                break;
            case ItemGrade.Rare:
                gradeMat = ItemManager.itemManager.item_Rare;
                break;
            case ItemGrade.Unique:
                gradeMat = ItemManager.itemManager.item_Unique;
                break;
        }
        imageGrade.sprite = gradeMat;
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
                break;

        }
        imageGrade.gameObject.SetActive(true);

        if (ci.amount == 1)
            textNum.gameObject.SetActive(false);
        else
        {
            textNum.gameObject.SetActive(true);
            textNum.text = ci.amount.ToString();
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
            textNum.gameObject.SetActive(false);
        else
        {
            textNum.gameObject.SetActive(true);
            textNum.text = ci.amount.ToString();
        }
    }
    public void ClearSlot()
    {
        ci = null;
        textPokerNum.transform.parent.gameObject.SetActive(false);
        imageGrade.gameObject.SetActive(false);
        imageItem.gameObject.SetActive(false);
        textNum.gameObject.SetActive(false);
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
        if (!isSelected)
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
        if (!isSelected || ci == null)
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
        if (!isSelected)
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
        ItemType itemType = targetSlot.itemType;

        if (targetSlot.item != null)//교체하기
        {
            InventorySlot existingSlot = ItemManager.itemManager.GetExistingSlot(targetSlot.item);
            if (existingSlot == null)
            {
                if (curCi.amount == 1)
                {
                    SetSlot(new(targetSlot.item));
                }
                else
                    ItemManager.itemManager.SetItemToAbleIndex(new CountableItem(targetSlot.item));
            }
            else
                existingSlot.ChangeCiAmount(1);
        }
        else
        {
            if (ci.amount > 1)
            {
                ChangeCiAmount(-1);

            }
            else if (ci.amount == 1)
            {
                ClearSlot();
            }
        }

        targetSlot.SetSlot(curCi.item);
        CharacterData targetCharacter = ItemManager.itemManager.selectedCharacter;
        switch (itemType)
        {
            default:
                targetCharacter.ChangeWeapon(curCi.item as WeaponClass);
                break;
            case ItemType.Skill:
                targetCharacter.skills[targetSlot.index] = curCi.item as Skill;
                break;
        }

        targetSlot.HightlightOff();

        if (!targetCharacter.skills.Contains(null))
        {
            ItemManager.itemManager.inventoryUi.jobSlot.buttonExclaim.SetActive(true);
        }
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
        if (ItemManager.itemManager.inventoryUi.draggingSlot !=null)
        {
            ItemManager.itemManager.inventoryUi.targetInventorySlot = this;
        }
        else
        {
            ItemManager.itemManager.inventoryUi.targetInventorySlot = null;
        }
        int row = slotIndex / 5;
        float yOffset = 0f;
        switch (row)
        {
            case 3:
                yOffset += 40f;
                break;
            case 4:
                yOffset += 80f;
                break;
        }
        int column = slotIndex % 5;
        float xOffset = 0f;
        switch (column)
        {
            case 3:
                xOffset -= 40f;
                break;
            case 4:
                xOffset -= 80f;
                break;
        }
        if (ci != null&&!ItemManager.itemManager.inventoryUi.draggingSlot)
        {

            ItemManager.itemManager.inventoryUi.SetTooltipAtInventory(transform.parent.parent, transform.localPosition +  new Vector3(xOffset,yOffset), ci.item);
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
        if (!isSelected || ci == null)
            return;
        PointerEventData pointerData = (PointerEventData)data;

        // 우클릭 감지
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            switch (ci.item.itemType)
            {
                case ItemType.Weapon:
                    if (ItemManager.itemManager.inventoryUi.equipSlots[2].item == null)
                    {
                        
                    }
                    break;
                case ItemType.Skill:
                    if (ItemManager.itemManager.inventoryUi.equipSlots[0].item == null)
                    {
                    
                    }
                    else if (ItemManager.itemManager.inventoryUi.equipSlots[1].item == null)
                    {
                    
                    }
                    else
                    {
                    
                    }
                    break;
            }
        }
    }
}
