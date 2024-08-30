using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : SlotBase
{
    public int index;
    public TMP_Text textName;
    public TMP_Text textLv;
    public GameObject button_Up;
    public string curId;
    public Image imageIcon;
    public GameObject imageFame;
    public TMP_Text textFame;
    public void OnUpBtnClicked()
    {
        SoundManager.SfxPlay("Decline");
        GameManager.lobbyScenario.OnUpBtnClicked(this);
    }
    public void OnPointerEnter()
    {
        HighlightOn();
        GameManager.lobbyScenario.OnPointerEnter_Slot(this);
    }
    public void OnPointerExit()
    {
        HighlightOff();
        GameManager.lobbyScenario.OnPointerExit_Slot();
    }
}
