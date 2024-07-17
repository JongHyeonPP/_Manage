using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeSkillUi : MonoBehaviour
{
    public Image imageSkill;
    public Image imageFill;
    public void SetUpgradeSkillUi(Skill _skill, int _index)
    {
        CharacterData character = ItemManager.itemManager.selectedCharacter;
        float expValue = character.exp[_index];
        imageSkill.sprite = _skill.sprite;
        imageFill.fillAmount = expValue;
        ItemManager.itemManager.inventoryUi.UpgradeCase(_skill.categori);
    }
    private void OnEnable()
    {
        if (!ItemManager.itemManager)
            return;
        ItemManager.itemManager.isUpgradeCase = true;
        ItemManager.itemManager.backgroundInventoryAdd.SetActive(true);
        ItemManager.itemManager.inventoryUi.SetCanvasForPanelInventory();
    }
    private void OnDisable()
    {
        ItemManager.itemManager.backgroundInventoryAdd.SetActive(false);
        Destroy(ItemManager.itemManager.inventoryUi.panelInventory.GetComponent<GraphicRaycaster>());
        Destroy(ItemManager.itemManager.inventoryUi.panelInventory.GetComponent<Canvas>());
        ItemManager.itemManager.inventoryUi.SetSlotCheckAll(false);
        ItemManager.itemManager.isUpgradeCase = false;
    }
}
