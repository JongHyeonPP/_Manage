using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharEquipZone : MonoBehaviour
{
    public int myCharIndex;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI stat_0, stat_1, stat_2;

    private void OnEnable()
    {
        var cm = CharacterManager.characterManager;
        var dataList = cm_DebugSet._data[myCharIndex];

        charName.text = "Hp : "+dataList.hp + " / " + dataList.maxHp;
        stat_0.text = dataList.ability + " ";
        stat_1.text = dataList.resist + " ";
        stat_2.text = dataList.speed + " ";
    }
}
