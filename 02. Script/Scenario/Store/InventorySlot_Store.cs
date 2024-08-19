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
        GameManager.storeScenario.storeTooltip.gameObject.SetActive(false);
        storeUi.sellReady = false;
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
        if (storeUi.sellReady)
        {
            storeUi.SetSellUi(this);
        }
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
                        xOffset -= 30f;
                        break;
                    case 5:
                        xOffset -= 60f;
                        break;
                }
                switch (row)
                {
                    case 4:
                        yOffset += 1f;
                        break;
                    case 5:
                        yOffset += 2f;
                        break;
                }
                ItemTooltip tooltip = GameManager.storeScenario.storeTooltip;
                tooltip.gameObject.SetActive(true);

                // 마우스 위치를 로컬 좌표로 변환
                RectTransform tooltipRectTransform = tooltip.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    storeUi.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    GameManager.storeScenario.overlayCamera,
                    out var localPoint
                );

                // 로컬 좌표 기반으로 Tooltip 위치 설정
                tooltip.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                tooltip.rectTransform.anchorMax = new Vector2(0f, 0.5f);
                tooltip.rectTransform.pivot = new Vector2(0f, 0.5f);
                tooltip.rectTransform.localPosition = localPoint + new Vector2(xOffset, yOffset);

                tooltip.SetTooltipInfo(connectedSlot.ci.item);
            }
        }
    }

    public void OnExitSlot()
    {
        if (storeUi.draggingSlot == null)
        {
            HighlightOff();
            GameManager.storeScenario.storeTooltip.gameObject.SetActive(false);
        }
    }

    public void SetAmountResult(int _amount)
    {
        connectedSlot.ChangeCiAmount(-_amount);
        if (connectedSlot.ci == null)
        {
            panelBack.gameObject.SetActive(false);
        }
        else
        {
            textAmount.text = connectedSlot.ci.amount.ToString();
        }
    }
}
