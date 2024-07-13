using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobExplainSlot : SlotBase
{
    StatusExplain statusExplain;
    private void Start()
    {
        statusExplain = ItemManager.itemManager.inventoryUi.statusExplain;
    }
    public void OnPointerEnter()
    {
        return;
        statusExplain.transform.parent = transform.parent;
        statusExplain.gameObject.SetActive(true);
        statusExplain.transform.localPosition = transform.localPosition;
        JobClass job = ItemManager.itemManager.selectedCharacter.jobClass;
        statusExplain.SetExplain(job.name[GameManager.language], job.effectExplain[GameManager.language]);
        HightlightOn();
    }
    public void OnPointerExit()
    {
        statusExplain.gameObject.SetActive(false);
        HightlightOff();
    }
}
