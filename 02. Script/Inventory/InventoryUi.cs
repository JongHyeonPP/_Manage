using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;

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
    public InventoryTooltip tooltip;
    public List<EquipSlot> equipSlots;//스킬 0, 스킬 1, 무기
    public Transform parentItemTypeSelect;
    [SerializeField]private List<SelectButton> selectButtons = new();
    private void Awake()
    {
        tooltip.gameObject.SetActive(false);
    }
    private void Start()
    {
        SetCharacterAtInventory(0);
        for (int i = 0; i < 5; i++)
        {
            selectButtons.Add(parentItemTypeSelect.GetChild(i).GetComponent<SelectButton>());
        }
        OnSelectButtonClicked(selectButtons[0]);
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
    public void SetInventorySlot(CountableItem _ci, int _index)
    {
        inventorySlots[_index].SetSlot(_ci);
    }

    public void SetCharacterAtInventory(int _index)
    {
        ItemManager.itemManager.selectedCharacterIndex = _index;
        CharacterData character = GameManager.gameManager.characterList[_index];
        if (character != null)
        {
            float maxHp = character.maxHp;
            float hp = character.hp;
            float ability = character.ability;
            float resist = character.resist;
            float speed =  character.speed;
            hpText.text = hp.ToString("F0") +"/" + maxHp.ToString("F0");
            abilityText.text = ability.ToString("F0");
            resistText.text = resist.ToString("F0");
            speedText.text = speed.ToString("F1");
            ch.CopyHierarchySprite(character.characterHierarchy);
        }
        for (int i = 0; i < characterSelectFocus.Count; i++)
        {
            characterSelectFocus[i].SetActive(i == _index);
        }
        equipSlots[0].SetSlot(character.skills[0]);
        equipSlots[1].SetSlot(character.skills[1]);
        equipSlots[2].SetSlot(character.weapon);
    }

    public void SetEquipSlot(Item _item, int _index)
    {
        equipSlots[_index].SetSlot(_item);
    }
    public void OnSelectButtonClicked(SelectButton _selectButton)
    {
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
    public void SetTooltipAtInventory(Transform _parent, Vector3 _localPosition, Item _item)
    {
        tooltip.transform.SetParent(_parent);
        tooltip.gameObject.SetActive(true);
        tooltip.transform.localPosition = _localPosition;// + new Vector3(25, -15, 0f);
        tooltip.SetTooltipInfo(_item);
    }
}
