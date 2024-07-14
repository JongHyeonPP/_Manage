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
    private void Start()
    {
        statusExplain = ItemManager.itemManager.inventoryUi.statusExplain;
    }
    public void OnPointerEnter()
    {
        JobClass job = ItemManager.itemManager.selectedCharacter.jobClass;
        if (job.jobId == "000")
            return;
        statusExplain.transform.parent = transform.parent;
        statusExplain.gameObject.SetActive(true);
        statusExplain.transform.localPosition = transform.localPosition;
        statusExplain.SetExplain(job.name[GameManager.language], job.jobSkill.GetExplain());
        HightlightOn();
    }
    public void OnPointerExit()
    {
        statusExplain.gameObject.SetActive(false);
        HightlightOff();
    }
    public void SetJobIcon(Sprite _jobIcon)
    {
        buttonExclaim.SetActive(false);
        jobIconSlot.gameObject.SetActive(true);
        jobIconSlot.sprite = _jobIcon;
    }
    public void Case000(CharacterData _character)
    {
        jobIconSlot.gameObject.SetActive(false);
        if (_character.skills[0] != null && _character.skills[1] != null)//�� �� null�� �ƴϸ�
        {
            buttonExclaim.SetActive(true);
        }
        else
            buttonExclaim.SetActive(false);
    }
}
