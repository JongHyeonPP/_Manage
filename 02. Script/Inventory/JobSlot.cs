using BattleCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobSlot : SlotBase
{
    StatusExplain statusExplain;
    public Image jobIconSlot;
    public GameObject buttonExclaim;
    public bool isInGetJob;
    private void Start()
    {
        statusExplain = ItemManager.itemManager.inventoryUi.statusExplain;
    }
    public void OnPointerEnter()
    {
        JobClass job;
        if (isInGetJob)
        {
            job = GameManager.gameManager.GetJob(ItemManager.itemManager.selectedCharacter.skillAsIItems[0].itemId, ItemManager.itemManager.selectedCharacter.skillAsIItems[1].itemId);
        }
        else
        {
            job = ItemManager.itemManager.selectedCharacter.jobClass;
        }
        if (job.jobId == "000")
            return;
        statusExplain.transform.parent = transform.parent;
        statusExplain.gameObject.SetActive(true);
        statusExplain.transform.localPosition = transform.localPosition;
        statusExplain.SetExplain(job.name[GameManager.language], job.jobSkill.explain[0][GameManager.language]);
        HightlightOn();
    }
    public void OnPointerExit()
    {
        statusExplain.gameObject.SetActive(false);
        HightlightOff();
    }
    public void SetJobIcon(JobClass _job)
    {
        var _jobIcon = _job.jobIcon;
        buttonExclaim.SetActive(false);
        jobIconSlot.gameObject.SetActive(true);
        jobIconSlot.sprite = _jobIcon;
    }
    public void Case000(CharacterData _character)
    {
        jobIconSlot.gameObject.SetActive(false);
        if (_character.skillAsIItems[0] != null && _character.skillAsIItems[1] != null)//둘 다 null이 아니면
        {
            buttonExclaim.SetActive(true);
        }
        else
            buttonExclaim.SetActive(false);
    }
}
