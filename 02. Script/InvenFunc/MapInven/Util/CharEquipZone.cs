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
        CharUnit dataList = SGT_GUI_CharData.getSGT().dataList[myCharIndex];
        charName.text = dataList.name;
        stat_0.text = dataList.stat.x + " ";
        stat_1.text = dataList.stat.y + " ";
        stat_2.text = dataList.stat.z + " ";
    }
}
