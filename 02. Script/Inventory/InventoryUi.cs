using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;
using BattleCollection;
using System.Linq;
using Firebase.Firestore;
using System.Threading.Tasks;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    //Inventory
    public Transform parentSlot;
    public List<InventorySlot> inventorySlots { get; private set; }
    //Equip
    public CharacterHierarchy ch;
    public TMP_Text hpText;
    public TMP_Text abilityText;
    public TMP_Text resistText;
    public TMP_Text speedText;
    public List<GameObject> characterSelectFocus = new();
    public ItemTooltip tooltip;
    public StatusExplain statusExplain;
    public List<EquipSlot> equipSlots;//스킬 0, 스킬 1, 무기
    public Transform parentItemTypeSelect;
    public List<SelectButton> selectButtons = new();
    public bool throwReady { get; set; }
    public PanelThrow panelThrow;
    public GameObject panelInventory;
    public JobSlot jobSlot;
    public InventorySlot targetInventorySlot;
    public EquipSlot targetEquipSlot;
    public InventorySlot draggingSlot;
    public InventorySlot throwSlot;
    public SelectButton currentSelectButton;
    public ParentStatusUp parentStatusUp;
    public List<TalentSlot_Inventory> talentSlots;
    private void Awake()
    {
        tooltip.gameObject.SetActive(false);
        statusExplain.gameObject.SetActive(false);
        panelThrow.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        currentSelectButton = selectButtons[0];
        GameManager.gameManager.uiRaycastBlock.SetActive(true);
    }
    private void OnDisable()
    {
        GameManager.gameManager.uiRaycastBlock.SetActive(false);
    }
    private void Start()
    {
        SetCharacterAtInventory(0);
        SelectButtonSelect(selectButtons[0]);
    }
    public void InitInventory()
    {
        inventorySlots = new();
        for (int i = 0; i < ItemManager.inventorySize; i++)
        {
            InventorySlot slot = parentSlot.GetChild(i).GetComponent<InventorySlot>();
            inventorySlots.Add(slot);
            slot.slotIndex = i;
        }
        equipSlots[0].itemType = ItemType.Skill;
        equipSlots[1].itemType = ItemType.Skill;
        equipSlots[2].itemType = ItemType.Weapon;
        equipSlots[0].index = 0;
        equipSlots[1].index = 1;
        ClearInventory();
    }
    public void ClearInventory()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.ClearSlot();
        }
    }


    public void SetCharacterAtInventory(int _index)
    {
        CharacterData character = GameManager.gameManager.characterList[_index];
        if (character == null)
            return;
        ItemManager.itemManager.selectedCharacter = character;
        float maxHp = character.maxHp;
        float hp = character.hp;
        float ability = character.ability;
        float resist = character.resist;
        float speed = character.speed;
        hpText.text = hp.ToString("F0") + "/" + maxHp.ToString("F0");
        abilityText.text = ability.ToString("F0");
        resistText.text = resist.ToString("F0");
        speedText.text = speed.ToString("F1");
        ch.CopyHierarchySprite(character.characterHierarchy);

        for (int i = 0; i < characterSelectFocus.Count; i++)
        {
            characterSelectFocus[i].SetActive(i == _index);
        }
        equipSlots[0].SetSlot(character.skillAsItems[0]);
        equipSlots[1].SetSlot(character.skillAsItems[1]);
        equipSlots[2].SetSlot(character.weapon);
        if (character.jobClass.jobId != "000")
            for (int i = 0; i < 2; i++)
            {
                if (character.skillAsItems[i] == null)
                    continue;
                equipSlots[i].expBar.SetActive(true);
                equipSlots[i].SetExp(character.exp[i]);
            }
        else
        {
            for (int i = 0; i < 2; i++)
                equipSlots[i].expBar.SetActive(false);
        }

        switch (character.jobClass.jobId)
        {
            case "000":
                jobSlot.Case000(character);
                break;
            default:
                jobSlot.SetJobIcon(character.jobClass);
                break;

        }
        for (int i = 0; i < 3; i++)
        {
            if (i < character.talents.Count)
            {
                talentSlots[i].gameObject.SetActive(true);
                talentSlots[i].SetTalentSlot(character.talents[i]);
            }
            else
            {
                talentSlots[i].gameObject.SetActive(false);
            }
        }
    }



    public void SetEquipSlot(Item _item, int _index)
    {
        equipSlots[_index].SetSlot(_item);
    }
    public void SelectButtonSelect(SelectButton _selectButton)
    {
        currentSelectButton = _selectButton;
        foreach (SelectButton sb in selectButtons)
        {
            sb.ActiveHighlight(sb == _selectButton);
        }
        ItemType type = _selectButton.type;
        foreach (InventorySlot slot in inventorySlots)
        {
            bool isActive;
            if (type == ItemType.All)
                isActive = true;
            else
            {
                if (slot.ci == null)
                    isActive = true;
                else
                    isActive = slot.ci.item.itemType == type;
            }
            slot.SetSelected(isActive);
        }
    }
    public void UpgradeCase(SkillCategori _skillCategori)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            bool isActive = false;
            if (slot.ci != null && slot.ci.item.itemType == ItemType.Skill && ((SkillAsItem)slot.ci.item).categori == _skillCategori)
            {
                isActive = true;
                slot.textAmount.text = slot.ci.amount.ToString();
                slot.textAmount.gameObject.SetActive(true);
            }
            slot.SetSelected(isActive);
        }
        
    }
    public void SetTooltipAtInventory(Transform _parent, Vector3 _localPosition, Item _item)
    {
        tooltip.transform.SetParent(_parent);
        tooltip.gameObject.SetActive(true);
        tooltip.transform.localPosition = _localPosition;
        tooltip.SetTooltipInfo(_item);
    }
    public void EnterThrowZone()
    {
        if (draggingSlot)
        {
            throwReady = true;
        }
    }
    public void SetGetJobUi()
    {
        ItemManager.itemManager.SetGetJobUi();
    }
    public void InventorySorting()
    {
        List<CountableItem> ciList = new();
        List<CountableItem> sortedList = new();
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.ci != null)
                ciList.Add(slot.ci);
        }

        List<CountableItem> sortedWeaponList = ciList
            .Where(data => data.item.itemType == ItemType.Weapon)
            .Select(data => new { CountableItem = data, Weapon = (WeaponClass)data.item })
            .OrderBy(data => data.Weapon.itemGrade)
            .ThenBy(data => data.Weapon.weaponType)
            .ThenBy(data => data.Weapon.itemId)
            .Select(data => data.CountableItem)
            .ToList();

        List<CountableItem> sortedSkillList = ciList
    .Where(data => data.item.itemType == ItemType.Skill)
    .Select(data => new { CountableItem = data, Skill = (SkillAsItem)data.item })
    .OrderBy(data => data.Skill.categori)
    .ThenBy(data => data.Skill.itemId)
    .Select(data => data.CountableItem)
    .ToList();

        List<CountableItem> sortedIngredientList = ciList
    .Where(data => data.item.itemType == ItemType.Ingredient)
    .Select(data => new { CountableItem = data, Ingredient = (IngredientClass)data.item })
    .OrderBy(data => data.Ingredient.ingredientType)
    .ThenBy(data => data.Ingredient.itemGrade)
    .ThenBy(data => data.Ingredient.pokerNum)
    .Select(data => data.CountableItem)
    .ToList();

        List<CountableItem> sortedFoodList = ciList
    .Where(data => data.item.itemType == ItemType.Food)
    .Select(data => new { CountableItem = data, Food = (FoodClass)data.item })
    .OrderBy(data => data.Food.pokerCombination)
    .ThenBy(data => data.Food.itemId)
    .Select(data => data.CountableItem)
    .ToList();
        sortedList.AddRange(sortedWeaponList);
        sortedList.AddRange(sortedSkillList);
        sortedList.AddRange(sortedFoodList);
        sortedList.AddRange(sortedIngredientList);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (i < sortedList.Count)
                slot.SetSlot(sortedList[i]);
            else
                slot.ClearSlot();
            SelectButtonSelect(currentSelectButton);
        }
    }

    public async void InventoryActive()
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
        if (!isActive)
        {
            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
            {
                await ItemManager.itemManager.SetInventoryAtDb();
                await ItemManager.itemManager.SetCharacterAtDb();
                return Task.CompletedTask;
            });
        }
    }
    public void SetCanvasForPanelInventory()
    {
        var canvas = panelInventory.AddComponent<Canvas>();
        panelInventory.AddComponent<GraphicRaycaster>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 7;
    }
    public void UnsetUpgradeMode()
    {
        foreach (InventorySlot x in inventorySlots)
        {
            if (x.ci != null)
                if (x.isSelected)
                    if (x.ci.amount == 1)
                        x.ChangeCiAmount(0);
        }
    }
}
