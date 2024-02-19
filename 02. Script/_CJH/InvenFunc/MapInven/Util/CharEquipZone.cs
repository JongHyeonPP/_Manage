using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharEquipZone : MonoBehaviour
{
    public int myCharIndex;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI stat_0, stat_1, stat_2;
    private CJH_CharacterData cash;
    private void OnEnable()
    {
        RefreshText();
    }

    public void RefreshText()
    {
        if(cash == null && cash != CJH_CharacterData.getSGT())
        {
            cash = CJH_CharacterData.getSGT();
            cash.viewData[myCharIndex] = this;
        }

        CharacterData cm = CJH_CharacterData.getCharData(myCharIndex);

        charName.text = "Hp : " + cm.hp + " / " + cm.maxHp;
        stat_0.text = cm.ability + " ";
        stat_1.text = cm.resist + " ";
        stat_2.text = cm.speed + " ";
    }
}
