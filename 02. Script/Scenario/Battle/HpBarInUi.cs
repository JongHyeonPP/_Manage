using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarInUi : HpBarBase
{
    public List<CooldownSlot> cooldownSlots;
    [SerializeField] CharacterHierarchy characterHierarchy;

    public void InitHpBarInUi(SkillAsItem[] _skillAsIItems, CharacterHierarchy _characterHierarchy)
    {
        for (int i = 0; i < cooldownSlots.Count; i++)
        {
            CooldownSlot slot = cooldownSlots[i];
            if (_skillAsIItems[i] != null)
            {
                slot.InitSkill(_skillAsIItems[i]);
                slot.gameObject.SetActive(true);
            }
            else
                slot.gameObject.SetActive(false);
        }
        characterHierarchy.CopyHierarchySprite(_characterHierarchy);
    }
}
