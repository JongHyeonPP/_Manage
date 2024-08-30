using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusSlot_Equip : SlotBase, IPointerEnterHandler, IPointerExitHandler
{
    StatusExplain statusExplain;
    public StatusType statusType;
    private void Start()
    {
        statusExplain = ItemManager.itemManager.inventoryUi.statusExplain;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightOn();
        statusExplain.transform.parent = transform.parent;
        statusExplain.gameObject.SetActive(true);
        statusExplain.transform.localPosition = transform.localPosition;
        CharacterData data =  ItemManager.itemManager.selectedCharacter;
        if (data == null)
            return;
        switch (statusType)
        {
            case StatusType.Hp:
                float maxHp = data.maxHp;
                float hp = data.hp;
                statusExplain.SetHpExplain(maxHp, hp);
                break;
            case StatusType.Ability:
                float ability = data.ability;
                statusExplain.SetAbilityExplain(ability);
                break;
            case StatusType.Resist:
                float resist = data.resist;
                statusExplain.SetResistExplain(resist);
                break;
            case StatusType.Speed:
                float speed = data.speed;
                statusExplain.SetSpeedExplain(speed);
                break;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightOff();
        statusExplain.gameObject.SetActive(false);
    }
}