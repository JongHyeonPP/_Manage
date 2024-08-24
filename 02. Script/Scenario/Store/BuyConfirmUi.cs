using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyConfirmUi : MonoBehaviour
{
    private GoodsSlot goodsSlot;
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textPrice;
    [SerializeField] TMP_Text textConfirm;
    [SerializeField] TMP_Text textReturn;
    [SerializeField] Image imageGrade;
    [SerializeField] Image imageItem;
    private StoreUi storeUi;
    private void Start()
    {
        storeUi = GameManager.storeScenario.storeUi;
        textConfirm.text = GameManager.language == Language.Ko ? "�����ϱ�" : "Buy";
        textReturn.text = GameManager.language == Language.Ko ? "���ư���" : "Go back";
    }
    public void SetItemPrice(GoodsSlot _goodsSlot)
    {
        goodsSlot = _goodsSlot;
        Item item = goodsSlot.item;
        int price = goodsSlot.price;
        item.SetSpriteToImage(imageItem);
        textExplain.text = GameManager.language == Language.Ko ? "���� �����Ͻðڽ��ϱ�?" : "Are you sure you want to buy?";
        textName.text = item.name[GameManager.language];
        switch (item.itemGrade)
        {
            case ItemGrade.Normal:
                imageGrade.sprite = ItemManager.itemManager.item_Normal;
                break;
            case ItemGrade.Rare:
                imageGrade.sprite = ItemManager.itemManager.item_Rare;
                break;
            case ItemGrade.Unique:
                imageGrade.sprite = ItemManager.itemManager.item_Unique;
                break;
        }
        if (item.itemType == ItemType.Skill)
        {
            string gradeStr = string.Empty;
            switch (item.itemGrade)
            {
                case ItemGrade.Normal:
                    gradeStr = GameManager.language == Language.Ko ? "�븻" : "Normal";
                    break;
                case ItemGrade.Rare:
                    gradeStr = GameManager.language == Language.Ko ? "����" : "Rare";
                    break;
                case ItemGrade.Unique:
                    gradeStr = GameManager.language == Language.Ko ? "����ũ" : "Unique";
                    break;
            }
            textName.text += $" ({gradeStr})";
        }
        textPrice.text = price.ToString();
    }
    private void OnEnable()
    {
        GameManager.storeScenario?.storeUi.raycastBlock.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        GameManager.storeScenario.storeUi.raycastBlock.gameObject.SetActive(false);
    }
    public async void OnConfirmButtonClick()
    {
        Item item = goodsSlot.item;
        int price = goodsSlot.price;
        InventorySlot_Store slot = null;
        bool isSuccess = false;
        if (item.itemType == ItemType.Skill)
            slot = storeUi.GetExistSlot(item);
        if (slot == null)//�� ���Կ� �ֱ�
        {
            storeUi.ConnectEmptySlot(item);
            goodsSlot.SoldOut();
            isSuccess = true;
        }
        if (slot != null)//�̹� �ִ� ���Կ� �ֱ�
        {
            slot.SetAmountResult(1);
            goodsSlot.SoldOut();
            isSuccess = true;
        }
        if (isSuccess)
        {
            GameManager.gameManager.ChangeGold(-price);
            GameManager.storeScenario.SetStoreAtDb();
            await ItemManager.itemManager.SetInventoryAtDb();
        }
        else
        {
            string popUpMessage = GameManager.language == Language.Ko ? "������ �������� �������� ���߽��ϴ�." : "couldn't buy it because bag is full.";
            GameManager.gameManager.SetPopUp(popUpMessage);
        }
        gameObject.SetActive(false);
    }
}