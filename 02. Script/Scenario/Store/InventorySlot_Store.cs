using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot_Store : SlotBase
{
    StoreUi storeUi;
    public InventorySlot connectedSlot { get; private set; }
    public int slotIndex;
    //Ui
    public Transform panelBack;
    public Image imageGrade;
    public Image imageItem;
    public TMP_Text textAmount;
    public GameObject imageNoSelect;
    public TMP_Text textPokerNum;
    private bool isSelected;
    private void Start()
    {
        storeUi = GameManager.storeScenario.storeUi;
    }
    public void SetSlot(InventorySlot _connectedSlot)
    {
        connectedSlot = _connectedSlot;
        if (_connectedSlot.ci == null)
        {
            panelBack.gameObject.SetActive(false);
        }
        else
        {
            panelBack.gameObject.SetActive(true);
            connectedSlot = _connectedSlot;
            imageGrade.sprite = _connectedSlot.imageGrade.sprite;
            imageItem.sprite = _connectedSlot.imageItem.sprite;
            imageItem.transform.localScale = _connectedSlot.imageItem.transform.localScale;
            int amount = _connectedSlot.ci.amount;
            SetAmount(amount);
            imageNoSelect.SetActive(false);
            if (_connectedSlot.textPokerNum.transform.parent.gameObject.activeSelf)
            {
                textPokerNum.transform.parent.gameObject.SetActive(true);
                textPokerNum.text = _connectedSlot.textPokerNum.text;
                textPokerNum.color = _connectedSlot.textPokerNum.color;
            }
            else
            {
                textPokerNum.transform.parent.gameObject.SetActive(false);
            }
        }
    }
    private void SetAmount(int _amount)
    {
        if (_amount == 1)
            textAmount.gameObject.SetActive(false);
        else
        {
            textAmount.gameObject.SetActive(true);
            textAmount.text = _amount.ToString();
        }
    }

    public void SetSelected(bool _isSelect)
    {
        isSelected = _isSelect;
        imageNoSelect.SetActive(!isSelected);
    }
    public void OnDrag()
    {
        if (!isSelected || connectedSlot.ci == null)
            return;

        // 마우스의 현재 위치를 기준으로 이동
        Vector3 mousePosition = Input.mousePosition;

        // ItemManager.itemManager.inventoryUi의 RectTransform을 기준으로 로컬 좌표 계산
        RectTransform inventoryRectTransform = storeUi.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryRectTransform,
            mousePosition,
            Camera.main,
            out localPoint
        );

        // 이미지의 로컬 좌표를 설정
        imageGrade.transform.localPosition = localPoint;
    }
    public void OnBeginDrag()
    {
        if (!isSelected || connectedSlot.ci == null)
            return;
        imageGrade.transform.SetParent(storeUi.transform, true);
        storeUi.draggingSlot = this;
        storeUi.imageSell.gameObject.SetActive(true);
    }
    public void OnEndDrag()
    {
        if (!isSelected || connectedSlot.ci == null)
            return;
        storeUi.draggingSlot = null;
        imageGrade.transform.parent = panelBack;
        imageGrade.transform.localPosition = Vector3.zero;
        storeUi.imageSell.gameObject.SetActive(false);
        HighlightOff();
    }
    public void OnEnterSlot()
    {
        if (storeUi.draggingSlot == null)
        {
            HighlightOn();

            if (connectedSlot.ci != null)
            {
                int row = slotIndex / 6;
                int column = slotIndex % 6;
                float xOffset = 0f;
                float yOffset = 0f;
                switch (column)
                {
                    case 4:
                        xOffset -= 40f;
                        break;
                    case 5:
                        xOffset -= 80f;
                        break;
                }
                switch (row)
                {
                    case 4:
                        yOffset += 40f;
                        break;
                    case 5:
                        yOffset += 80f;
                        break;
                }
                var tooltip = GameManager.storeScenario.storeTooltip;
                RectTransform inventoryRectTransform = storeUi.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
        GetComponent<RectTransform>(),
        transform.position,
        GameManager.startScenario.uiCamera,
        out var worldPoint
        );
                tooltip.transform.parent = transform;
                tooltip.gameObject.SetActive(true);
                tooltip.SetTooltipInfo(connectedSlot.ci.item, new Vector2(xOffset, yOffset));
            }
        }
    }
    public void OnExitSlot()
    {
        if (storeUi.draggingSlot == null)
        {
            HighlightOff();
        }
    }
}
