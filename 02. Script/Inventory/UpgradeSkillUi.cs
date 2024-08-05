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
        inventoryUi.UnsetUpgradeMode();
        inventoryUi.SelectButtonSelect(inventoryUi.currentSelectButton);
        ResetUpgradeSkillUi();

    }
    public void ResetUpgradeSkillUi()
    {
        foreach (UpgradeSkillSlot slot in slots.Where(data => data.targetInventorySlot))
        {
            slot.targetInventorySlot.ChangeCiAmount(slot.ci.amount);
            for (int i = 0; i < slot.ci.amount; i++)
            {
                skillExpBar.DecreaseExpectValue(slot.ci.item.itemGrade);
            }
            slot.ClearSlot();
        }
    }
    public bool SetItemToUpgradeSlot(Item _item, InventorySlot _targetInventorySlot)
    {
        if (skillExpBar.isOperating)
        {
            Debug.Log("강화 진행 중");
            return false;
        }
        if (!skillExpBar.IncreaseExpectValue(_item.itemGrade))
        {
            GameManager.gameManager.SetPopUp("더 이상 강화할 수 없습니다.");
            return false;
        }
        UpgradeSkillSlot existSlot = slots.Where(data => data.ci != null).Where(data => data.ci.item.itemId == _item.itemId).FirstOrDefault();
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
        }
        inventoryUi.equipSlots[skillIndex].SetExp(exp);
        character.exp[skillIndex] = exp;

        foreach (UpgradeSkillSlot slot in slots.Where(data => data.targetInventorySlot != null))
        {
            if (slot.targetInventorySlot.ci.amount == 0)
            {
                slot.targetInventorySlot.ChangeCiAmount(0);
            }
            slot.ClearSlot();
        }
        {
            //FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
            //{
            //    ItemManager.itemManager.SetCharacterAtDb();
            //    ItemManager.itemManager.SetInventoryAtDb();
            //});
        }
    }

    public IEnumerator UpdateSkillGrade(ItemGrade _grade)
    {
        // 1에서 0으로 알파값 변경
        float duration = 0.5f;
        float elapsedTime = 0f;
        Color color = textSkillGrade.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            textSkillGrade.color = color;
            yield return null;
        }

        // 원래 메서드 내용 수행
        string skillGradeStr = string.Empty;
        for (int i = 0; i <= (int)_grade; i++)
        {
            skillGradeStr += 'I';
        }
        textSkillGrade.text = skillGradeStr;

        // 0에서 1로 알파값 변경
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            textSkillGrade.color = color;
            yield return null;
        }

        color.a = 1f;
        textSkillGrade.color = color;
    }
}
