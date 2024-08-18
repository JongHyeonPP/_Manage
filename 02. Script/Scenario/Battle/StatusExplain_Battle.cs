using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusExplain_Battle : MonoBehaviour
{
    [SerializeField] TMP_Text textName;
    [SerializeField] StatusSlot_Battle hpSlot;
    [SerializeField] StatusSlot_Battle abilitySlot;
    [SerializeField] StatusSlot_Battle resistSlot;
    [SerializeField] StatusSlot_Battle speedSlot;
    public void SetExplain(BaseInBattle owner)
    {
        textName.text = owner.name[GameManager.language];
        hpSlot.textStatus.text = owner.maxHp.ToString("F0");
        abilitySlot.textStatus.text = owner.ability.ToString("F0");
        resistSlot.textStatus.text = owner.resist.ToString("F0");
        speedSlot.textStatus.text = owner.speed.ToString("F1");
    }
}
