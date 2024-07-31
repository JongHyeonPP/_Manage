using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeSkillUi : MonoBehaviour
{
    private InventoryUi inventoryUi;
    public Image imageSkill;
    public SkillExpBar skillExpBar;
    public TMP_Text textSkillGrade;
    public List<UpgradeSkillSlot> slots;
    private int skillIndex;
    private void Awake()
    {
        inventoryUi = ItemManager.itemManager.inventoryUi;
    }
    public void InitUpgradeSkillUi(SkillAsItem _skillAsItem, int _index)
    {
        skillIndex = _index;
        CharacterData character = ItemManager.itemManager.selectedCharacter;
        imageSkill.sprite = _skillAsItem.sprite;
        int expValue = character.exp[skillIndex];
        skillExpBar.SetBaseValue(expValue, _skillAsItem.itemGrade);
        inventoryUi.UpgradeCase(_skillAsItem.categori);
        string skillGradeStr = string.Empty;
        for (int i = 0; i <= (int)_skillAsItem.itemGrade; i++)
        {
            skillGradeStr += 'I';
        }
        textSkillGrade.text = skillGradeStr;
    }
    private void OnEnable()
    {
        if (!ItemManager.itemManager)
            return;
        ItemManager.itemManager.isUpgradeCase = true;
        ItemManager.itemManager.backgroundInventoryAdd.SetActive(true);
        inventoryUi.SetCanvasForPanelInventory();
    }
    private void OnDisable()
    {
        ItemManager.itemManager.backgroundInventoryAdd.SetActive(false);
        Destroy(ItemManager.itemManager.inventoryUi.panelInventory.GetComponent<GraphicRaycaster>());
        Destroy(ItemManager.itemManager.inventoryUi.panelInventory.GetComponent<Canvas>());
        ItemManager.itemManager.isUpgradeCase = false;
        inventoryUi.SelectButtonSelect(inventoryUi.currentSelectButton);

        초기화하기
    }
    public bool SetItemToUpgradeSlot(Item _item, InventorySlot _targetInventorySlot)
    {
        if (!skillExpBar.IncreaseExpectValue(_item.itemGrade))
        {
            GameManager.gameManager.SetPopUp("더 이상 강화할 수 없습니다.");
            return false;
        }
        UpgradeSkillSlot existSlot = slots.Where(data => data.ci != null).Where(data=>data.ci.item.itemId == _item.itemId).FirstOrDefault();
        if (existSlot == null)
        {
            UpgradeSkillSlot emptySlot = slots.Where(data => data.ci == null).FirstOrDefault();
            if (emptySlot == null)
            {
                GameManager.gameManager.SetPopUp("슬롯이 다 찼습니다.");
                return false;
            }
            emptySlot.SetCi(new CountableItem(_item), _targetInventorySlot);
        }
        else
        {
            existSlot.AddItemAmount();
        }
        return true;
    }

    public void RefreshSkillUpgradeSlots()
    {
        IEnumerable<UpgradeSkillSlot> notNullSlots = slots.Where(data => data.ci != null);
        List<CountableItem> cies = notNullSlots.Select(data => data.ci).ToList();
        List<InventorySlot> targets = notNullSlots.Select(data => data.targetInventorySlot).ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < cies.Count)
            {
                slots[i].SetCi(cies[i], targets[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
    public void OnUpgradeButtonClicked()
    {
        if (slots.Where(item => item.ci != null).Count() == 0)
        {
            Debug.Log("재료 없음");
            return;
        }
        CharacterData character = ItemManager.itemManager.selectedCharacter;
        if (skillExpBar.CalcGradeExp(out ItemGrade itemGrade, out int exp))
        {
            SkillAsItem currentSkill = character.skillAsIItems[skillIndex];
            Skill newSkill = LoadManager.loadManager.skillsDict[currentSkill.itemId];
            SkillAsItem newSkillAsItem = newSkill.GetAsItem((int)itemGrade);
            character.skillAsIItems[skillIndex] = newSkillAsItem;
            inventoryUi.equipSlots[skillIndex].SetSlot(newSkillAsItem);
            string skillGradeStr = string.Empty;
            for (int i = 0; i <= (int)newSkillAsItem.itemGrade; i++)
            {
                skillGradeStr += 'I';
            }
            textSkillGrade.text = skillGradeStr;
        }
        inventoryUi.equipSlots[skillIndex].SetExp(exp);
        character.exp[skillIndex] = exp;

        foreach (UpgradeSkillSlot slot in slots.Where(data=>data.targetInventorySlot!=null))
        {
            if (slot.targetInventorySlot.ci.amount == 0)
            {
                slot.targetInventorySlot.ci = null;
            }
            slot.ClearSlot();
        }
        {
            FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
            {
                ItemManager.itemManager.SetCharacterAtDb();
                ItemManager.itemManager.SetInventoryAtDb();
            });
        }
    }
}
